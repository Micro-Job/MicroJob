using Job.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Job.DAL.Configurations
{
    public class ResumeSkillConfiguration : IEntityTypeConfiguration<ResumeSkill>
    {
        public void Configure(EntityTypeBuilder<ResumeSkill> builder)
        {
            builder.HasKey(rs => new { rs.ResumeId, rs.SkillId });
        }
    }
}