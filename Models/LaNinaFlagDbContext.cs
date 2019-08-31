using Microsoft.EntityFrameworkCore;

namespace Lanina.Public.Web.Api.Models
{
    public class LaNinaFlagDbContext : DbContext
    {

        public LaNinaFlagDbContext(DbContextOptions<LaNinaFlagDbContext> options)
            : base(options) { }

        public DbSet<Flag> Flags { get; set; }
    }
}
