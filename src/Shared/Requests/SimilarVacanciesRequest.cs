namespace Shared.Requests
{
    public class SimilarVacanciesRequest
    {
        public string VacancyId { get; set; }
        public int Skip { get; set; } = 1;
        public int Take { get; set; } = 6;
    }
}