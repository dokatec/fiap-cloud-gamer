using Fcg.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Data
{
    public class FcgDbContext : DbContext
    {
        public FcgDbContext(DbContextOptions<FcgDbContext> options) : base(options) { }


        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Promotion> Promotions { get; set; }
        public virtual DbSet<UserGaming> UserGamings { get; set; }

    }
}