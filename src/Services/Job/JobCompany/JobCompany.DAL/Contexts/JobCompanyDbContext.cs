using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobCompany.DAL.Contexts
{
    public class JobCompanyDbContext : DbContext
    {
        public JobCompanyDbContext(DbContextOptions<JobCompanyDbContext> options) : base(options)
        {
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyNumber> CompanyNumbers { get; set; }
        public DbSet<VacancyNumber> VacancyNumbers { get; set; }
        public DbSet<VacancySkill> VacancySkills { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<SavedVacancy> SavedVacancies { get; set; }

        public DbSet<SkillTranslation> SkillTranslations { get; set; }
        public DbSet<CategoryTranslation> CategoryTranslations { get; set; }
        public DbSet<CountryTranslation> CountryTranslations { get; set; }
        public DbSet<CityTranslation> CityTranslations { get; set; }

        public DbSet<UserExam> UserExams { get; set; }
        public DbSet<VacancyComment> VacancyComments { get; set; }
        public DbSet<VacancyCommentTranslation> VacancyCommentTranslations { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<VacancyMessage> VacancyMessages { get; set; }
        public DbSet<MessageTranslation> MessageTranslations { get; set; }
        public DbSet<ApplicationStep> ApplicationSteps { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}