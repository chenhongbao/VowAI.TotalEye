using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Client
{
    public interface IClientControlPolicyProvider
    {
        public ClientControlPolicy? GetPolicy(string tag);
    }
}
