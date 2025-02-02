namespace VowAI.TotalEye.Server.Models
{
    public class UserPolicyBinding
    {
        public int BindingId { get; set; }
        public User User { get; set; }
        public ServerControlPolicy Policy { get; set; }
    }
}
