using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;
using VowAI.TotalEye.Tools;

namespace VowAI.TotalEye.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostControlPolicyController : ControllerBase
    {
        private readonly IDbContextFactory<ServerDbContext> _dbFactory;

        public PostControlPolicyController(IDbContextFactory<ServerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> Post([FromForm] ClientControlPolicy controlPolicy)
        {
            try
            {
                using (ServerDbContext context = _dbFactory.CreateDbContext())
                {
                    ControlPolicy policy = new ControlPolicy
                    {
                        Description = controlPolicy.Description,
                        Tag = controlPolicy.Tag,
                        FilterWords = controlPolicy.FilterWords,
                        FilterCondition = controlPolicy.FilterCondition,
                        Action = controlPolicy.Action,
                        ActionDescription = controlPolicy.ActionDescription,
                    };

                    context.Add(policy);
                    await context.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception exception)
            {
                exception.WriteString<PostControlPolicyController>();
                return BadRequest(exception);
            }
        }
    }
}
