namespace VowAI.TotalEye.Server.Models
{
    public class UserInfoSession
    {
        public int SessionId { get; set; }
        public string? Token { get;set; }
        public UserInfoRequest? Request { get; set; }
        public SessionHttpLog? HttpLogs { get; set; }
        public SessionCommandOutput? CommandOutput { get; set; }
        public SessionScreenshot? Screenshot { get; set; }
    }
}
