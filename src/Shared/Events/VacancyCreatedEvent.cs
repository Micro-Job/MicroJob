namespace SharedLibrary.Events
{
    public class VacancyCreatedEvent
    {
        public Guid VacancyId { get; set; }
        public string Title {  get; set; }
        public List<Guid> SkillIds { get; set; }
        public Guid CreatedById { get; set; }
    }
}