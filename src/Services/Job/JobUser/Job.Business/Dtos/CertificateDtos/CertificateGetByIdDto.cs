namespace Job.Business.Dtos.CertificateDtos
{
    public class CertificateGetByIdDto
    {
        public Guid Id { get; set; }
        public string CertificateName { get; set; }
        public string GivenOrganization { get; set; }
        public string CertificateFile { get; set; }
    }
}