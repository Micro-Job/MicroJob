namespace SharedLibrary.Requests
{
    public class GetAllCountriesRequest
    {
        public int Skip { get; set; } = 1;
        public int Take { get; set; } = 6;
    }
}