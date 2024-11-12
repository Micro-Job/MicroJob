using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Services.Vacancy
{
    public interface IVacancyService
    {
        Task ToggleSaveVacancyAsync(string vacancyId);
        Task GetAllSavedVacancyAsync();
    }
}
