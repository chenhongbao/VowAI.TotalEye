using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostBindUserPolicyController : ControllerBase
    {
        private readonly IDbContextFactory<ServerDbContext> _dbFactory;

        public PostBindUserPolicyController(IDbContextFactory<ServerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Post([FromForm] int userId, [FromForm] int policyId)
        {
            try
            {
                using (ServerDbContext context = _dbFactory.CreateDbContext())
                {
                    User? user = await context.Users.Include(u => u.Policies).FirstOrDefaultAsync(u => u.UserId == userId);
                    ControlPolicy? policy = await context.Policies.FirstOrDefaultAsync(p => p.PolicyId == policyId);
                    
                    if (user == null || policy == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        if (user.Policies == null)
                        {
                            user.Policies = new List<ControlPolicy>();
                        }

                        user.Policies.Add(policy);
                        context.Entry(user.Policies).State = EntityState.Modified;

                        await context.SaveChangesAsync();

                        return true;
                    }
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
