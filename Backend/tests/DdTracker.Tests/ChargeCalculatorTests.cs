using DdTracker.Api.Domain.Entities;
using DdTracker.Api.Domain.Enums;
using DdTracker.Api.Domain.Services;

namespace DdTracker.Tests;

public class ChargeCalculatorTests
{
    public static readonly IReadOnlyList<TariffTier> HcDemurrage = new List<TariffTier>
    {
        new(){ChargeType=ChargeType.Demurrage, Currency="USD", FromDay=1, ToDay=5, RatePerDay=0},
        new(){ChargeType= ChargeType.Demurrage, Currency="USD", FromDay=6, ToDay=10, RatePerDay=25},
        new(){ChargeType=ChargeType.Demurrage, Currency="USD", FromDay=11, RatePerDay=50}
    };
    private static DateTime Start = new(2026, 10, 18);
    private static DateTime AsOfDays(int days)
    {
        return Start.AddDays(days - 1);
    }
    [Fact]
    public void WithinFreeDays_AmountIsZero()
    {
        var result = ChargeCalculator.Calculate(ChargeType.Demurrage, Start, null, AsOfDays(2), HcDemurrage);
        // Console.WriteLine(result);
        Assert.Equal(0m, result.Amount);
        Assert.Equal(3, result.FreeDaysRemaining);
    }

    [Fact]
    public void LastFreeDay_ZeroAmount()
    {
        var result = ChargeCalculator.Calculate(ChargeType.Demurrage, Start, new(2026, 10, 22), AsOfDays(202), HcDemurrage);
        Assert.Equal(0m, result.Amount);
        Assert.Equal(5, result.DaysElapsed);
        Assert.Equal(0, result.FreeDaysRemaining);
    }

    [Fact]
    public void InfinteDays()
    {
        var result = ChargeCalculator.Calculate(ChargeType.Demurrage, Start, null, AsOfDays(1321), HcDemurrage);
        // Console.WriteLine(result);
        Assert.Equal(0, result.FreeDaysRemaining);
        Assert.Equal(65675m, result.Amount);
        Assert.Equal(1321, result.DaysElapsed);
    }
    [Fact]
    public void Charge_OneDay()
    {
        var result = ChargeCalculator.Calculate(ChargeType.Demurrage, Start, null, AsOfDays(6), HcDemurrage);
        Assert.Equal(25m, result.Amount);
    }
    
    [Fact]
    public void Charge_7days()
    {
        var result = ChargeCalculator.Calculate(ChargeType.Demurrage, Start, null, AsOfDays(7), HcDemurrage);
        // Console.WriteLine(result);
        Assert.Equal(50m, result.Amount);
        Assert.Equal(7, result.DaysElapsed);
        Assert.Equal(0, result.FreeDaysRemaining);
    }
    [Fact]
    public void EndOf_Charge()
    {
        var result = ChargeCalculator.Calculate(ChargeType.Demurrage, Start, new(2026, 10, 28), AsOfDays(20), HcDemurrage);
        // Console.WriteLine(result);
        Assert.Equal(11, result.DaysElapsed);
        Assert.Equal(0, result.FreeDaysRemaining);
        Assert.Equal(175m, result.Amount);
    }
    [Fact]
    public void EndLessThanStart_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
        ChargeCalculator.Calculate(ChargeType.Demurrage, Start, null, AsOfDays(-2), HcDemurrage)
        );
    }
}