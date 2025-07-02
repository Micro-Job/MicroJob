using JobPayment.Business.Dtos.PacketDtos;
using JobPayment.Business.Services.PacketServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace JobPayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacketController(PacketService _packetService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllPackets()
        {
            return Ok(await _packetService.GetAllPacketsAsync());
        }

        [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin)]
        [HttpPost("[action]")]
        public async Task<IActionResult> CreatePacket(CreatePacketDto dto)
        {
            await _packetService.CreatePacketAsync(dto);
            return Ok();
        }

        [AuthorizeRole(UserRole.SuperAdmin, UserRole.Admin)]
        [HttpPut("[action]")]
        public async Task<IActionResult> UpdatePacket(UpdatePacketDto dto)
        {
            await _packetService.UpdatePacketAsync(dto);
            return Ok();
        }
    }
}
