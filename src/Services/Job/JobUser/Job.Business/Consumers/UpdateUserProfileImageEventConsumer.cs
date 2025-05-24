using Job.Core.Entities;
using Job.DAL.Contexts;
using MassTransit;
using SharedLibrary.Dtos.FileDtos;
using SharedLibrary.Events;
using SharedLibrary.Exceptions;
using SharedLibrary.ExternalServices.FileService;
using SharedLibrary.Statics;

namespace Job.Business.Consumers;

public class UpdateUserProfileImageEventConsumer(JobDbContext _jobDbContext, IFileService _fileService) : IConsumer<UpdateUserProfileImageEvent>
{
    public async Task Consume(ConsumeContext<UpdateUserProfileImageEvent> context)
    {
        var user = await _jobDbContext.Users.FindAsync(context.Message.UserId) ?? throw new NotFoundException<User>();
        Console.WriteLine(user.Image);
        if (user.Image is not null)
        {
            Console.WriteLine("user image not nullllllllll");
            _fileService.DeleteFile(user.Image);
            user.Image = null;
        }

        if (context.Message.Base64Image is not null)
        {
            var bytes = Convert.FromBase64String(context.Message.Base64Image);

            FileDto fileResult = await _fileService.UploadAsync(FilePaths.image, context.Message.FileName!, bytes);

            user.Image = $"{fileResult.FilePath}/{fileResult.FileName}";
        }

        await _jobDbContext.SaveChangesAsync();
    }
}
