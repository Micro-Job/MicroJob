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
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.HasOne(x => x.ExtraInformation)
                .WithMany(x => x.Languages)
                .HasForeignKey(x=>x.ExtraInformationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
