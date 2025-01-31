namespace VowAI.TotalEye.Models
{
    public interface IClientControlPolicy
    {
        public string Tag { get; }
        public ICollection<ControlPolicyItem> Policies { get; }
    }
}
