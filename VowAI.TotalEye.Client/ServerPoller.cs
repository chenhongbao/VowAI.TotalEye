using System.Net.Http.Json;
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
            ServerInfoRequest? request;
            ClientControlPolicy? policy;

            if ((request = await Ask(client)) == null)
            {
                throw new InvalidDataException("Fail obtaining request information from centre.");
            }
            else if ((policy = await Reply(client, request)) == null)
            {
                throw new InvalidDataException("Fail obtaining client control policy from centre.");
            }
            else
            {
                ApplyPolicy(policy);
            }
        }

        private async Task<ServerInfoRequest?> Ask(HttpClient client)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(_configuration.AskUrl, new VerifiedUser { UserId = _configuration.UserId, Pin = _configuration.Pin });

            if (response.IsSuccessStatusCode == false)
            {
                throw new InvalidOperationException($"Fail to read from centre: HTTP {response.StatusCode}.");
            }

            return await response.Content.ReadFromJsonAsync<ServerInfoRequest>();
        }

        private async Task<ClientControlPolicy?> Reply(HttpClient client, ServerInfoRequest request)
        {
            switch (request.Name.ToLower())
            {
                case "client_screenshot":

                    return await UploadImageFromPath(client, request);

                case "client_command":

                    return await UploadCommandOutput(client, request);

                case "http_logs":

                    return await UploadHttpLogs(client, request);

                default:

                    throw new ArgumentException($"Unknown request from centre '{request.Name}'.");
            }
        }

        private async Task<ClientControlPolicy?> UploadImageFromPath(HttpClient client, ServerInfoRequest request)
        {
            string location = LocalComputer.RunCommand("VowAI.TotalEye.CatchScreen.exe /Destination:Screenshot.jpg");

            return await UploadFile(client, request, new FileInfo(location).Name, File.ReadAllBytes(location));
        }

        private async Task<ClientControlPolicy?> UploadHttpLogs(HttpClient client, ServerInfoRequest request)
        {
            return await UploadText(client, request, JsonSerializer.Serialize(_httpSniffer.ReadHttpLogs()));
        }

        private async Task<ClientControlPolicy?> UploadCommandOutput(HttpClient client, ServerInfoRequest request)
        {
            return await UploadText(client, request, LocalComputer.RunCommand(request.Description));
        }

        private void ApplyPolicy(ClientControlPolicy policy)
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

        private async Task<ClientControlPolicy?> UploadFile(HttpClient client, ServerInfoRequest request, string name, byte[] bytes)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            StringContent tokenContent = new StringContent(request.Token);
            ByteArrayContent imageContent = new ByteArrayContent(bytes);

            imageContent.Headers.Add("Content-Type", "application/octet-stream");

            content.Add(tokenContent, "token");
            content.Add(imageContent, "payload", name);

            HttpResponseMessage response = await client.PostAsync(request.ReplyUrl, content);
            return await response.Content.ReadFromJsonAsync<ClientControlPolicy>();
        }

        private async Task<ClientControlPolicy?> UploadText(HttpClient client, ServerInfoRequest request, string text)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            StringContent token = new StringContent(request.Token, Encoding.UTF8);
            StringContent uploadContent = new StringContent(text, Encoding.UTF8, "text/plain; charset=UTF-8");

            content.Add(token, "token");
            content.Add(uploadContent, "payload");

            HttpResponseMessage response = await client.PostAsync(request.ReplyUrl, content);
            return await response.Content.ReadFromJsonAsync<ClientControlPolicy>();
        }

        #endregion
    }
}
