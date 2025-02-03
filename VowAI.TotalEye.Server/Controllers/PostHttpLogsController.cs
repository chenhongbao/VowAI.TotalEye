using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostHttpLogsController : ControllerBase
    {
        private readonly IDbContextFactory<ServerDbContext> _dbFactory;

        public PostHttpLogsController(IDbContextFactory<ServerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public static string GetControllerName()
        {
            return nameof(PostHttpLogsController).Replace("Controller", "");
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
                        SessionHttpLog log = new SessionHttpLog
                        {
                            Log = payload,
                            Session = session,
                        };

                        context.Add(log);
                        await context.SaveChangesAsync();

                        SessionHttpLog? storedLogs = await context.HttpLogs
                            .Include(cmd => cmd.Session)
                            .FirstOrDefaultAsync<SessionHttpLog>(cmd => cmd.Session == null ? false : cmd.Session.SessionId == session.SessionId);

                        session.HttpLogs = storedLogs;
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
                exception.WriteString<PostHttpLogsController>();
                return BadRequest(exception);
            }
        }
    }
}
