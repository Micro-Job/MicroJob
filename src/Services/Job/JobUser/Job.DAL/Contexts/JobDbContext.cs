using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Job.DAL.Contexts
{
    public class JobDbContext : DbContext
    {
        public JobDbContext(DbContextOptions<JobDbContext> options) : base(options) { }

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Number> Numbers { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ResumeSkill> ResumeSkills { get; set; }
        public DbSet<ResumeLink> ResumeLinks { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SavedResume> SavedResumes { get; set; }
        public DbSet<CompanyResumeAccess> CompanyResumeAccesses { get; set; }
        public DbSet<Position> Positions { get; set; }

        public DbSet<SkillTranslation> SkillTranslations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}