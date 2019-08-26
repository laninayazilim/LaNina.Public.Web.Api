using Microsoft.EntityFrameworkCore;

namespace Lanina.Public.Web.Api.Models
{
    public class LaNinaApplicantDbContext : DbContext
    {

        public LaNinaApplicantDbContext(DbContextOptions<LaNinaApplicantDbContext> options) 
            : base(options) {}

        public DbSet<Applicant> Applicants { get; set; }

        public DbSet<Interview> Interviews { get; set; }

        public DbSet<InterviewDate> InterviewDates { get; set; }

        public DbSet<Flag> Flags { get; set; }

        public DbSet<ApplyToken> ApplyTokens { get; set; }
    }
}
