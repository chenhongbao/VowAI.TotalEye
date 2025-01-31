using System.Collections.Concurrent;
using VowAI.TotalEye.Models;

namespace VowAI.TotalEye.Client
{
    internal partial class Program
    {
        public class ClientControlPolicyProvider : IClientControlPolicyProvider
        {
            private readonly ConcurrentDictionary<string, ClientControlPolicy> _policies = new();

            public IClientControlPolicy? GetPolicy(string tag)
            {
                return _policies.TryGetValue(tag, out var policy) ? policy : null;
            }

            public ClientControlPolicy? SetPolicy(ClientControlPolicy policy)
            {
                if (_policies.TryAdd(policy.Tag, policy))
                {
                    return null;
                }
                else if (_policies.TryRemove(policy.Tag, out var removed))
                {
                    _policies.TryAdd(policy.Tag, policy);
                    return removed;
                }
                else
                {
                    throw new InvalidOperationException($"Can't remove an existing key '{policy.Tag}'.");
                }
            }
        }
    }
}
