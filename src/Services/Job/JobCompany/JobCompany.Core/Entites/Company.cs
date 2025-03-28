namespace JobCompany.Core.Entites
{
    public class Company
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyInformation { get; set; } 
        public string? CompanyLocation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? WebLink { get; set; }
        public int? EmployeeCount { get; set; }
        public string? CompanyLogo { get; set; }
        public string? Email { get; set; }
        public bool IsCompany { get; set; }

        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }

        public Guid? CountryId { get; set; }
        public Country? Country { get; set; }

        public Guid? CityId { get; set; }
        public City? City { get; set; }

        public ICollection<CompanyNumber>? CompanyNumbers { get; set; }
        public ICollection<Vacancy>? Vacancies { get; set; }
        public ICollection<Status> Statuses { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<Exam> Exams { get; set; }
    }
}