namespace VowAI.TotalEye.Server.Models
{
    public class UserHttpLog
    {
        public int LogId {  get; set; }
        public User? User { get; set; }
        public string? Log { get; set; }
    }
}
