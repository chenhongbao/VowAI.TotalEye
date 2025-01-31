
namespace VowAI.TotalEye.Models
{
    public class ClientControlPolicy : IClientControlPolicy
    {
        public int PolicyId { get; set; }
        public string Tag { get; set; }

        public ICollection<ControlPolicyItem> Policies { get; set; }
    }
}
