namespace JobCompany.Business.Dtos.VacancyDtos
{
    public record VacancyListDtoForAppDto
    {
        public Guid VacancyId { get; set; }
        public string VacancyName { get; set; }
    }
}
