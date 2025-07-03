using JobCompany.Core.Entites.Base;

namespace JobCompany.Core.Entites
{
    public class Application : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid VacancyId { get; set; }
        public Vacancy Vacancy { get; set; }
        public Guid StatusId { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; }

        public Guid ResumeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
    }
}
