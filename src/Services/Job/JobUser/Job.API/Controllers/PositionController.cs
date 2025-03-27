using Job.Business.Services.Position;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionController(IPositionService _positionService) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllPositions()
    {
        var positions = await _positionService.GetAllPositionsAsync();
        return Ok(positions);
    }
}