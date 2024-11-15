namespace Job.Business.Dtos.ExperienceDtos
{
    public record ExperienceCreateListDto
    {
        public ICollection<ExperienceCreateDto> Experiences { get; set; }
    }
}