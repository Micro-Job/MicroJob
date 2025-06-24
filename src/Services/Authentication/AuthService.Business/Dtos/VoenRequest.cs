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
        public string type { get; set; }
        public string tin { get; set; }
    }
}
