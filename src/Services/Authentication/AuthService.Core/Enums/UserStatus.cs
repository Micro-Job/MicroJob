using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Core.Enums
{
    public enum UserStatus : byte
    {
        SimpleUser = 1,
        EmployeUser = 2,
        CompanyUser = 3,
    }
}
