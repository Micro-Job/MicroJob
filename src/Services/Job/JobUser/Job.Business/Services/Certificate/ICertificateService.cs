using Job.Business.Dtos.CertificateDtos;

namespace Job.Business.Services.Certificate
{
    public interface ICertificateService
    {
        Task<Core.Entities.Certificate> CreateCertificateAsync(CertificateCreateDto dto);
        Task<ICollection<Core.Entities.Certificate>> CreateBulkCertificateAsync(ICollection<CertificateCreateDto> dtos);
        Task UpdateCertificateAsync(CertificateUpdateDto dto);
        Task<ICollection<Core.Entities.Certificate>> UpdateBulkCertificateAsync(ICollection<CertificateUpdateDto> dtos);
    }
}