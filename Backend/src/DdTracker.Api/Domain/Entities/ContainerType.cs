namespace DdTracker.Api.Domain.Entities;

public class ContainerType
{
    public int Id {get; set;}
    public string Code{get; set;} = ""; // 20 GP, 40GP. 40HC (40HQ), ISOTANK; GP-> General Purpose, HC->High Cuber
    public string Description{get; set;} = "";
}