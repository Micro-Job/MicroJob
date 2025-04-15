using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.DAL.Configurations;

public class MessageTranslationConfiguration : IEntityTypeConfiguration<MessageTranslation>
{
    public void Configure(EntityTypeBuilder<MessageTranslation> builder)
    {
        builder.HasKey(mt => mt.Id);

        builder.Property(mt => mt.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(mt => mt.Language)
            .IsRequired();
    }
}


