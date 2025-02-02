using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Client
{
    public interface IClientControlPolicyProvider
    {
        public ClientControlPolicySet? GetPolicy(string tag);
    }
}
