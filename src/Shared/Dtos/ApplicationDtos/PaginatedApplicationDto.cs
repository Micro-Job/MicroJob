namespace SharedLibrary.Dtos.ApplicationDtos;

public class PaginatedApplicationDto
{
    public List<ApplicationDto> Applications { get; set; }
    public int TotalCount { get; set; }
}