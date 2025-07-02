using Job.Business.Services.Position;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Attributes;
using SharedLibrary.Enums;

namespace Job.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[AuthorizeRole(UserRole.SimpleUser)]
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
    public async Task<IActionResult> GetSubPositions(Guid mainPositionId)
    {
        return Ok(await _positionService.GetSubPositionsAsync(mainPositionId));
    }
}