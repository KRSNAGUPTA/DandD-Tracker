namespace DdTracker.Api.Dtos;

public record AlertDto(
    int Id,
    int ContainerId,
    string ContainerNo,
    string Severity,
    string ChargeType,
    string Message,
    bool Acknowledged,
    DateTime CreatedAt);