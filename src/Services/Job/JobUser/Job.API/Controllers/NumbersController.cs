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

        [HttpPost("CreateNumber")]
        public async Task<IActionResult> Post([FromForm] NumberCreateDto numberCreateDto)
        {
            await _service.CreateAsync(numberCreateDto);
            return Ok();
        }

        [HttpPut("UpdateNumber")]
        public async Task<IActionResult> Put(string id, [FromForm] NumberUpdateDto numberUpdateDto)
        {
            await _service.UpdateAsync(id, numberUpdateDto);
            return Ok();
        }
    }
}