using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Server.Controllers
{
    public interface IPolicyExecutor
    {
        public Task<ClientControlPolicy> ApplyHttpPolicy(string token, ClientHttpLogs logs);
        public Task<ClientControlPolicy> ApplyScreenshotPolicy(string token, ImageItem image);
        public Task<ClientControlPolicy> ApplyCommandPolicy(string token, string commandOutput);
    }
}
