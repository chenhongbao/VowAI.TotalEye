namespace VowAI.TotalEye.Server.Models
{
    public class UserInfoSession
    {
        public int SessionId { get; set; }
        public string? Token { get;set; }
        public UserInfoRequest? Request { get; set; }
        public ControlPolicy? Policy { get; set; }
        public UserHttpLog? Logs { get; set; }
        public UserCommandOutput? CommandOutput { get; set; }
        public UserScreenshot? Screenshot { get; set; }
    }
}
