using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobCompany.DAL.Configurations
{
    public class CompanyNumberConfiguration : IEntityTypeConfiguration<CompanyNumber>
    {
        public void Configure(EntityTypeBuilder<CompanyNumber> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(cn => cn.Number)
                   .HasMaxLength(32);

            builder.HasOne(cn => cn.Company)
                   .WithMany(v => v.CompanyNumbers)
                   .HasForeignKey(cn => cn.CompanyId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}