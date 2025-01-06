namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public class ApplicationInfoListDto
    {
        public Guid ApplicationId { get; set; }
        public string FullName { get; set; }
        public string Position { get; set; }
        public string ImageUrl { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}