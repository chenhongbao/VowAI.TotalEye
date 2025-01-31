using VowAI.TotalEye.Models;

namespace VowAI.TotalEye.Client
{
    public interface IClientControlPolicyProvider
    {
        public IClientControlPolicy? GetPolicy(string tag);
    }
}
