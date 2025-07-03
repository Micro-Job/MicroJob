using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Business.Dtos
{
    public class VoenRequest
    {
        public string? middleName { get; set; }
        public required string type { get; set; }
        public required string tin { get; set; }
    }
}
