using JobCompany.Core.Entites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace JobCompany.DAL.Configurations;

public class VacancyMessageConfiguration : IEntityTypeConfiguration<VacancyMessage>
{
    public void Configure(EntityTypeBuilder<VacancyMessage> builder)
    {
        builder.HasKey(vm => vm.Id);

        builder.HasOne(vm => vm.Vacancy)
            .WithMany(v => v.VacancyMessages)
            .HasForeignKey(vm => vm.VacancyId)
            .OnDelete(DeleteBehavior.Cascade); // Vakansiya silinsə, VacancyMessages də silinsin

        builder.HasOne(vm => vm.Message)
            .WithMany(m => m.VacancyMessages)
            .HasForeignKey(vm => vm.MessageId)
            .OnDelete(DeleteBehavior.Cascade); // Mesaj silinsə VacancyMessages də silinsin
    }
}


