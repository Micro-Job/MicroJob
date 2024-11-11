using Job.Business.Dtos.CertificateDtos;
using Job.Business.Dtos.FileDtos;
using Job.Business.Exceptions.Common;
using Job.Business.ExternalServices;
using Job.Business.Statics;
using Job.DAL.Contexts;

namespace Job.Business.Services.Certificate
{
    public class CertificateService(JobDbContext context, IFileService fileService) : ICertificateService
    {
        readonly JobDbContext _context = context;
        readonly IFileService _fileService = fileService;

        public async Task<ICollection<Core.Entities.Certificate>> CreateBulkCertificateAsync(ICollection<CertificateCreateDto> dtos)
        {
            var certificatesToAdd = dtos.Select(async dto =>
            {
                FileDto fileResult = await _fileService.UploadAsync(FilePaths.document, dto.CertificateFile);

                return new Core.Entities.Certificate
                {
                    CertificateName = dto.CertificateName,
                    CertificateFile = $"{fileResult.FilePath}/{fileResult.FileName}",
                    GivenOrganization = dto.GivenOrganization
                };
            }).ToList();

            var certificates = await Task.WhenAll(certificatesToAdd);
            await _context.Certificates.AddRangeAsync(certificates);

            return [.. certificates];
        }

        public async Task CreateCertificateAsync(CertificateCreateDto dto)
        {
            FileDto fileResult = await _fileService.UploadAsync(FilePaths.document, dto.CertificateFile);
            var certificate = new Core.Entities.Certificate
            {
                CertificateName = dto.CertificateName,
                CertificateFile = $"{fileResult.FilePath}/{fileResult.FileName}",
                GivenOrganization = dto.GivenOrganization
            };
            await _context.Certificates.AddAsync(certificate);
        }

        public async Task UpdateCertificateAsync(string id, CertificateUpdateDto dto)
        {
            var certificateId = Guid.Parse(id);
            var certificate = await _context.Certificates.FindAsync(certificateId)
                ?? throw new NotFoundException<Core.Entities.Certificate>();
            certificate.CertificateName = dto.CertificateName;
            certificate.GivenOrganization = dto.GivenOrganization;
        }
    }
}