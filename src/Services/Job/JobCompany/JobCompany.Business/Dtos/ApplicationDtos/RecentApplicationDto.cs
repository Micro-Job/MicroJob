namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public record RecentApplicationDto
    {
        public string UserId {  get; set; }
        public string VacancyId {  get; set; }
        public string StatusId {  get; set; }
    }
}