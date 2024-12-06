using JobCompany.Core.Entites;
using JobCompany.Core.Entites.ExamEntities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobCompany.DAL.Contexts
{
    public class JobCompanyDbContext : DbContext
    {
        public JobCompanyDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyNumber> CompanyNumbers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<VacancyTest> VacancyTests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Questions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}