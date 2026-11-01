namespace DdTracker.Api.Dtos;
public record ContainerEventDto(
    string EventType,
    DateTime OccurredAt,
    string? Note);