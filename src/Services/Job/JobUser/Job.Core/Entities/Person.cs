using Job.Core.Enums;

namespace Job.Core.Entities
{
    public class Person : BaseEntity
    {
        public User User { get; set; }
        public Guid UserId { get; set; }
        public string FatherName { get; set; }
        public string? UserPhoto { get; set; }
        public bool IsDriver { get; set; }
        public bool IsMarried { get; set; }
        public bool IsCitizen { get; set; }
        public Gender Gender { get; set; }
        public string? Adress { get; set; }
        public DateTime BirthDay { get; set; }
        public ICollection<Number> PhoneNumbers { get; set; }
    }
}