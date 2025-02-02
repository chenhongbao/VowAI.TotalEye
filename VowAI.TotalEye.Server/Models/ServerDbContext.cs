using Microsoft.EntityFrameworkCore;

namespace VowAI.TotalEye.Server.Models
{
    public class ServerDbContext : DbContext
    {
        public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserControlPolicy> Policies { get; set; }
        public DbSet<UserInfoSession> Sessions { get; set; }
        public DbSet<ImageItem> Screenshots { get; set; }

    }
}
