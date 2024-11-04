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
    public class ExtraInformationConfiguration : IEntityTypeConfiguration<ExtraInformation>
    {
        public void Configure(EntityTypeBuilder<ExtraInformation> builder)
        {
            builder.HasOne(x => x.Resume)
                .WithMany(x => x.ExtraInformations)
                .HasForeignKey(x => x.ResumeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
