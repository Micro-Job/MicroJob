using SharedLibrary.Enums;

namespace Shared.Responses
{
    public class GetApplicationDetailResponse
    {
        public string VacancyId { get; set; }
        public string VacancyName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public string Location { get; set; }
        public WorkType? WorkType { get; set; }
        public WorkStyle? WorkStyle { get; set; }
        public DateTime startDate { get; set; }
        public bool IsSaved { get; set; }
        public ICollection<string> CompanyStatuses { get; set; }
        public string ApplicationStatus { get; set; }
    }
}