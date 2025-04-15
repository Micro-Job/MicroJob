using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.DAL.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.CreatedDate)
            .IsRequired();

        builder.HasMany(m => m.Translations)
            .WithOne(mt => mt.Message)
            .HasForeignKey(mt => mt.MessageId)
            .OnDelete(DeleteBehavior.Cascade);  // Mesaj silinsə, onun tərcümələri də silinsin

        builder.HasMany(m => m.VacancyMessages)
            .WithOne(vm => vm.Message)
            .HasForeignKey(vm => vm.MessageId)
            .OnDelete(DeleteBehavior.Cascade);  // Mesaj silinsə, onun vakansiya mesajları da silinsin
    }
}
