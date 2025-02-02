using Microsoft.AspNetCore.Mvc;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadCommandOutputController : ControllerBase
    {
        private readonly IPolicyExecutor _policyExecutor;

        public UploadCommandOutputController(IPolicyExecutor policyExecutor)
        {
            _policyExecutor = policyExecutor;
        }

        [HttpPost]
        public async Task<ActionResult<ClientControlPolicy>> Post([FromForm] string token, [FromForm] string payload)
        {
            try
            {
                return await _policyExecutor.ApplyCommandPolicy(token, payload);
            }
            catch (Exception exception)
            {
                exception.WriteString<UploadImageController>();
                return BadRequest(exception);
            }
        }
    }
}
