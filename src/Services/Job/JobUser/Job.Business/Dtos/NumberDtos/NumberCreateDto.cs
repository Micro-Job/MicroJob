namespace Job.Business.Dtos.NumberDtos
{
    public record NumberCreateDto
    {
        public string ResumeId { get; set; }
        public string PhoneNumber { get; set; }
    }
}