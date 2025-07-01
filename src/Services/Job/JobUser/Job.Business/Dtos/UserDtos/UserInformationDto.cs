using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Job.Business.Dtos.UserDtos
{
    public class UserInformationDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MainPhoneNumber { get; set; }
        public string? Image { get; set; }
        public UserRole UserRole { get; set; }
        public JobStatus JobStatus { get; set; }
    }
}
