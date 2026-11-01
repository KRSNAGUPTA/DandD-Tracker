using DdTracker.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace DdTracker.Api.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly AlertService _service;
    public AlertsController(AlertService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool? acknowledged)
        => Ok(await _service.GetAsync(acknowledged));

    [HttpPost("{id:int}/ack")]
    public async Task<IActionResult> Acknowledge(int id)
    {
        var ok = await _service.AcknowledgeAsync(id);
        return ok ? Ok(new { acknowledged = true }) : NotFound();
    }
}