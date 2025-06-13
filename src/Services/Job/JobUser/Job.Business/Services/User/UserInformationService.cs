using Job.Business.Dtos.UserDtos;
using Job.Business.Exceptions.UserExceptions;
using Job.DAL.Contexts;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.User;

public class UserInformationService(IRequestClient<GetUserDataRequest> _client, JobDbContext _context, ICurrentUser _user, IPublishEndpoint _endPoint) : IUserInformationService
{
    public async Task<GetUserDataResponse> GetUserDataAsync(Guid userId)
    {
        var response = await _client.GetResponse<GetUserDataResponse>(new GetUserDataRequest { UserId = userId });
        return response.Message;
    }

    public async Task<JobStatus> UpdateUserJobStatusAsync(UserJobStatusUpdateDto dto)
    {
        var affectedRows = await _context.Users
            .Where(u => u.Id == _user.UserGuid)
            .ExecuteUpdateAsync(builder => builder
            .SetProperty(u => u.JobStatus, _ => dto.JobStatus));

        if (affectedRows == 0) throw new NotFoundUserException();

        await _endPoint.Publish(new UpdateUserJobStatusEvent { UserId = _user.UserGuid!.Value, JobStatus = dto.JobStatus });
        return dto.JobStatus;
    }
}