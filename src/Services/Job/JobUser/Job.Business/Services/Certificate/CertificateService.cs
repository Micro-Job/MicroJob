using Job.Business.Dtos.CertificateDtos;
using Job.Business.Exceptions.Common;
using Job.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace Job.Business.Services.Certificate
{
    public class CertificateService(JobDbContext context, IFileService fileService)
        : ICertificateService
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
            FileDto fileResult = await fileService.UploadAsync(
                FilePaths.document,
                dto.CertificateFile
            );

            var certificate = new Core.Entities.Certificate
            {
                CertificateName = dto.CertificateName,
                CertificateFile = $"{fileResult.FilePath}/{fileResult.FileName}",
                GivenOrganization = dto.GivenOrganization,
            };

            await context.Certificates.AddAsync(certificate);
            return certificate;
        }

        public async Task<ICollection<Core.Entities.Certificate>> UpdateBulkCertificateAsync(ICollection<CertificateUpdateDto> dtos)
        {
            var updatedCertificates = new List<Core.Entities.Certificate>(); // Nəticə olaraq qaytarılacaq sertifikat siyahısı
            var newCertificateDtos = new List<CertificateCreateDto>(); // Yeni əlavə olunacaq sertifikatlar üçün yaradılacaq DTO siyahısı

            foreach (var dto in dtos)
            {
                if (Guid.TryParse(dto.Id, out var parsedId) && parsedId != Guid.Empty) // Sertifikat ID-si varsa və düzgün formatda parse edilirsə
                {
                    var certificate = await context.Certificates.FirstOrDefaultAsync(x => x.Id == parsedId)
                        ?? throw new NotFoundException<Core.Entities.Certificate>();

                    fileService.DeleteFile(certificate.CertificateFile); // Mövcud sertifikatın faylını silirik

                    var fileResult = await fileService.UploadAsync(FilePaths.document, dto.CertificateFile); // Yeni faylı yükləyirik

                    // Sertifikatın məlumatlarını güncəlləyirik
                    certificate.CertificateName = dto.CertificateName; 
                    certificate.GivenOrganization = dto.GivenOrganization;
                    certificate.CertificateFile = $"{fileResult.FilePath}/{fileResult.FileName}";

                    updatedCertificates.Add(certificate);  // Yenilənmiş sertifikatı siyahıya əlavə edirik
                }
                else // Əgər sertifikat ID-si yoxdursa, yeni sertifikat yaradırıq
                {
                    newCertificateDtos.Add(new CertificateCreateDto
                    {
                        CertificateName = dto.CertificateName,
                        GivenOrganization = dto.GivenOrganization,
                        CertificateFile = dto.CertificateFile
                    });
                }
            }

            if (newCertificateDtos.Count > 0)  // Əgər yeni sertifikatlar varsa, onları əlavə edirik
            {
                var newlyCreated = await CreateBulkCertificateAsync(newCertificateDtos);
                updatedCertificates.AddRange(newlyCreated);
            }

            return updatedCertificates; // Yenilənmiş sertifikatları qaytarırıq
        }



        public async Task UpdateCertificateAsync(CertificateUpdateDto dto)
        {
            var parsedId = Guid.Parse(dto.Id);

            var certificate =
                await context.Certificates.FirstOrDefaultAsync(x => x.Id == parsedId)
                ?? throw new NotFoundException<Core.Entities.Certificate>();

            fileService.DeleteFile(certificate.CertificateFile);

            FileDto fileResult = await fileService.UploadAsync(
                FilePaths.document,
                dto.CertificateFile
            );

            certificate.CertificateName = dto.CertificateName;
            certificate.GivenOrganization = dto.GivenOrganization;
            certificate.CertificateFile = $"{fileResult.FilePath}/{fileResult.FileName}";

            await context.SaveChangesAsync();
        }
    }
}
