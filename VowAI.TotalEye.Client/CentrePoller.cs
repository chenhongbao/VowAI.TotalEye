using System.Text.Json;
using System.Threading;
using VowAI.TotalEye.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Client
{
    internal partial class Program
    {
        public class CentrePoller : IDisposable
        {
            private readonly ICentrePollerConfiguration _configuration;
            private readonly IHttpClientFactory _clientFactory;
            private readonly IConfiguredHttpSniffer _httpSniffer;
            private readonly Timer _timer;

            public CentrePoller(ICentrePollerConfiguration configuration, IHttpClientFactory clientFactory, IConfiguredHttpSniffer httpSniffer)
            {
                _configuration = configuration;
                _clientFactory = clientFactory;
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

                        TryDownlodaUpdate(client);
                        Poll(await client.GetAsync(_configuration.Url));
                    }
                    catch (Exception exception)
                    {
                        exception.Write<CentrePoller>();
                    }

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(90));
                    }
                    catch (Exception exception)
                    {
                        exception.Write<CentrePoller>();
                    }
                }
            }

            private async void Poll(HttpResponseMessage response)
            {
                string body =  await response.Content.ReadAsStringAsync();

                //TODO Can't use JSON. How to encode the message?
            }

            private void TryDownlodaUpdate(HttpClient client)
            {
                //TODO Download update.
            }

            public void Dispose()
            {
                _httpSniffer.Dispose();
            }
        }
    }
}
