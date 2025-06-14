using Job.Business.Dtos.CertificateDtos;

namespace Job.Business.Services.Certificate;

public interface ICertificateService
{
    Task<ICollection<Core.Entities.Certificate>> CreateBulkCertificateAsync(ICollection<CertificateCreateDto> dtos);
    
    Task<ICollection<Core.Entities.Certificate>> UpdateBulkCertificateAsync(
        ICollection<CertificateUpdateDto> dtos, ICollection<Core.Entities.Certificate> existingCertificates);

    ICollection<Core.Entities.Certificate> DeleteAllCertificates(ICollection<Core.Entities.Certificate> existingCertificates);
}
