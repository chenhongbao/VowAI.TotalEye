using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Client
{
    public interface IConfiguredHttpSniffer: IDisposable
    {
        public ClientHttpLogs ReadHttpLogs();
    }
}