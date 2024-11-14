using AuthService.Core.Entities.Base;

namespace AuthService.Core.Entities
{
    public class LoginLog : BaseEntity
    {
        public DateTime Date { get; set; }

        public Guid UserId { get; set; }

        public bool IsSucceed { get; set; }

        public string? IP { get; set; }

        public virtual User? User { get; set; }
    }

}