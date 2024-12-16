using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job.Business.Dtos.CertificateDtos
{
    public class CertificateUpdateListDto
    {
        public ICollection<CertificateUpdateDto> Certificates { get; set; }  
    }
}