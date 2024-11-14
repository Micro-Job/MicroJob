using AuthService.Core.Entities.Base;

namespace AuthService.Core.Entities
{
    public class Company : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}