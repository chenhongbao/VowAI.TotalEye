﻿using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using VowAI.TotalEye.ClientShared;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Client
{
    public class ServerPoller : IDisposable
    {
        private readonly IServerPollerConfiguration _configuration;
        private readonly IUpdateConfiguration _updateConfiguration;
        private readonly IClientControlPolicyProvider _policyProvider;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguredComputerSniffer _computerSniffer;
        private readonly IConfiguredHttpSniffer _httpSniffer;

        public ServerPoller(IServerPollerConfiguration configuration, IUpdateConfiguration updateConfiguration, IClientControlPolicyProvider policyProvider, IHttpClientFactory clientFactory, IConfiguredComputerSniffer computerSniffer, IConfiguredHttpSniffer httpSniffer)
        {
            _configuration = configuration;
            _updateConfiguration = updateConfiguration;
            _policyProvider = policyProvider;
            _clientFactory = clientFactory;
            _computerSniffer = computerSniffer;
            _httpSniffer = httpSniffer;

            Task.Run(() => PollCentre(null));
        }

        private async void PollCentre(object? state)
        {
            while (true)
            {
                try
                {
                    HttpClient client = _clientFactory.CreateClient();

                    //TODO Need authentication.

                    TryDownloadUpdate(client);
                    Poll(client);
                }
                catch (Exception exception)
                {
                    exception.WriteString<ServerPoller>();
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(90));
                }
                catch (Exception exception)
                {
                    exception.WriteString<ServerPoller>();
                }
            }
        }

        private async void TryDownloadUpdate(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync(_updateConfiguration.VersionUrl);

            if (response.IsSuccessStatusCode == false)
            {
                throw new InvalidOperationException($"Fail to read update information: HTTP {response.StatusCode}.");
            }

            string text = await response.Content.ReadAsStringAsync();
            UpdateConfiguration newConfiguration = new UpdateConfiguration().Deserialize(text);

            if (newConfiguration.Version > _updateConfiguration.Version)
            {
                DownloadFile(client, newConfiguration.FileUrl);
                File.WriteAllText(newConfiguration.GetLocalPath(), text);
            }
        }

        private async void Poll(HttpClient client)
        {
            ClientInfoRequest? request;
            ClientControlPolicySet? policySet;

            if ((request = await Ask(client)) == null)
            {
                throw new InvalidDataException("Fail obtaining request information from centre.");
            }
            else if ((policySet = await Reply(client, request)) == null)
            {
                throw new InvalidDataException("Fail obtaining client control policy from centre.");
            }
            else
            {
                ApplyPolicy(policySet);
            }
        }

        private async Task<ClientInfoRequest?> Ask(HttpClient client)
        {
            return await client.GetFromJsonAsync<ClientInfoRequest>(_configuration.GetInfoRequestUrl + "?userId=" + _configuration.UserId);
        }

        private async Task<ClientControlPolicySet?> Reply(HttpClient client, ClientInfoRequest request)
        {
            switch (request.Name.ToLower())
            {
                case "client_screenshot":

                    return await UploadImageFromPath(client, request);

                case "client_command":

                    return await UploadCommandOutput(client, request);

                case "http_logs":

                    return await UploadHttpLogs(client, request);

                case "":

                    return await GetControlPolicySet(client, request);

                default:

                    throw new ArgumentException($"Unknown request from centre '{request.Name}'.");
            }
        }

        private async Task<ClientControlPolicySet?> GetControlPolicySet(HttpClient client, ClientInfoRequest request)
        {
            string url = request.ReplyUrl + "?userId=" + _configuration.UserId + "&token=" + request.Token;

            return await client.GetFromJsonAsync<ClientControlPolicySet>(url);
        }

        private async Task<ClientControlPolicySet?> UploadImageFromPath(HttpClient client, ClientInfoRequest request)
        {
            string location = LocalComputer.RunCommand("VowAI.TotalEye.CatchScreen.exe /Destination:Screenshot.jpg");

            return await UploadFile(client, request, new FileInfo(location).Name, File.ReadAllBytes(location));
        }

        private async Task<ClientControlPolicySet?> UploadHttpLogs(HttpClient client, ClientInfoRequest request)
        {
            return await UploadText(client, request, _httpSniffer.ReadHttpLogs());
        }

        private async Task<ClientControlPolicySet?> UploadCommandOutput(HttpClient client, ClientInfoRequest request)
        {
            return await UploadText(client, request, LocalComputer.RunCommand(request.Description));
        }

        private void ApplyPolicy(ClientControlPolicySet policySet)
        {
            if (policySet.Policies != null)
            {
                foreach (var policy in policySet.Policies)
                {
                    switch (policy.Tag.ToLower())
                    {
                        case "http_connect":
                        case "http_request":
                        case "http_response":
                        case "local_computer":

                            (_policyProvider as ClientControlPolicyProvider)?.SetPolicy(policy);
                            break;

                        case "" /* No further action. */:
                            break;

                        default:

                            throw new ArgumentException($"Unknown policy tag '{policy.Tag}'.");
                    }
                }
            }
        }

        public void Dispose()
        {
            _httpSniffer.Dispose();
            _computerSniffer.Dispose();
        }

        #region Upload and download helper methods

        private async void DownloadFile(HttpClient client, string fileUrl)
        {
            HttpResponseMessage response = await client.GetAsync(fileUrl);

            if (response.IsSuccessStatusCode == false)
            {
                throw new InvalidOperationException($"Fail to get update package: HTTP {response.StatusCode}.");
            }

            Stream downloadStream = await response.Content.ReadAsStreamAsync();

            string downloadDirectory = new UpdateConfiguration().GetDownloadDirectory();
            string downloadFile = $"{(DateTime.Now - DateTime.MinValue).TotalSeconds}.zip";
            string downloadPath = Path.Combine(downloadDirectory, downloadFile);

            using (FileStream file = new FileStream(downloadPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                downloadStream.CopyTo(file);
            }
        }

        private async Task<ClientControlPolicySet?> UploadFile(HttpClient client, ClientInfoRequest request, string name, byte[] bytes)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            StringContent tokenContent = new StringContent(request.Token);
            ByteArrayContent imageContent = new ByteArrayContent(bytes);

            imageContent.Headers.Add("Content-Type", "application/octet-stream");

            content.Add(tokenContent, "token");
            content.Add(imageContent, "payload", name);

            HttpResponseMessage response = await client.PostAsync(request.ReplyUrl, content);
            return await response.Content.ReadFromJsonAsync<ClientControlPolicySet>();
        }

        private async Task<ClientControlPolicySet?> UploadText(HttpClient client, ClientInfoRequest request, string text)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            StringContent token = new StringContent(request.Token, Encoding.UTF8);
            StringContent uploadContent = new StringContent(text, Encoding.UTF8, "text/plain; charset=UTF-8");

            content.Add(token, "token");
            content.Add(uploadContent, "payload");

            HttpResponseMessage response = await client.PostAsync(request.ReplyUrl, content);
            return await response.Content.ReadFromJsonAsync<ClientControlPolicySet>();
        }

        #endregion
    }
}
