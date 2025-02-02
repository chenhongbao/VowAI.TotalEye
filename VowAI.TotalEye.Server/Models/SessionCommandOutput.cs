namespace VowAI.TotalEye.Server.Models
{
    public class SessionCommandOutput
    {
        public int CommandOuputId { get; set; }
        public string? CommandOutput { get; set; }
        public UserInfoSession? Session { get; set; }
    }
}