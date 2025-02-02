using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetControlPolicyController : ControllerBase
    {
        private readonly IDbContextFactory<ServerDbContext> _factory;

        public GetControlPolicyController(IDbContextFactory<ServerDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public async Task<ActionResult<ClientControlPolicySet>> Get(int userId, string pin)
        {
            try
            {
                using (ServerDbContext context = _factory.CreateDbContext())
                {
                    User? user = await context.Users
                        .Include(u => u.Policies)
                        .FirstOrDefaultAsync(u => u.UserId == userId);

                    if (user == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok(user.GetPolicySet());
                    }
                }
            }
            catch (Exception exception)
            {
                exception.WriteString<GetControlPolicyController>();
                return BadRequest(exception);
            }
        }

        public static string GetControllerName()
        {
            return nameof(GetControlPolicyController).Replace("Controller", "");
        }
    }
}
