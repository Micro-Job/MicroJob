namespace JobCompany.Core.Entites
{
    public class Company
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? CompanyInformation { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? WebLink { get; set; }
        public ICollection<CompanyNumber>? CompanyNumbers { get; set; }
        public ICollection<Vacancy>? Vacancies { get; set; }
    }
}