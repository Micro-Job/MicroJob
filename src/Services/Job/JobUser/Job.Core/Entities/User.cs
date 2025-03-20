namespace Job.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Resume? Resume { get; set; }

        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<SavedResume>? SavedResumes { get; set; }
    }
}