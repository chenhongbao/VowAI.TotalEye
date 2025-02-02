namespace VowAI.TotalEye.Server.Models
{
    public class UserInfoHistory
    {
        public int HistoryId { get; set; }
        public User? User { get; set; }
        public ICollection<UserHttpLog>? HttpLogs { get; set; }
        public ICollection<UserScreenshot>? Screenshots { get; set; }
        public ICollection<UserCommandOutput>? CommandOutputs { get; set; }
    }
}
