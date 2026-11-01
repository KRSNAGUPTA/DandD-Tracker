namespace DdTracker.Api.Dtos;

public record TierBreakdownDto(
    int FromDay,
    int? ToDay,
    int DaysInTier,
    decimal Rate,
    decimal Subtotal);

public record ChargeDto(
    string ChargeType,
    int DaysElapsed,
    int FreeDaysRemaining,
    decimal Amount,
    string Currency,
    IReadOnlyList<TierBreakdownDto> Breakdown);