using AuthService.Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Entities
{
    public class Company : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
    }
}
