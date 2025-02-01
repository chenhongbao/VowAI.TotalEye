namespace VowAI.TotalEye.ServerShared.Models
{
    public class ClientControlPolicy
    {
        public int PolicyId { get; set; }
        public string Tag { get; set; }

        public ICollection<ControlPolicyItem> Policies { get; set; }
    }
}
