using DdTracker.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DdTracker.Api.Controllers;

[ApiController]
[Route("api/tariffs")]
public class TariffsController : ControllerBase
{
    private readonly AppDbContext _db;
    public TariffsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var rows = await _db.TariffTiers
            .Include(t => t.ContainerType)
            .AsNoTracking()
            .OrderBy(t => t.ContainerType.Code)
            .ThenBy(t => t.ChargeType)
            .ThenBy(t => t.FromDay)
            .Select(t => new
            {
                t.Id,
                ContainerType = t.ContainerType.Code,
                ChargeType = t.ChargeType.ToString(),
                t.FromDay,
                t.ToDay,
                t.RatePerDay,
                t.Currency
            })
            .ToListAsync();

        return Ok(rows);
    }
}