namespace SharedLibrary.Events
{
    public class VacancyCreatedEvent
    {
        public Guid SenderId { get; set; }
        public List<Guid> SkillIds { get; set; }
        public Guid InformationId { get; set; }
        public string? InformatioName { get; set; }

        public string SenderName { get; set; }
        public string? SenderImage { get; set; }
    }
}