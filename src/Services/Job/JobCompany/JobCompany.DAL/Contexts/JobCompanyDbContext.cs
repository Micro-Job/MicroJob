using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace JobCompany.DAL.Contexts
{
    public class JobCompanyDbContext : DbContext
    {
        public JobCompanyDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Company> Companies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}