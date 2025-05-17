using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations;

public class ApplicationStepConfiguration : IEntityTypeConfiguration<ApplicationStep>
{
    public void Configure(EntityTypeBuilder<ApplicationStep> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.StatusColor)
               .IsRequired();
    }
}
