
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Helpers;

namespace JobCompany.Business.Services.ManageService;

public class ManageService(JobCompanyDbContext _context) : IManageService
{
    public async Task VacancyAcceptAsync(VacancyAcceptDto dto)
    {
        var vacancyGuid = Guid.Parse(dto.VacancyId);
        var vacancy = await _context.Vacancies.Where(v => v.Id == vacancyGuid).FirstOrDefaultAsync()
            ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));
        vacancy.VacancyStatus = SharedLibrary.Enums.VacancyStatus.Active;
        await _context.SaveChangesAsync();
    }

    public async Task VacancyRejectAsync(VacancyStatusUpdateDto dto)
    {
        var vacancyGuid = Guid.Parse(dto.VacancyId);
        var vacancyCommenGuid = Guid.Parse(dto.VacancyCommentId);
        var vacancy = await _context.Vacancies.Where(v => v.Id == vacancyGuid).FirstOrDefaultAsync()
            ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

        vacancy.VacancyCommentId = vacancyCommenGuid;
        vacancy.VacancyStatus = SharedLibrary.Enums.VacancyStatus.Reject;
        await _context.SaveChangesAsync();
    }

    public async Task VacancyBlockAsync(VacancyStatusUpdateDto dto)
    {
        var vacancyGuid = Guid.Parse(dto.VacancyId);
        var vacancyCommenGuid = Guid.Parse(dto.VacancyCommentId);
        var vacancy = await _context.Vacancies.Where(v => v.Id == vacancyGuid).FirstOrDefaultAsync()
            ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

        vacancy.VacancyCommentId = vacancyCommenGuid;
        vacancy.VacancyStatus = SharedLibrary.Enums.VacancyStatus.Block;
        await _context.SaveChangesAsync();
    }
}
