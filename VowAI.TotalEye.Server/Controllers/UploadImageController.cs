using Microsoft.AspNetCore.Mvc;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadImageController : ControllerBase
    {
        private readonly IPolicyExecutor _policyExecutor;

        public UploadImageController(IPolicyExecutor policyExecutor)
        {
            _policyExecutor = policyExecutor;
        }

        [HttpPost]
        public async Task<ActionResult<ClientControlPolicy>> Post([FromForm] string token, IFormFile payload)
        {
            try
            {
                return await _policyExecutor.ApplyScreenshotPolicy(token, BuildImageItem(payload));
            }
            catch (Exception exception)
            {
                exception.WriteString<UploadImageController>();
                return BadRequest(exception);
            }
        }

        private ImageItem BuildImageItem(IFormFile payload)
        {
            ImageItem imageItem = new ImageItem
            {
                FileName = payload.FileName,
                Timestamp = DateTime.Now,
                Data = new byte[payload.Length],
            };

            using (Stream buffer = new MemoryStream(imageItem.Data))
            {
                payload.CopyToAsync(buffer);
            }

            return imageItem;
        }
    }
}
