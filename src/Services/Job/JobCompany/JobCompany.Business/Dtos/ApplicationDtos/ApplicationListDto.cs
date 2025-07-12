using SharedLibrary.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCompany.Business.Dtos.ApplicationDtos
{
    public class ApplicationListDto
    {
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid ResumeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string VacancyName { get; set; }
        public Guid VacancyId { get; set; }
        public StatusEnum Status { get; set; }
        public Guid? ExamId { get; set; }
        public float? ExamPercent { get; set; }
    }
}
