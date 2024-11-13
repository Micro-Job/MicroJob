using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(32);
            builder
                .HasMany(s => s.ResumeSkills)
                .WithOne(rs => rs.Skill)
                .HasForeignKey(rs => rs.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}