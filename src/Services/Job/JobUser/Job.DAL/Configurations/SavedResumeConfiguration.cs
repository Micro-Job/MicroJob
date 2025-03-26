using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.DAL.Configurations
{
    public class SavedResumeConfiguration : IEntityTypeConfiguration<SavedResume>
    {
        public void Configure(EntityTypeBuilder<SavedResume> builder)
        {
            builder.HasOne(x => x.Resume)
                .WithMany(x => x.SavedResumes)
                .HasForeignKey(x => x.ResumeId);

            builder.HasOne(x => x.CompanyUser)
                .WithMany(x => x.SavedResumes)
                .HasForeignKey(x => x.CompanyUserId);
        }
    }
}
