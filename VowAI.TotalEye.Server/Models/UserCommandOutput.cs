namespace VowAI.TotalEye.Server.Models
{
    public class UserCommandOutput
    {
        public int CommandOuputId { get; set; }
        public User? User { get; set; }
        public string? CommandOutput { get; set; }
    }
}