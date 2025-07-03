using SharedLibrary.Enums;

namespace Job.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Resume? Resume { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? Image { get; set; }
        public string Email { get; set; }
        public string MainPhoneNumber { get; set; }
        public JobStatus JobStatus { get; set; }

        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<SavedResume>? SavedResumes { get; set; }
        //public ICollection<CompanyResumeAccess>? CompanyResumeAccesses { get; set; }
    }
}