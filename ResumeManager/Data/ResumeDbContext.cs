


using Microsoft.EntityFrameworkCore;
using ResumeManager.Models;

namespace ResumeManager.Data
{
    public class ResumeDbContext : DbContext
    {
        public ResumeDbContext(DbContextOptions<ResumeDbContext> options) : base(options) { }

        public DbSet<Applicant> Applicants { get; set; }
        public DbSet<Experience> Experiences { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            modelBuilder.Entity<Experience>()
                .ToTable("Experience")  
                .HasOne(e => e.Applicant)
                .WithMany(a => a.Experiences)
                .HasForeignKey(e => e.ApplicantId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
