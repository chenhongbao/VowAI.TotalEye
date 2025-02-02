using Microsoft.AspNetCore.Mvc;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadHttpLogsController : ControllerBase
    {
        private readonly IPolicyExecutor _policyExecutor;

        public UploadHttpLogsController(IPolicyExecutor policyExecutor)
        {
            _policyExecutor = policyExecutor;
        }

        [HttpPost]
        public async Task<ActionResult<ClientControlPolicySet>> Post([FromForm] string token, [FromForm] ClientHttpLogs payload)
        {
            try
            {
                return await _policyExecutor.ApplyHttpPolicy(token, payload);
            }
            catch (Exception exception)
            {
                exception.WriteString<UploadHttpLogsController>();
                return BadRequest(exception);
            }
        }
    }
}
