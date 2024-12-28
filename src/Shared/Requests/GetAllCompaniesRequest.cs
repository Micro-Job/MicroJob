namespace SharedLibrary.Requests
{
    public class GetAllCompaniesRequest
    {
        public string? SearchTerm { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
    }
}