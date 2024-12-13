namespace Job.Business.Services.Application
{
    public interface IUserApplicationService
    {
        Task CreateUserApplicationAsync(string vacancyId);
    }
}