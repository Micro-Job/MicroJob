namespace SharedLibrary.Events;

public class VacancyRejectedEvent
{
    public Guid? SenderId { get; set; }
    public Guid InformationId { get; set; }
    public Guid ReceiverId { get; set; }
    public string? InformationName { get; set; }
}
