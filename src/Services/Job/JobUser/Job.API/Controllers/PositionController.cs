using Job.Business.Services.Position;
using Microsoft.AspNetCore.Mvc;

namespace Job.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PositionController(PositionService _positionService) : ControllerBase
{
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllPositions()
    {
        return Ok(await _positionService.GetAllPositionsAsync());
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetMainPositions()
    {
        return Ok(await _positionService.GetMainPositionsAsync());
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetSubPositions(string mainPositionId)
    {
        return Ok(await _positionService.GetSubPositionsAsync(mainPositionId));
    }
}