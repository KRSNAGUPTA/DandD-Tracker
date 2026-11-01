using DdTracker.Api.Data;
using DdTracker.Api.Domain.Entities;
using DdTracker.Api.Domain.Enums;
using DdTracker.Api.Domain.Services;
using DdTracker.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DdTracker.Api.Application;

public class ContainerService
{
    private readonly AppDbContext _db;
    private readonly IClock _clock;

    public ContainerService(AppDbContext db, IClock clock)
    {
        _db = db;
        _clock = clock;
    }

    // GET /api/containers
    public async Task<IReadOnlyList<ContainerListDto>> GetAllAsync(string? riskFilter = null)
    {
        var asOf = _clock.UtcNow;

        var containers = await _db.Containers
            .Include(c => c.ContainerType)
            .Include(c => c.DestinationPort)
            .Include(c => c.Events)
            .AsNoTracking()
            .ToListAsync();

        var allTiers = await _db.TariffTiers
            .AsNoTracking()
            .ToListAsync();

        // Group tariffs by container type once — O(1) lookup per container.
        var tiersByType = allTiers
            .GroupBy(t => t.ContainerTypeId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var rows = new List<ContainerListDto>(containers.Count);

        foreach (var c in containers)
        {
            var typeTiers = tiersByType.GetValueOrDefault(c.ContainerTypeId, new List<TariffTier>());

            var demurrage = ComputeCharge(c, ChargeType.Demurrage, asOf,
                typeTiers.Where(t => t.ChargeType == ChargeType.Demurrage).ToList());

            var detention = ComputeCharge(c, ChargeType.Detention, asOf,
                typeTiers.Where(t => t.ChargeType == ChargeType.Detention).ToList());

            ChargeDto? showClock;
            if (c.Status == ContainerStatus.Closed)
                showClock = detention;
            else if ((demurrage?.Amount ?? 0m) > 0m)
                showClock = demurrage;
            else if ((detention?.Amount ?? 0m) > 0m)
                showClock = detention;
            else
                showClock = c.Status == ContainerStatus.AtPort ? demurrage : detention;

            var daysElapsed = showClock?.DaysElapsed ?? 0;
            var freeRemaining = showClock?.FreeDaysRemaining ?? 0;
            var amount = (demurrage?.Amount ?? 0m) + (detention?.Amount ?? 0m);
            var currency = showClock?.Currency ?? "USD";

            var risk = ClassifyRisk(c, demurrage, detention, freeRemaining);

            rows.Add(new ContainerListDto(
                c.Id,
                c.ContainerNo,
                c.BookingRef,
                c.ContainerType.Code,
                c.DestinationPort.Name,
                c.Status.ToString(),
                daysElapsed,
                freeRemaining,
                amount,
                currency,
                risk));
        }

        // Optional risk filter.
        if (!string.IsNullOrWhiteSpace(riskFilter))
        {
            rows = rows
                .Where(r => r.Risk.Equals(riskFilter, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Sort: Accruing → AtRisk → Safe → Closed, then by urgency (fewest free days first).
        var order = new Dictionary<string, int>
        {
            ["Accruing"] = 0,
            ["AtRisk"] = 1,
            ["Safe"] = 2,
            ["Closed"] = 3
        };

        return rows
            .OrderBy(r => order.GetValueOrDefault(r.Risk, 99))
            .ThenByDescending(r => r.CurrentCharge)
            .ThenBy(r => r.FreeDaysRemaining)
            .ThenByDescending(r => r.DaysElapsed)
            .ToList();
    }

    // GET /api/containers/{id}
    public async Task<ContainerDetailDto?> GetByIdAsync(int id)
    {
        var c = await _db.Containers
            .Include(x => x.ContainerType)
            .Include(x => x.DestinationPort)
            .Include(x => x.Events)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (c is null) return null;

        var asOf = _clock.UtcNow;

        var tiers = await _db.TariffTiers
            .AsNoTracking()
            .Where(t => t.ContainerTypeId == c.ContainerTypeId)
            .ToListAsync();

        var demurrage = ComputeCharge(c, ChargeType.Demurrage, asOf,
            tiers.Where(t => t.ChargeType == ChargeType.Demurrage).ToList());

        var detention = ComputeCharge(c, ChargeType.Detention, asOf,
            tiers.Where(t => t.ChargeType == ChargeType.Detention).ToList());

        return new ContainerDetailDto(
            c.Id,
            c.ContainerNo,
            c.BookingRef,
            c.ContainerType.Code,
            c.DestinationPort.Name,
            c.Status.ToString(),
            c.CreatedAt,
            c.Events
                .OrderBy(e => e.OccurredAt)
                .Select(e => new ContainerEventDto(e.EventType.ToString(), e.OccurredAt, e.Note))
                .ToList(),
            demurrage,
            detention);
    }

public async Task<ProjectionDto?> GetProjectionAsync(int containerId, DateTime projectedAsOf)
{
    var c = await _db.Containers
        .Include(x => x.ContainerType)
        .Include(x => x.Events)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == containerId);

    if (c is null) return null;

    var tiers = await _db.TariffTiers.AsNoTracking()
        .Where(t => t.ContainerTypeId == c.ContainerTypeId)
        .ToListAsync();

    var demurrage = ComputeCharge(c, ChargeType.Demurrage, projectedAsOf,
        tiers.Where(t => t.ChargeType == ChargeType.Demurrage).ToList());

    var detention = ComputeCharge(c, ChargeType.Detention, projectedAsOf,
        tiers.Where(t => t.ChargeType == ChargeType.Detention).ToList());

    var total = (demurrage?.Amount ?? 0m) + (detention?.Amount ?? 0m);

    return new ProjectionDto(
        c.Id,
        c.ContainerNo,
        projectedAsOf,
        demurrage,
        detention,
        total,
        demurrage?.Currency ?? detention?.Currency ?? "USD");
}

    // POST /api/containers/{id}/events
    public async Task<ContainerDetailDto> RecordEventAsync(int containerId, CreateEventRequest req)
    {
        var c = await _db.Containers
            .Include(x => x.Events)
            .FirstOrDefaultAsync(x => x.Id == containerId)
            ?? throw new KeyNotFoundException($"Container {containerId} not found.");

        if (!Enum.TryParse<EventType>(req.EventType, ignoreCase: true, out var eventType))
            throw new ArgumentException($"Unknown event type '{req.EventType}'.");

        ValidateEvent(c, eventType, req.OccurredAt);

        c.Events.Add(new ContainerEvent
        {
            ContainerId = c.Id,
            EventType = eventType,
            OccurredAt = req.OccurredAt,
            Note = req.Note
        });

        c.Status = eventType switch
        {
            EventType.GatedOut => ContainerStatus.OutForDelivery,
            EventType.EmptyReturned => ContainerStatus.Closed,
            _ => c.Status
        };

        await _db.SaveChangesAsync();

        // Re-query to get the populated DTO (with computed charges).
        return (await GetByIdAsync(containerId))!;
    }

    // Helpers
    private static void ValidateEvent(Container c, EventType type, DateTime occurredAt)
    {
        var existing = c.Events.Select(e => e.EventType).ToHashSet();

        switch (type)
        {
            case EventType.Discharged:
                if (existing.Contains(EventType.Discharged))
                    throw new ArgumentException("Container already has a Discharged event.");
                if (c.Events.Any())
                    throw new ArgumentException("Discharged must be the first event.");
                break;

            case EventType.GatedOut:
                if (existing.Contains(EventType.GatedOut))
                    throw new ArgumentException("Container already has a GatedOut event.");
                var dis = c.Events.FirstOrDefault(e => e.EventType == EventType.Discharged)
                    ?? throw new ArgumentException("GatedOut requires a prior Discharged event.");
                if (occurredAt < dis.OccurredAt)
                    throw new ArgumentException("GatedOut cannot be before Discharged.");
                break;

            case EventType.EmptyReturned:
                if (existing.Contains(EventType.EmptyReturned))
                    throw new ArgumentException("Container already has an EmptyReturned event.");
                var go = c.Events.FirstOrDefault(e => e.EventType == EventType.GatedOut)
                    ?? throw new ArgumentException("EmptyReturned requires a prior GatedOut event.");
                if (occurredAt < go.OccurredAt)
                    throw new ArgumentException("EmptyReturned cannot be before GatedOut.");
                break;
        }
    }

    private static ChargeDto? ComputeCharge(
        Container c,
        ChargeType chargeType,
        DateTime asOf,
        IReadOnlyList<TariffTier> tiers)
    {
        if (tiers.Count == 0) return null;

        // Demurrage: Discharged → GatedOut.   Detention: GatedOut → EmptyReturned.
        var startType = chargeType == ChargeType.Demurrage ? EventType.Discharged : EventType.GatedOut;
        var endType = chargeType == ChargeType.Demurrage ? EventType.GatedOut : EventType.EmptyReturned;

        var start = c.Events.FirstOrDefault(e => e.EventType == startType)?.OccurredAt;
        if (start is null) return null;   // clock never started

        var end = c.Events.FirstOrDefault(e => e.EventType == endType)?.OccurredAt;

        var result = ChargeCalculator.Calculate(chargeType, start.Value, end, asOf, tiers);

        return new ChargeDto(
            result.ChargeType.ToString(),
            result.DaysElapsed,
            result.FreeDaysRemaining,
            result.Amount,
            result.Currency,
            result.Breakdown
                .Select(b => new TierBreakdownDto(b.FromDay, b.ToDay, b.DaysInTier, b.Rate, b.Subtotal))
                .ToList());
    }

    private static string ClassifyRisk(
        Container c,
        ChargeDto? demurrage,
        ChargeDto? detention,
        int freeRemaining)
    {
        if (c.Status == ContainerStatus.Closed) return "Closed";

        // "Accruing" if either clock has a positive amount — money is actively owed.
        if ((demurrage?.Amount ?? 0m) > 0m || (detention?.Amount ?? 0m) > 0m)
            return "Accruing";

        if (freeRemaining <= 2) return "AtRisk";

        return "Safe";
    }
}