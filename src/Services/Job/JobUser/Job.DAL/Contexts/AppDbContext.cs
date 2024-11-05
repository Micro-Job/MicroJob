using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Job.DAL.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<ExtraInformation> ExtraInformations { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Number> Numbers { get; set; }
        public DbSet<Resume> Resumes { get; set; }
        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}

