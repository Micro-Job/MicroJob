namespace Job.Business.Dtos.FileDtos
{
    public record FileDto
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}