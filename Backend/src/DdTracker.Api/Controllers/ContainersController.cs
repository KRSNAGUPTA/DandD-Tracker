using DdTracker.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace DdTracker.Api.Controllers;

[ApiController]
[Route("api/containers")]
public class ContainersController : ControllerBase
{
    private readonly ContainerService _service;

    public ContainersController(ContainerService service) => _service = service;

    // GET /api/containers?risk=Accruing
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? risk)
        => Ok(await _service.GetAllAsync(risk));

    // GET /api/containers/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    // POST /api/containers/{id}/events
    [HttpPost("{id:int}/events")]
    public async Task<IActionResult> RecordEvent(int id, [FromBody] CreateEventRequest request)
    {
        try
        {
            var result = await _service.RecordEventAsync(id, request);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(new { error = ex.Message }); }
        catch (ArgumentException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpGet("{id:int}/projection")]
    public async Task<IActionResult> Projection(int id, [FromQuery] DateTime date)
    {
        var result = await _service.GetProjectionAsync(id, date);
        return result is null ? NotFound() : Ok(result);
    }
}
