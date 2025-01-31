namespace VowAI.TotalEye.Models
{
    public interface IClientControlPolicyProvider
    {
        public IClientControlPolicy GetPolicy(string tag);
    }
}
