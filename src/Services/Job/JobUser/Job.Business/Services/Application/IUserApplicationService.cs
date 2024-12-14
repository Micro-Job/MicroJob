using SharedLibrary.Responses;

using System.Linq;
using System.Threading.Tasks;

using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Services.Application
{
    public interface IUserApplicationService
    {
        Task<GetUserApplicationsResponse> GetUserApplicationsAsync(int skip, int take);
        Task CreateUserApplicationAsync(string vacancyId);
    }
}