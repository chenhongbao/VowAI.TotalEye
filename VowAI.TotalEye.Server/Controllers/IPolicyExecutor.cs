using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Server.Controllers
{
    public interface IPolicyExecutor
    {
        public Task<ClientControlPolicySet> ApplyHttpPolicy(string token, ClientHttpLogs logs);
        public Task<ClientControlPolicySet> ApplyScreenshotPolicy(string token, ImageItem image);
        public Task<ClientControlPolicySet> ApplyCommandPolicy(string token, string commandOutput);
    }
}
