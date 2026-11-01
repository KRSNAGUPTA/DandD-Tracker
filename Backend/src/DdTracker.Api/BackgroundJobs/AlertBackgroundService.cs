using DdTracker.Api.Application;
using DdTracker.Api.Data;
using DdTracker.Api.Domain.Entities;
using DdTracker.Api.Domain.Enums;
using DdTracker.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DdTracker.Api.BackgroundJobs;

public class AlertBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _config;
    private readonly ILogger<AlertBackgroundService> _logger;

    public AlertBackgroundService(
        IServiceScopeFactory scopeFactory,
        IConfiguration config,
        ILogger<AlertBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var intervalMinutes = _config.GetValue<int?>("AlertJob:IntervalMinutes") ?? 5;
        var warningThreshold = _config.GetValue<int?>("AlertJob:WarningDaysThreshold") ?? 2;

        _logger.LogInformation(
            "AlertBackgroundService started. Interval={Interval}min, WarningThreshold={Threshold}d",
            intervalMinutes, warningThreshold);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await RunOnceAsync(warningThreshold, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Alert job run failed.");
            }

            await Task.Delay(TimeSpan.FromMinutes(intervalMinutes), stoppingToken);
        }
    }

    private async Task RunOnceAsync(int warningThreshold, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var containerService = scope.ServiceProvider.GetRequiredService<ContainerService>();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var containers = await containerService.GetAllAsync();

        foreach (var c in containers)
        {
            if (c.Status == "Closed") continue;

            // Decide severity based on current charge and free days
            string? severity = null;
            if (c.CurrentCharge > 0m)
                severity = "Critical";
            else if (c.FreeDaysRemaining <= warningThreshold)
                severity = "Warning";

            if (severity is null) continue;

            // Fetch the container detail to know which charge type has the accrual / urgency
            var detail = await containerService.GetByIdAsync(c.Id);
            if (detail is null) continue;

            ChargeDto? activeCharge =
                (detail.Demurrage?.Amount ?? 0m) > 0m ? detail.Demurrage :
                (detail.Detention?.Amount ?? 0m) > 0m ? detail.Detention :
                detail.Demurrage ?? detail.Detention;

            var chargeType = activeCharge?.ChargeType ?? "Demurrage";

            // Dedup: one unacknowledged alert per (container, severity, chargeType)
            var alreadyExists = await db.Alerts.AnyAsync(a =>
                a.ContainerId == c.Id &&
                !a.Acknowledged &&
                a.Severity.ToString() == severity &&
                a.ChargeType.ToString() == chargeType, ct);

            if (alreadyExists) continue;

            var message = severity == "Critical"
                ? $"{c.ContainerNo}: ${c.CurrentCharge:F2} accrued on {chargeType} ({c.DaysElapsed} days elapsed)."
                : $"{c.ContainerNo}: {c.FreeDaysRemaining} free days remaining on {chargeType}.";

            db.Alerts.Add(new Alert
            {
                ContainerId = c.Id,
                Severity = Enum.Parse<AlertSeverity>(severity),
                ChargeType = Enum.Parse<ChargeType>(chargeType),
                Message = message,
                Acknowledged = false,
                CreatedAt = DateTime.UtcNow
            });
        }

        var saved = await db.SaveChangesAsync(ct);
        if (saved > 0)
            _logger.LogInformation("Alert job inserted {Count} new alert(s).", saved);
    }
}