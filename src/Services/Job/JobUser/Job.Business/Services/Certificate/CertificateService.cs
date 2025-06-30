using Job.Business.Dtos.CertificateDtos;
using Job.DAL.Contexts;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace Job.Business.Services.Certificate;

public class CertificateService(JobDbContext context, IFileService fileService)
{
    public async Task<ICollection<Core.Entities.Certificate>> CreateBulkCertificateAsync(ICollection<CertificateCreateDto> dtos)
    {
        var certificatesToAdd = new List<Core.Entities.Certificate>();

        foreach (var dto in dtos)
        {
            var certificate = await CreateCertificateFromDto(dto);
            certificatesToAdd.Add(certificate);
        }

        await context.Certificates.AddRangeAsync(certificatesToAdd);
        return certificatesToAdd;
    }

    public async Task<ICollection<Core.Entities.Certificate>> UpdateBulkCertificateAsync(ICollection<CertificateUpdateDto> dtos, ICollection<Core.Entities.Certificate> existingCertificates)
    {
        var incomingIds = dtos
            .Select(dto => Guid.TryParse(dto.Id, out var id) ? id : Guid.Empty)
            .Where(id => id != Guid.Empty)
            .ToHashSet();

        var certificatesToUpdate = existingCertificates
            .Where(c => incomingIds.Contains(c.Id))
            .ToList();

        foreach (var cert in certificatesToUpdate) // Mövcud sertifikatları yeniləyir
        {
            var dto = dtos.FirstOrDefault(d => Guid.TryParse(d.Id, out var id) && id == cert.Id);
            if (dto == null) continue;
            await UpdateCertificateFromDto(cert, dto);
        }

        var toRemove = existingCertificates      // Mövcud sertifikatlardan silinməsi lazım olanları tapır
            .Where(c => !incomingIds.Contains(c.Id))
            .ToList();

        if (toRemove.Count != 0)
        {
            foreach (var cert in toRemove)
                fileService.DeleteFile(cert.CertificateFile);

            context.Certificates.RemoveRange(toRemove);
        }

        var createDtos = dtos                   // Yeni sertifikatları əlavə edir
            .Where(d => !Guid.TryParse(d.Id, out var id) || id == Guid.Empty)
            .Select(d => new CertificateCreateDto
            {
                CertificateName = d.CertificateName,
                GivenOrganization = d.GivenOrganization,
                CertificateFile = d.CertificateFile
            }).ToList();

        if (createDtos.Count != 0)
            certificatesToUpdate.AddRange(await CreateBulkCertificateAsync(createDtos));

        return certificatesToUpdate;
    }

    public ICollection<Core.Entities.Certificate> DeleteAllCertificates(ICollection<Core.Entities.Certificate> existingCertificates)
    {
        if (existingCertificates.Count > 0)
        {
            foreach (var cert in existingCertificates)
            {
                fileService.DeleteFile(cert.CertificateFile);
            }
            context.Certificates.RemoveRange(existingCertificates);
        }

        return [];
    }


    private async Task<Core.Entities.Certificate> CreateCertificateFromDto(CertificateCreateDto dto)
    {
        FileDto fileResult = await fileService.UploadAsync(FilePaths.document, dto.CertificateFile);

        var certificate = new Core.Entities.Certificate
        {
            CertificateName = dto.CertificateName,
            CertificateFile = $"{fileResult.FilePath}/{fileResult.FileName}",
            GivenOrganization = dto.GivenOrganization,
        };

        return certificate;
    }

    private async Task UpdateCertificateFromDto(Core.Entities.Certificate certificate, CertificateUpdateDto dto)
    {
        fileService.DeleteFile(certificate.CertificateFile);
        var upload = await fileService.UploadAsync(FilePaths.document, dto.CertificateFile);

        certificate.CertificateName = dto.CertificateName;
        certificate.GivenOrganization = dto.GivenOrganization;
        certificate.CertificateFile = $"{upload.FilePath}/{upload.FileName}";
    }
}
