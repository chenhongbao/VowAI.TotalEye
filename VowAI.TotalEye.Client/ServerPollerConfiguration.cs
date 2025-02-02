namespace VowAI.TotalEye.Client
{
    public class ServerPollerConfiguration : IServerPollerConfiguration
    {
        public string LoginUrl { get; set; } = "";
        public string AskUrl { get; set; } = "";
        public int UserId { get; set; } = 0;
        public string Password { get; set; } = "";
        public string Pin { get; set; } = "";
    }
}
