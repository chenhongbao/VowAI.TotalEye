using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Server.Models
{
    public class ControlPolicy
    {
        public int PolicyId { get; set; }
        public int Description { get; set; }
        public string? Tag { get; set; }
        public ICollection<ClientControlPolicyItem>? Policies { get; set; }
    }
}
