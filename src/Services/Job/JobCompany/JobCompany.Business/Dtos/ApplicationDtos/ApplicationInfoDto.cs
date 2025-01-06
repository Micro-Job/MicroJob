namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public class ApplicationInfoDto
    {
        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}