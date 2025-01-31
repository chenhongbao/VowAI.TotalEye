namespace VowAI.TotalEye.Client
{
    public interface IConfiguredHttpSniffer: IDisposable
    {
        public string ReadActivityLogs();
    }
}