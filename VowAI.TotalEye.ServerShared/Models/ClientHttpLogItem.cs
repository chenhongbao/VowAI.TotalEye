namespace VowAI.TotalEye.ServerShared.Models
{
    public class ClientHttpLogItem
    {
        public int LogId { get; set; }
        public string Method { get; set; }
        public string Host { get; set; }
        public DateTime Timestamp { get; set; }
    }
}