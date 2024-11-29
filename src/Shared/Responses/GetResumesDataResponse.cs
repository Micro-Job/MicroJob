
namespace Shared.Responses
{
    public class GetResumesDataResponse
    {
        public List<GetResumeDataResponse> Users { get; set; } = new List<GetResumeDataResponse>();
    }

    public class GetResumeDataResponse
    {
        public Guid UserId { get; set; }
        public string Position { get; set; }
    }
}