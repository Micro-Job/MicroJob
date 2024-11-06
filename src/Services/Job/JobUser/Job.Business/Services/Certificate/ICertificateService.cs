using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.CertificateDtos;

namespace Job.Business.Services.Certificate
{
    public interface ICertificateService
    {
        Task CreateCertificateAsync(CertificateCreateDto dto);
        Task UpdateCertificateAsync(string id , CertificateUpdateDto dto);
    }
}