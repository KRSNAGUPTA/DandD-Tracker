namespace DdTracker.Api.Dtos;

public record DashboardSummaryDto(
    int TotalContainers,
    int Safe,
    int AtRisk,
    int Accruing,
    int Closed,
    decimal TotalAccrued,
    string Currency);