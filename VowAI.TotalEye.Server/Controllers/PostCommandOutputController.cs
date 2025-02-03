using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCommandOutputController : ControllerBase
    {
        private readonly IDbContextFactory<ServerDbContext> _dbFactory;

        public PostCommandOutputController(IDbContextFactory<ServerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public static string GetControllerName()
        {
            return nameof(PostCommandOutputController).Replace("Controller", "");
        }

        public static string GetControllerUrl()
        {
            return $"api/{GetControllerName()}";
        }

        [HttpPost]
        public async Task<ActionResult<ClientControlPolicySet>> Post([FromForm] string token, [FromForm] string payload)
        {
            try
            {
                using (ServerDbContext context = _dbFactory.CreateDbContext())
                {
                    UserInfoSession? session = await context.Sessions
                        .Include(s => s.Request)
                        .ThenInclude(r => r.User)
                        .ThenInclude(u => u.Policies)
                        .FirstOrDefaultAsync<UserInfoSession>(session => session.Token == token);

                    if (session == null || session == default)
                    {
                        throw new ArgumentException($"Unknown token '{token}'.");
                    }
                    else
                    {
                        SessionCommandOutput cmd = new SessionCommandOutput
                        {
                            Session = session,
                            CommandOutput = payload,
                        };

                        context.Add(cmd);
                        await context.SaveChangesAsync();

                        SessionCommandOutput? storedCmd = await context.CommandOutputs
                            .Include(cmd => cmd.Session)
                            .FirstOrDefaultAsync<SessionCommandOutput>(cmd => cmd.Session == null ? false : cmd.Session.SessionId == session.SessionId);

                        session.CommandOutput = storedCmd;
                        context.Entry(session).State = EntityState.Modified;

                        await context.SaveChangesAsync();

                        if (session.Request?.User != null)
                        {
                            return Redirect(GetControlPolicyController.GetControllerUrl(session.Request.User.UserId, session.Request.User.Pin));
                        }
                        else
                        {
                            throw new InvalidDataException("Missing user property in session.");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                exception.WriteString<PostScreenshotController>();
                return BadRequest(exception);
            }
        }
    }
}
