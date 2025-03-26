
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.Business.Exceptions.Common;
using JobCompany.Business.Exceptions.VacancyExceptions;
using JobCompany.Core.Entites;
using JobCompany.DAL.Contexts;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;

namespace JobCompany.Business.Services.ManageService;

public class ManageService(JobCompanyDbContext _context,ICurrentUser _user) : IManageService
{
    private bool HasRole(string roleName)
    {
        if (Enum.TryParse<UserRole>(roleName, out var role))
        {
            return _user.UserRole == (byte)role;
        }
        return false; 
    }

    public async Task VacancyAcceptAsync(VacancyAcceptDto dto)
    {
        if (HasRole("SimpleUser") || HasRole("EmployeeUser") || HasRole("CompanyUser"))
        {
            throw new DontHavePermissionException(MessageHelper.GetMessage("NO_PERMISSION"));
        }
        var vacancyGuid = Guid.Parse(dto.VacancyId);
        var vacancy = await _context.Vacancies.Where(v => v.Id == vacancyGuid).FirstOrDefaultAsync()
            ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));
        vacancy.VacancyStatus = SharedLibrary.Enums.VacancyStatus.Active;
        await _context.SaveChangesAsync();
    }

    public async Task VacancyRejectAsync(VacancyStatusUpdateDto dto)
    {
        if (HasRole("SimpleUser") || HasRole("EmployeeUser") || HasRole("CompanyUser"))
        {
            throw new DontHavePermissionException(MessageHelper.GetMessage("NO_PERMISSION"));
        }
        var vacancyGuid = Guid.Parse(dto.VacancyId);
        var vacancyCommenGuid = Guid.Parse(dto.VacancyCommentId);
        var vacancy = await _context.Vacancies.Where(v => v.Id == vacancyGuid).FirstOrDefaultAsync()
            ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

        vacancy.VacancyCommentId = vacancyCommenGuid;
        vacancy.VacancyStatus = SharedLibrary.Enums.VacancyStatus.Reject;
        await _context.SaveChangesAsync();
    }

    public async Task ToggleBlockVacancyStatusAsync(VacancyStatusUpdateDto dto)
    {
        if (HasRole("SimpleUser") || HasRole("EmployeeUser") || HasRole("CompanyUser"))
        {
            throw new DontHavePermissionException(MessageHelper.GetMessage("NO_PERMISSION"));
        }
        var vacancyGuid = Guid.Parse(dto.VacancyId);
        var vacancy = await _context.Vacancies
            .Where(v => v.Id == vacancyGuid)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException<Vacancy>(MessageHelper.GetMessage("NOT_FOUND"));

        if (vacancy.VacancyStatus != SharedLibrary.Enums.VacancyStatus.Block)
        {
            vacancy.VacancyStatus = SharedLibrary.Enums.VacancyStatus.Block;
            vacancy.VacancyCommentId = Guid.Parse(dto.VacancyCommentId);
        }
        else
        {
            vacancy.VacancyStatus = SharedLibrary.Enums.VacancyStatus.Deactive;

            if (vacancy.VacancyCommentId != null)
            {
                vacancy.VacancyCommentId = null;
            }
        }

        await _context.SaveChangesAsync();
    }
}
