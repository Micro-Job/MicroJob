using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary.Responses;

namespace Job.Business.Services.VacancyInformation
{
    public interface IVacancyInformation
    {
        Task<GetUserSavedVacanciesResponse> GetUserVacancyDataAsync(List<Guid> VacancyIds);
    }
}