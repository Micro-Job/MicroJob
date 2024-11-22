using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.VacancyInformation
{
    public class VacancyInformation(IRequestClient<GetUserSavedVacanciesRequest> _client) : IVacancyInformation
    {
        public async Task<GetUserSavedVacanciesResponse> GetUserSavedVacancyDataAsync(List<Guid> vacancyIds)
        {
            if (vacancyIds == null || !vacancyIds.Any())
            {
                return new GetUserSavedVacanciesResponse
                {
                    Vacancies = new List<VacancyResponse>()
                };
            }

            var response = await _client.GetResponse<GetUserSavedVacanciesResponse>(
                new GetUserSavedVacanciesRequest { VacancyIds = vacancyIds }
            );

            return response.Message;
        }
    }
}
