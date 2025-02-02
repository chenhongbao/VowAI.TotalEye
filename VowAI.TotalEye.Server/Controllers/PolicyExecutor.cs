using Microsoft.EntityFrameworkCore;
using VowAI.TotalEye.Server.Models;
using VowAI.TotalEye.ServerShared.Models;

namespace VowAI.TotalEye.Server.Controllers
{
    public class PolicyExecutor : IPolicyExecutor
    {
        private readonly IDbContextFactory<ServerDbContext> _dbFactory;

        public PolicyExecutor(IDbContextFactory<ServerDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<ClientControlPolicySet> ApplyCommandPolicy(string token, string commandOutput)
        {
            using (ServerDbContext context = _dbFactory.CreateDbContext())
            {
                UserInfoSession? session = await context.Sessions.FirstOrDefaultAsync<UserInfoSession>(session => session.Token == token);

                if (session == default)
                {
                    throw new ArgumentException($"Unknown token '{token}'.");
                }
                else
                {
                    session.CommandOutput = new UserCommandOutput
                    {
                        User = session.User,
                        CommandOutput = commandOutput
                    };

                    await context.SaveChangesAsync();

                   return session
                }
            }
        }

        public async Task<ClientControlPolicySet> ApplyHttpPolicy(string token, ClientHttpLogs logs)
        {
            throw new NotImplementedException();
        }

        public async Task<ClientControlPolicySet> ApplyScreenshotPolicy(string token, ImageItem image)
        {
            throw new NotImplementedException();
        }
    }
}
