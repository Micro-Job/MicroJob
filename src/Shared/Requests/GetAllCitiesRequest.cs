namespace SharedLibrary.Requests
{
    public class GetAllCitiesRequest
    {
        public int Skip { get; set; }
        public int Take {  get; set; }
        public Guid? CountryId { get; set; }
    }
}