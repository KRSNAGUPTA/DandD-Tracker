using DdTracker.Api.Domain.Enums;

namespace DdTracker.Api.Domain.Entities;

public class ContainerEvent
{
    public int Id {get; set;}
    public int ContainerId{get; set;}
    public EventType EventType{get; set;}
    public DateTime OccurredAt{get; set;}
    public string? Note{get; set;}

    public Container Container{get; set;} = null!;
}