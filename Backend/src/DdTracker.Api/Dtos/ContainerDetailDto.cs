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