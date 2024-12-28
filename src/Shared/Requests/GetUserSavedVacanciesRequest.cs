namespace SharedLibrary.Requests
{
    public class GetUserSavedVacanciesRequest
    {
        public List<Guid> VacancyIds { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}