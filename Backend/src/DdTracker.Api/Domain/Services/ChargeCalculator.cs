using DdTracker.Api.Domain.Entities;
using DdTracker.Api.Domain.Enums;

namespace DdTracker.Api.Domain.Services;

public record ChargeResult(
    ChargeType ChargeType,
    int DaysElapsed,
    int FreeDaysRemaining,
    decimal Amount,
    string Currency,
    IReadOnlyList<TierBreakdown> Breakdown);

public record TierBreakdown(int FromDay, int? ToDay, int DaysInTier, decimal Rate, decimal Subtotal);

public static class ChargeCalculator
{
    public static ChargeResult Calculate(
        ChargeType type,
        DateTime start,
        DateTime? end,
        DateTime asOf,
        IReadOnlyList<TariffTier> tiers)
    {
        if (tiers.Count == 0)
            throw new ArgumentException("At least one tariff tier is required.", nameof(tiers)); // nameof return string of variable: nameof(krishna)->"krishna"

        var endTime = end ?? asOf;
        if (endTime.Date < start.Date)
            throw new ArgumentException("End date cannot be less than start date.");

        var days = (endTime.Date - start.Date).Days + 1;

        var breakDown = new List<TierBreakdown>();
        decimal total = 0;

        foreach (var tier in tiers.OrderBy(t => t.FromDay))
        {
            var tierEnd = tier.ToDay ?? int.MaxValue;
            var overlapStart = Math.Max(1, tier.FromDay);
            var overlapEnd = Math.Min(days, tierEnd);
            var daysInTier = Math.Max(0, overlapEnd - overlapStart + 1);

            var subtotal = daysInTier * tier.RatePerDay;
            total += subtotal;

            breakDown.Add(new TierBreakdown(
                tier.FromDay,
                tier.ToDay,
                daysInTier,
                tier.RatePerDay,
                subtotal));
        }

        var freeDays = tiers
            .Where(t => t.RatePerDay == 0)
            .Select(t => t.ToDay ?? 0)
            .DefaultIfEmpty(0)
            .Max();

        return new ChargeResult(
            type,
            days,
            Math.Max(0, freeDays - days),
            total,
            tiers[0].Currency,
            breakDown);
    }
}