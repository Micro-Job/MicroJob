namespace Job.Core.Entities
{
    public class User 
    {
        public Guid Id { get; set; }
        public ICollection<SavedVacancy>? SavedVacancies { get; set; }
    }
}