namespace DdTracker.Api.Dtos;

public record ContainerListDto(
    int Id,
    string ContainerNo,
    string BookingRef,
    string Type,
    string DestinationPort,
    string Status,
    int DaysElapsed,
    int FreeDaysRemaining,
    decimal CurrentCharge,
    string Currency,
    string Risk);