using SharedLibrary.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Services.User
{
    public interface IUserInformationService
    {
        Task<GetUserDataResponse> GetUserDataAsync(Guid userId);

    }
}
