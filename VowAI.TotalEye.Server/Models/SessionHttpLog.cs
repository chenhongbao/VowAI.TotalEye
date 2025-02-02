namespace VowAI.TotalEye.Server.Models
{
    public class SessionHttpLog
    {
        public int LogId {  get; set; }
        public string? Log { get; set; }
        public UserInfoSession? Session { get; set; }
    }
}
