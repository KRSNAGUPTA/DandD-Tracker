using DdTracker.Api.Domain.Enums;

namespace DdTracker.Api.Domain.Entities;

public class Alert
{
    public int Id{get; set;}
    public int ContainerId {get; set;}
    public AlertSeverity Severity{get; set;}
    public ChargeType ChargeType{get; set;}
    public string Message {get; set;} = null!;
    public bool Acknowledged {get; set;}
    public DateTime CreatedAt{get; set;}

    public Container Container{get; set;} = null!;

}