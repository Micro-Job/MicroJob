﻿using SharedLibrary.Responses;

namespace Job.Business.Services.User
{
    public interface IUserInformationService
    {
        Task<GetUserDataResponse> GetUserDataAsync(Guid userId);

    }
}