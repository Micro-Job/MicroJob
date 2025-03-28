using SharedLibrary.Enums;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public record ApplicationGetByIdDto
    {
        public Guid VacancyId { get; set; }
        public string? VacancyImage { get; set; }
        public string VacancyTitle { get; set; }
        public string CompanyName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Description {  get; set; }
        public StatusEnum? Status {  get; set; }
        public string? StatusColor { get; set; }
        public List<string>? Steps { get; set; }
    }
}