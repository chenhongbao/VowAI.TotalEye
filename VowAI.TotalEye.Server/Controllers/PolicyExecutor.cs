using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Server.Controllers
{
    public class PolicyExecutor : IPolicyExecutor
    {
        public Task<ClientControlPolicy> ApplyCommandPolicy(string token, string commandOutput)
        {
            throw new NotImplementedException();
        }

        public Task<ClientControlPolicy> ApplyHttpPolicy(string token, ClientHttpLogs logs)
        {
            throw new NotImplementedException();
        }

        public Task<ClientControlPolicy> ApplyScreenshotPolicy(string token, ImageItem image)
        {
            throw new NotImplementedException();
        }
    }
}
