namespace VowAI.TotalEye.ServerShared.Models
{
    public class ClientControlPolicy
    {
        public int PolicyId { get; set; }
        public int Description { get; set; }
        public string Tag { get; set; }
        public ICollection<ClientControlPolicyItem> Policies { get; set; }
    }
}
