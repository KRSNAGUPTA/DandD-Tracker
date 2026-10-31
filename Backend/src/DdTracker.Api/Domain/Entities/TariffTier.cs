using DdTracker.Api.Domain.Enums;

namespace DdTracker.Api.Domain.Entities;

public class TariffTier
{
    public int Id {get; set;}

    public int ContainerTypeId {get; set;}
    public ChargeType ChargeType {get; set;}
    public int FromDay{get; set;}
    public int? ToDay{get; set;}
    public decimal RatePerDay {get; set;} // 0 to 5 -> Free; later charges
    public string Currency{get; set; } = null!; // USD

    public ContainerType ContainerType{get; set;} = null!;
}