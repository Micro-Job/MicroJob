using AuthService.Core.Entities.Base;

namespace AuthService.Core.Entities
{
    public class UserStatus : BaseEntity
    {
        public string StatusName { get; set; } = null!;

        public virtual ICollection<User> Users { get; set; } = [];
    }
}