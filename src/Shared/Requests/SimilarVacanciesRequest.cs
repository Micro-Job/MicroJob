namespace Shared.Requests
{
    public class SimilarVacanciesRequest
    {
        public string VacancyId { get; set; }
        public int Skip { get; set; } 
        public int Take { get; set; } 
    }
}