using AuthService.Business.Services.CurrentUser;
using JobCompany.Business.Dtos.NumberDtos;
using JobCompany.Business.Dtos.VacancyDtos;
using JobCompany.DAL.Contexts;
using SharedLibrary.ExternalServices.FileService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Services.Vacancy
{
    public class VacancyService : IVacancyService
    {
        private readonly JobCompanyDbContext _context;
        private readonly ICurrentUser _currentUser;
        private readonly Guid _userGuid;
        private readonly IFileService _fileService;

        public VacancyService(JobCompanyDbContext context,ICurrentUser currentUser,IFileService fileService)
        {
            _context = context;
            _currentUser = currentUser;
            _fileService = fileService;
            _userGuid = Guid.Parse(_currentUser.UserId);
        }

        public Task CreateVacancyAsync(CreateVacancyDto vacancyDto, ICollection<CreateNumberDto>? numberDto)
        {
            throw new Exception();
        }
    }
}
