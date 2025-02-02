namespace VowAI.TotalEye.ServerShared.Models
{
    public class ClientControlPolicySet
    {
        public int PolicySetId { get; set; }
        public ICollection<ClientControlPolicy> Policies { get; set; } = new List<ClientControlPolicy>();
    }
}
