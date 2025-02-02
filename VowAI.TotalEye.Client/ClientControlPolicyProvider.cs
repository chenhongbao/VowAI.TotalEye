using System.Collections.Concurrent;
using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Client
{
    public class ClientControlPolicyProvider : IClientControlPolicyProvider
    {
        private readonly ConcurrentBag<ClientControlPolicy> _policies = new();

        public ClientControlPolicySet? GetPolicy(string tag)
        {
            ClientControlPolicySet policySet = new()
            {
                Policies = new List<ClientControlPolicy>()
            };

            foreach (var policy in _policies)
            {
                if (policy.Tag == tag)
                {
                    policySet.Policies.Add(policy);
                }
            }
            return policySet;
        }

        public void SetPolicy(ClientControlPolicy policy)
        {
            _policies.Add(policy);
        }
    }
}
