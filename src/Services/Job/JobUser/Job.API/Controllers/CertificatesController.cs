using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.CertificateDtos;
using Job.Business.Services.Certificate;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CertificatesController : ControllerBase
    {
        readonly ICertificateService _service;

        public CertificatesController(ICertificateService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCertificate([FromForm] CertificateCreateDto dto)
        {
            await _service.CreateCertificateAsync(dto);
            return Ok();
        }

        [HttpPut("[action]/{id}")]
        public async Task<IActionResult> UpdateCertificate(string id, [FromForm] CertificateUpdateDto dto)
        {
            await _service.UpdateCertificateAsync(id, dto);
            return Ok();
        }
    }
}