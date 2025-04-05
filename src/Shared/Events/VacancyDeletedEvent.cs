namespace SharedLibrary.Events;

public class VacancyDeletedEvent
{
    public Guid SenderId { get; set; }
    public Guid InformationId { get; set; }
    public List<Guid> UserIds { get; set; }
    public string? InformationName { get; set; }
}
