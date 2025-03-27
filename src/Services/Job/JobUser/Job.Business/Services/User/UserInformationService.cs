using Job.Business.Dtos.UserDtos;
using Job.Business.Exceptions.Common;
using Job.Business.Exceptions.UserExceptions;
using Job.DAL.Contexts;
using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Enums;
using SharedLibrary.Events;
using SharedLibrary.Helpers;
using SharedLibrary.HelperServices.Current;
using SharedLibrary.Requests;
using SharedLibrary.Responses;

namespace Job.Business.Services.User
{
    public class UserInformationService(IRequestClient<GetUserDataRequest> _client, JobDbContext _context, ICurrentUser _user , IPublishEndpoint _endPoint) : IUserInformationService
    {
        public async Task<GetUserDataResponse> GetUserDataAsync(Guid userId)
        {
            var response = await _client.GetResponse<GetUserDataResponse>(new GetUserDataRequest { UserId = userId });
            return response.Message;
        }

        public async Task<JobStatus> UpdateUserJobStatusAsync(UserJobStatusUpdateDto dto)
        {
            var user =
                await _context.Users.FirstOrDefaultAsync(u => u.Id == _user.UserGuid)
                ?? throw new NotFoundUserException(MessageHelper.GetMessage("NOTFOUNDEXCEPTION_USER"));

            user.JobStatus = dto.JobStatus;

            await _context.SaveChangesAsync();

            //await _endPoint.Publish(new UpdateUserJobStatusEvent {  UserId = user.Id, JobStatus = user.JobStatus });
            return user.JobStatus;
        }
    }
}