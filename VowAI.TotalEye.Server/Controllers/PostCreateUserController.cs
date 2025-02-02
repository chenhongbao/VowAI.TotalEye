using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostCreateUserController : ControllerBase
    {
        private readonly IDbContextFactory<ServerDbContext> _dbFactory;

        public PostCreateUserController(IDbContextFactory<ServerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Post([FromForm] ClientUserProfile userProfile)
        {
            try
            {
                using (ServerDbContext context = _dbFactory.CreateDbContext())
                {
                    User user = new User
                    {
                        No = userProfile.No,
                        Name = userProfile.Name,
                        Password = userProfile.Password,
                        Pin = userProfile.Pin,
                        Email = userProfile.Email,
                        Description = userProfile.Description,
                        Role = userProfile.Role,
                        Group = userProfile.Group,
                        Organization = userProfile.Organization,
                    };

                    context.Add(user);
                    await context.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception exception)
            {
                exception.WriteString<PostBindUserPolicyController>();
                return BadRequest(exception);
            }
        }
    }
}
