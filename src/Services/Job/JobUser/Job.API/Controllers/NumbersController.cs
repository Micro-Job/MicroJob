using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Job.Business.Dtos.NumberDtos;
using Job.Business.Services.Number;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NumbersController : ControllerBase
    {
        readonly INumberService _service;

        public NumbersController(INumberService service)
        {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Post([FromForm] NumberCreateDto dto)
        {
            await _service.CreateAsync(dto);
            return StatusCode(StatusCodes.Status201Created);
        }

    }
}