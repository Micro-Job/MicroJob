using JobPayment.Business.Dtos.PacketDtos;
using JobPayment.Business.Services.PacketSer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPayment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PacketController(IPacketService _packetService) : ControllerBase
    {
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllPackets()
        {
            return Ok(await _packetService.GetAllPacketsAsync());
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreatePacket(CreatePacketDto dto)
        {
            await _packetService.CreatePacketAsync(dto);
            return Ok();
        }
    }
}
