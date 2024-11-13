namespace Job.Business.Dtos.SkillDtos
{
    public record GetAllSkillDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}