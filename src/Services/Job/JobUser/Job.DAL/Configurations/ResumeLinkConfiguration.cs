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
    public class ResumeLinkConfiguration : IEntityTypeConfiguration<ResumeLink>
    {
        public void Configure(EntityTypeBuilder<ResumeLink> builder)
        {
            builder.Property(x => x.Url).IsRequired(true).HasMaxLength(128);

            builder.HasOne(x => x.Resume)
                .WithMany(x => x.ResumeLinks)
                .HasForeignKey(x => x.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
