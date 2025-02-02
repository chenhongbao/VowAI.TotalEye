using Microsoft.EntityFrameworkCore;

namespace VowAI.TotalEye.Server.Models
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ControlPolicy> Policies { get; set; }
        public DbSet<UserInfoSession> Sessions { get; set; }
        public DbSet<UserScreenshot> Screenshots { get; set; }
        public DbSet<UserCommandOutput> CommandOutputs { get; set; }
        public DbSet<UserHttpLog> HttpLogs { get; set; }
    }
}
