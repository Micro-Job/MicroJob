namespace Shared.Events
{
    public class VacancyApplicationEvent
    {
        public Guid UserId { get; set; }
        public Guid SenderId { get; set; }
        public Guid VacancyId { get; set; }
        public string Content { get; set; }
    }
}