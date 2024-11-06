using Job.Business.Dtos.CertificateDtos;

namespace Job.Business.Services.Certificate
{
    public interface ICertificateService
    {
        Task CreateCertificateAsync(CertificateCreateDto dto);
        Task<ICollection<Core.Entities.Certificate>> CreateBulkCertificateAsync(ICollection<CertificateCreateDto> dtos);
        Task UpdateCertificateAsync(string id, CertificateUpdateDto dto);
    }
}