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
        public DbSet<SavedVacancy> SavedVacancies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ResumeSkill> ResumeSkills { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}