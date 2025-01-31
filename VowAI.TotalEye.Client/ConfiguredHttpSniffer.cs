using Titanium.Web.Proxy.EventArguments;
using VowAI.TotalEye.Models;

namespace VowAI.TotalEye.Client
{
    internal partial class Program
    {
        public class ConfiguredHttpSniffer: IConfiguredHttpSniffer
        {
            private readonly IClientControlPolicyProvider _policyProvider;
            private readonly IHttpSnifferConfiguration _configuration;
            private readonly HttpSniffer _httpSniffer;

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

            private void OnTunnelConnect(TunnelConnectSessionEventArgs args)
            {
                IClientControlPolicy? policy = _policyProvider.GetPolicy("HTTP_Connect");

                if (policy != null)
                {
                    ApplyConnectPolicy(args, policy);
                }
            }

            private void OnRequest(SessionEventArgs args)
            {
                IClientControlPolicy? policy = _policyProvider.GetPolicy("HTTP_Request");

                if (policy != null)
                {
                    ApplySessionPolicy(args, policy);
                }
            }

            private void OnResponse(SessionEventArgs args)
            {
                IClientControlPolicy? policy = _policyProvider.GetPolicy("HTTP_Response");

                if (policy != null)
                {
                    ApplySessionPolicy(args, policy);
                }
            }

            private void ApplyConnectPolicy(TunnelConnectSessionEventArgs args, IClientControlPolicy policy)
            {
                string hostname = args.HttpClient.Request.RequestUri.Host;

                foreach (ControlPolicyItem policyItem in policy.Policies)
                {
                    if (policyItem.FilterWords.Split(';').Any(word => ApplyCondition(policyItem.FilterCondition, hostname, word)))
                    {
                        ApplyConnectAction(args, policyItem.Action, policyItem.ActionDescription);
                        break;
                    }
                }
            }

            private void ApplySessionPolicy(SessionEventArgs args, IClientControlPolicy policy)
            {
                string hostname = args.HttpClient.Request.RequestUri.Host;

                foreach (ControlPolicyItem policyItem in policy.Policies)
                {
                    if (policyItem.FilterWords.Split(';').Any(word => ApplyCondition(policyItem.FilterCondition, hostname, word)))
                    {
                        ApplySessionAction(args, policyItem.Action, policyItem.ActionDescription);
                        break;
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
}
