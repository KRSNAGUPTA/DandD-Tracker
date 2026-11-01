namespace DdTracker.Api.Dtos;
public record ContainerDetailDto(
    int Id,
    string ContainerNo,
    string BookingRef,
    string Type,
    string DestinationPort,
    string Status,
    DateTime CreatedAt,
    IReadOnlyList<ContainerEventDto> Events,
    ChargeDto? Demurrage,
    ChargeDto? Detention);

public record ProjectionDto(
    int ContainerId,
    string ContainerNo,
    DateTime ProjectedAsOf,
    ChargeDto? Demurrage,
    ChargeDto? Detention,
    decimal TotalAmount,
    string Currency);