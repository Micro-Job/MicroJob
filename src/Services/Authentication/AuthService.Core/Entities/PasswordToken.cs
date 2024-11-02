using AuthService.Core.Entities.Base;

namespace AuthService.Core.Entities
{
    public class PasswordToken : BaseEntity<Guid>
    {
        public string Token { get; set; }

        public DateTime ExpireTime { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
