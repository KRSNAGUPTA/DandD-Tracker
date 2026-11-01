using DdTracker.Api.Data;
using DdTracker.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DdTracker.Api.Application;

public class AlertService
{
    private readonly AppDbContext _db;

    public AlertService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<AlertDto>> GetAsync(bool? acknowledged = null)
    {
        var q = _db.Alerts
            .Include(a => a.Container)
            .AsNoTracking()
            .AsQueryable();

        if (acknowledged.HasValue)
            q = q.Where(a => a.Acknowledged == acknowledged.Value);

        return await q
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AlertDto(
                a.Id,
                a.ContainerId,
                a.Container.ContainerNo,
                a.Severity.ToString(),
                a.ChargeType.ToString(),
                a.Message,
                a.Acknowledged,
                a.CreatedAt))
            .ToListAsync();
    }

    public async Task<bool> AcknowledgeAsync(int alertId)
    {
        var alert = await _db.Alerts.FirstOrDefaultAsync(a => a.Id == alertId);
        if (alert is null) return false;

        alert.Acknowledged = true;
        await _db.SaveChangesAsync();
        return true;
    }
}