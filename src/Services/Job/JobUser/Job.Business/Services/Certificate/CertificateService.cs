using Job.Business.Dtos.CertificateDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace Job.Business.Services.Certificate
{
    public class CertificateService(JobDbContext context, IFileService fileService) : ICertificateService
    {
        public async Task<ICollection<Core.Entities.Certificate>> CreateBulkCertificateAsync(ICollection<CertificateCreateDto> dtos)
        {
            var certificatesToAdd = new List<Core.Entities.Certificate>();

            foreach (var dto in dtos)
            {
                var certificate = await CreateCertificateAsync(dto);
                certificatesToAdd.Add(certificate);
            }

            return certificatesToAdd;
        }

        public async Task<Core.Entities.Certificate> CreateCertificateAsync(CertificateCreateDto dto)
        {
            FileDto fileResult = await fileService.UploadAsync(FilePaths.document, dto.CertificateFile);

            var certificate = new Core.Entities.Certificate
            {
                CertificateName = dto.CertificateName,
                CertificateFile = $"{fileResult.FilePath}/{fileResult.FileName}",
                GivenOrganization = dto.GivenOrganization
            };

            await context.Certificates.AddAsync(certificate);
            return certificate;
        }


        public async Task<ICollection<Core.Entities.Certificate>> UpdateBulkCertificateAsync(ICollection<CertificateUpdateDto> dtos)
        {
            Guid parsedId;

            var updatedCertificates = new List<Core.Entities.Certificate>();

            foreach (var dto in dtos)
            {
                await UpdateCertificateAsync(dto);

                parsedId = Guid.Parse(dto.Id);

                var certificate = await context.Certificates.FirstOrDefaultAsync(x => x.Id == parsedId);

                if (certificate != null) updatedCertificates.Add(certificate);
            }

            await context.SaveChangesAsync();

            return updatedCertificates;
        }

        public async Task UpdateCertificateAsync(CertificateUpdateDto dto)
        {
            var parsedId = Guid.Parse(dto.Id);

            var certificate = await context.Certificates.FirstOrDefaultAsync(x => x.Id == parsedId)
                ?? throw new NotFoundException<Core.Entities.Certificate>();

            fileService.DeleteFile(certificate.CertificateFile);

            FileDto fileResult = await fileService.UploadAsync(FilePaths.document, dto.CertificateFile);

            certificate.CertificateName = dto.CertificateName;
            certificate.GivenOrganization = dto.GivenOrganization;
            certificate.CertificateFile = $"{fileResult.FilePath}/{fileResult.FileName}";

            await context.SaveChangesAsync();
        }
    }
}
