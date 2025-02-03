using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetInfoRequestController : ControllerBase
    {
        private readonly IDbContextFactory<ServerDbContext> _factory;

        public GetInfoRequestController(IDbContextFactory<ServerDbContext> factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public async Task<ActionResult<ClientInfoRequest>> Get(int userId)
        {
            try
            {
                using (ServerDbContext context = _factory.CreateDbContext())
                {
                    User? user = await context.Users
                        .Include(u => u.Requests)
                        .FirstOrDefaultAsync(u => u.UserId == userId);

                    UserInfoRequest? request = user?.Requests?.FirstOrDefault(r => r.Status?.ToLower() == "pending");

                    if (request == null || user == null)
                    {
                        return Ok(new ClientInfoRequest());
                    }
                    else
                    {
                        string token = Guid.NewGuid().ToString();

                        UserInfoSession session = new UserInfoSession
                        {
                            Token = token,
                            Request = request,
                        };

                        context.Add(session);
                        await context.SaveChangesAsync();

                        if (user?.Sessions == null)
                        {
                            user.Sessions = new List<UserInfoSession>();
                        }

                        UserInfoSession? storedSession = await context.Sessions.FirstOrDefaultAsync(s => s.Token == token);
                        
                        if (storedSession == null)
                        {
                            throw new InvalidDataException("Lost new session in database.");
                        }

                        user.Sessions.Add(storedSession);
                        context.Entry(user.Sessions).State = EntityState.Modified;

                        await context.SaveChangesAsync();

                        request.Status = "replying";
                        context.Entry(request).State = EntityState.Modified;

                        await context.SaveChangesAsync();

                        return Ok(new ClientInfoRequest
                        {
                            Name = request.Name ?? "",
                            Description = request.Description ?? "",
                            Token = token,
                            ReplyUrl = request.ReplyUrl ?? "",
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                exception.WriteString<GetInfoRequestController>();
                return BadRequest(exception);
            }
        }
    }
}
