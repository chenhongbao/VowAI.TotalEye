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
        public DbSet<UserInfoRequest> Requests { get; set; }
        public DbSet<UserInfoSession> Sessions { get; set; }
        public DbSet<SessionScreenshot> Screenshots { get; set; }
        public DbSet<SessionCommandOutput> CommandOutputs { get; set; }
        public DbSet<SessionHttpLog> HttpLogs { get; set; }
    }
}
