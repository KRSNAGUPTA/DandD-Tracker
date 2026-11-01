public record CreateEventRequest(
    string EventType,
    DateTime OccurredAt,
    string? Note);