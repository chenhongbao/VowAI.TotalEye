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
        public async Task<ActionResult<ClientControlPolicySet>> Get(int userId, string token)
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
                        UserInfoSession? session = await context.Sessions
                            .Include(s => s.Request)
                            .FirstOrDefaultAsync<UserInfoSession>(session => session.Token == token);

                        if (session == null)
                        {
                            throw new ArgumentException($"Unknown token '{token}'.");
                        }

                        if (session.Request == null)
                        {
                            throw new ArgumentException($"Session with token '{token}' has no request.");
                        }

                        session.Request.Status = "filled";
                        context.Entry(session.Request).State = EntityState.Modified;

                        await context.SaveChangesAsync();

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

        public static string GetControllerUrl(int? userId, string? pin)
        {
            return "api/" + GetControlPolicyController.GetControllerName() + "?userId=" + userId + "&pin=" + pin;
        }
    }
}
