using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{

    public class UserExamConfiguration : IEntityTypeConfiguration<UserExam>
    {
        public void Configure(EntityTypeBuilder<UserExam> builder)
        {
            builder.HasKey(ua => ua.Id);

            builder.HasOne(ua => ua.User)
                   .WithMany(u => u.UserExams)
                   .HasForeignKey(ua => ua.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}