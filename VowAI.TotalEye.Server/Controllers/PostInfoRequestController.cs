using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostInfoRequestController : ControllerBase
    {
        private readonly IDbContextFactory<ServerDbContext> _dbFactory;

        public PostInfoRequestController(IDbContextFactory<ServerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Post([FromForm] int userId, [FromForm] ClientInfoRequest request)
        {
            try
            {
                using (ServerDbContext context = _dbFactory.CreateDbContext())
                {
                    User? user = await context.Users.Include(u => u.Requests).FirstOrDefaultAsync(u => u.UserId == userId);

                    if (user == null)
                    {
                         return NotFound();
                    }
                    else
                    {
                        UserInfoRequest userRequest = new UserInfoRequest()
                        {
                            Name = request.Name,
                            Description = request.Description,
                            ReplyUrl = request.ReplyUrl,
                            Status = "pending",
                            User = user,
                        };

                        context.Add(userRequest);
                        await context.SaveChangesAsync();

                        if (user.Requests == null)
                        {
                            user.Requests = new List<UserInfoRequest>();
                        }

                        UserInfoRequest? storedRequest = await context.Requests.Include(r => r.User).FirstOrDefaultAsync(r => r.User.UserId == user.UserId);

                        if (storedRequest == null)
                        {
                            throw new InvalidDataException("Lost new request in database.");
                        }

                        user.Requests.Add(storedRequest);
                        context.Entry(user).State = EntityState.Modified;

                        await context.SaveChangesAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.WriteString<PostInfoRequestController>();
                return BadRequest(ex);
            }
        }
    }
}
