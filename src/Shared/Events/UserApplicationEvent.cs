namespace Shared.Events
{
    public class UserApplicationEvent
    {
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }
    }
}