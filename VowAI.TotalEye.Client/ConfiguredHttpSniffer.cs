using System.Text;
using Titanium.Web.Proxy.EventArguments;
using VowAI.TotalEye.Models;
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

        public string ReadActivityLogs()
        {
            DirectoryInfo directory = LocalComputer.GetApplicationDirectory<ConfiguredHttpSniffer>();
            string path = Path.Combine(directory.FullName, LOG_FILE);
            string text = File.ReadAllText(path, Encoding.UTF8);

            File.WriteAllText(path, "");

            return text;
        }

        private void OnTunnelConnect(TunnelConnectSessionEventArgs args)
        {
            IClientControlPolicy? policy = _policyProvider.GetPolicy("http_connect");

            if (policy != null)
            {
                ApplyConnectPolicy(args, policy);
            }
        }

        private void OnRequest(SessionEventArgs args)
        {
            IClientControlPolicy? policy = _policyProvider.GetPolicy("http_request");

            if (policy != null)
            {
                ApplySessionPolicy(args, policy);
            }
        }

        private void OnResponse(SessionEventArgs args)
        {
            WriteActivityLog(args);

            IClientControlPolicy? policy = _policyProvider.GetPolicy("http_response");

            if (policy != null)
            {
                ApplySessionPolicy(args, policy);
            }
        }

        private void WriteActivityLog(SessionEventArgs args)
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

        private void ApplyConnectPolicy(TunnelConnectSessionEventArgs args, IClientControlPolicy policy)
        {
            if (policy.Policies != null && policy.Policies.Any())
            {
                string hostname = args.HttpClient.Request.RequestUri.Host;

                foreach (ControlPolicyItem policyItem in policy.Policies)
                {
                    if (policyItem.FilterWords.Split([';', ',']).Any(word => ApplyCondition(policyItem.FilterCondition, hostname, word)))
                    {
                        ApplyConnectAction(args, policyItem.Action, policyItem.ActionDescription);
                        break;
                    }
                }
            }
        }

        private void ApplySessionPolicy(SessionEventArgs args, IClientControlPolicy policy)
        {
            if (policy.Policies != null && policy.Policies.Any())
            {
                string hostname = args.HttpClient.Request.RequestUri.Host;

                foreach (ControlPolicyItem policyItem in policy.Policies)
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
                case "redirect":

                    args.Redirect(actionDescription);
                    break;

                case "ok":

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
                case "deny":

                    args.DenyConnect = true;
                    break;

                case "ignore":

                    args.DecryptSsl = false;
                    break;

                default:

                    throw new ArgumentException($"Unknown action '{action}'.");
            }
        }
    }
}
