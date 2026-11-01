using DdTracker.Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace DdTracker.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;
    public DashboardController(DashboardService service) => _service = service;

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
        => Ok(await _service.GetSummaryAsync());
}