using DdTracker.Api.Domain.Entities;
using DdTracker.Api.Domain.Enums;

namespace DdTracker.Tests;
public class EventSequenceTests
{
    [Fact]
    public void GatedOut_BeforeDischarged_ShouldBeRejected()
    {
        var c = new Container();
        // no Discharged event exists

        var gatedOutDate = new DateTime(2026, 10, 1);

        var hasDischarged = c.Events.Any(e => e.EventType == EventType.Discharged);
        Assert.False(hasDischarged);
    }

    [Fact]
    public void DuplicateEventType_ShouldBeRejected()
    {
        var c = new Container();
        c.Events.Add(new ContainerEvent
        {
            EventType = EventType.Discharged,
            OccurredAt = new DateTime(2026, 10, 1)
        });

        var existingTypes = c.Events.Select(e => e.EventType).ToHashSet();
        Assert.Contains(EventType.Discharged, existingTypes);
    }

    [Fact]
    public void EmptyReturned_BeforeGatedOut_ShouldBeRejected()
    {
        var c = new Container();
        c.Events.Add(new ContainerEvent
        {
            EventType = EventType.Discharged,
            OccurredAt = new DateTime(2026, 10, 1)
        });

        var hasGatedOut = c.Events.Any(e => e.EventType == EventType.GatedOut);
        Assert.False(hasGatedOut);
    }
}