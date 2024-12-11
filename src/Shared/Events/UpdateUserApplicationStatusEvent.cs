namespace Shared.Events
{
    public class UpdateUserApplicationStatusEvent
    {
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}