using Job.Business.Dtos.CertificateDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;

namespace Job.Business.Services.Certificate
{
    public class CertificateService : ICertificateService
    {
        readonly JobDbContext _context;

        public CertificateService(JobDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Core.Entities.Certificate>> CreateBulkCertificateAsync(ICollection<CertificateCreateDto> dtos)
        {
            var certificatesToAdd = dtos.Select(dto => new Core.Entities.Certificate
            {
                CertificateName = dto.CertificateName,
                CertificateFile = dto.CertificateFile,
                GivenOrganization = dto.GivenOrganization
            }).ToList();

            await _context.Certificates.AddRangeAsync(certificatesToAdd);

            return certificatesToAdd;
        }

        public async Task CreateCertificateAsync(CertificateCreateDto dto)
        {
            var certificate = new Core.Entities.Certificate
            {
                CertificateName = dto.CertificateName,
                CertificateFile = dto.CertificateFile,
                GivenOrganization = dto.GivenOrganization
            };
            await _context.Certificates.AddAsync(certificate);
        }

        public async Task UpdateCertificateAsync(string id, CertificateUpdateDto dto)
        {
            var certificateId = Guid.Parse(id);
            var certificate = await _context.Certificates.FindAsync(certificateId);
            if (certificate is null) throw new NotFoundException<Core.Entities.Certificate>();

            certificate.CertificateName = dto.CertificateName;
            certificate.GivenOrganization = dto.GivenOrganization;
        }
    }
}