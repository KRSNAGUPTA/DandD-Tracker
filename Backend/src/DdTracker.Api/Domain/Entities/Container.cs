using DdTracker.Api.Domain.Enums;

namespace DdTracker.Api.Domain.Entities;

public class Container
{
    public int Id{get; set;}
    public string ContainerNo{get;set;}="";
    public string BookingRef{get; set;} = "";
    public int ContainerTypeId{get; set;}
    public int DestinationPortId{get; set;}
    public ContainerStatus Status {get; set;}
    public DateTime CreatedAt{get; set;}

    public ContainerType ContainerType {get; set;}=null!;
    public Port DestinationPort {get; set;} = null!;
    public List<ContainerEvent>Events {get; set;} = new();
    public List<Alert>Alerts{get; set;} = new();
}