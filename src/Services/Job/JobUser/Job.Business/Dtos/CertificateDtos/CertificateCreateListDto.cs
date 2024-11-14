using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.CertificateDtos
{
    public class CertificateCreateListDto
    {
        public ICollection<CertificateCreateDto> Certificates { get; set; }   
    }
}