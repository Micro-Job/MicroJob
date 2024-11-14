using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.DAL.Contexts
{
    public class JobCompanyDbContext : DbContext
    {
        public JobCompanyDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyNumber> CompanyNumbers { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Vacancy> Vacancies { get; set; }
        public DbSet<VacancyTest> VacancyTests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
