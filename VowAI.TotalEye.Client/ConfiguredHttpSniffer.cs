using System.Text;
using Titanium.Web.Proxy.EventArguments;
using VowAI.TotalEye.ServerShared;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Client
{
    public class ConfiguredHttpSniffer : IConfiguredHttpSniffer
    {
        private readonly IClientControlPolicyProvider _policyProvider;
        private readonly IHttpSnifferConfiguration _configuration;
        private readonly HttpSniffer _httpSniffer;

        private readonly string LOG_FILE = "HTTP_Activity.log";

        public ConfiguredHttpSniffer(IHttpSnifferConfiguration configuration, IClientControlPolicyProvider policyProvider)
        {
            _configuration = configuration;
            _policyProvider = policyProvider;
            _httpSniffer = new HttpSniffer(_configuration.Port, OnTunnelConnect, OnRequest, OnResponse);
        }

        public void Dispose()
        {
            _httpSniffer.Dispose();
        }

        public ClientHttpLogs ReadHttpLogs()
        {
            DirectoryInfo directory = LocalComputer.GetApplicationDirectory<ConfiguredHttpSniffer>();
            string path = Path.Combine(directory.FullName, LOG_FILE);
            ClientHttpLogs logs = ReadHttpLogsFile(path);

            File.WriteAllText(path, "");

            return logs;
        }

        private void OnTunnelConnect(TunnelConnectSessionEventArgs args)
        {
            ClientControlPolicySet? policySet = _policyProvider.GetPolicy("http_connect");

            if (policySet != null)
            {
                ApplyConnectPolicy(args, policySet);
            }
        }

        private void OnRequest(SessionEventArgs args)
        {
            ClientControlPolicySet? policySet = _policyProvider.GetPolicy("http_request");

            if (policySet != null)
            {
                ApplySessionPolicy(args, policySet);
            }
        }

        private void OnResponse(SessionEventArgs args)
        {
            WriteHttpLog(args);

            ClientControlPolicySet? policySet = _policyProvider.GetPolicy("http_response");

            if (policySet != null)
            {
                ApplySessionPolicy(args, policySet);
            }
        }

        private ClientHttpLogs ReadHttpLogsFile(string path)
        {
            //TODO Read client's HTTP logs.
            throw new NotImplementedException();
        }

        private void WriteHttpLog(SessionEventArgs args)
        {
            DirectoryInfo directory = LocalComputer.GetApplicationDirectory<ConfiguredHttpSniffer>();
            string path = Path.Combine(directory.FullName, LOG_FILE);

            try
            {
                File.AppendAllText(path, BuildHttpActivityLog(args), Encoding.UTF8);
            }
            catch (Exception exception)
            {
                exception.WriteString<ConfiguredHttpSniffer>();
            }
        }

        private string BuildHttpActivityLog(SessionEventArgs args)
        {
            return $"{DateTime.Now}\t{args.HttpClient.Request.Method}\t{args.HttpClient.Request.RequestUri.Host}";
        }

        private void ApplyConnectPolicy(TunnelConnectSessionEventArgs args, ClientControlPolicySet policySet)
        {
            if (policySet.Policies.Any())
            {
                string hostName = args.HttpClient.Request.RequestUri.Host;

                foreach (ClientControlPolicyItem policyItem in policySet.GetPolicyItems())
                {
                    if (policyItem.FilterWords.Split([';', ',']).Any(word => ApplyCondition(policyItem.FilterCondition, hostName, word)))
                    {
                        ApplyConnectAction(args, policyItem.Action, policyItem.ActionDescription);
                        break;
                    }
                }
            }
        }

        private void ApplySessionPolicy(SessionEventArgs args, ClientControlPolicySet policySet)
        {
            if (policySet.Policies != null && policySet.Policies.Any())
            {
                string hostname = args.HttpClient.Request.RequestUri.Host;

                foreach (ClientControlPolicyItem policyItem in policySet.GetPolicyItems())
                {
                    if (policyItem.FilterWords.Split([';', ',']).Any(word => ApplyCondition(policyItem.FilterCondition, hostname, word)))
                    {
                        ApplySessionAction(args, policyItem.Action, policyItem.ActionDescription);
                        break;
                    }
                }
            }
        }

        private void ApplySessionAction(SessionEventArgs args, string action, string actionDescription)
        {
            switch (action.ToLower())
            {
                case "http_session_redirect":

                    args.Redirect(actionDescription);
                    break;

                case "http_session_ok":

                    args.Ok(actionDescription);
                    break;

                default:

                    throw new ArgumentException($"Unknown action '{action}'.");
            }
        }

        private bool ApplyCondition(string condition, string hostname, string word)
        {
            return condition.ToLower() switch
            {
                "contain" => hostname.Contains(word),
                "equal" => hostname.Equals(word),
                _ => throw new ArgumentException($"Unknown filter condition '{condition}'."),
            };
        }

        private void ApplyConnectAction(TunnelConnectSessionEventArgs args, string action, string actionDescription)
        {
            switch (action.ToLower())
            {
                case "http_connect_deny":

                    args.DenyConnect = true;
                    break;

                case "http_connect_ignore":

                    args.DecryptSsl = false;
                    break;

                default:

                    throw new ArgumentException($"Unknown action '{action}'.");
            }
        }
    }
}
