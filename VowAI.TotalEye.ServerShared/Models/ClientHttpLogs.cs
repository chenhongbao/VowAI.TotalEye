namespace VowAI.TotalEye.ServerShared.Models
{
    public class ClientHttpLogs
    {
        public int LogsId { get; set; }
        public ICollection<ClientHttpLogItem> Logs { get; set; } = new List<ClientHttpLogItem>();
    }
}
