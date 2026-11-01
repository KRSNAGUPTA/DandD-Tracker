using DdTracker.Api.Domain.Entities;
using DdTracker.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DdTracker.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Ports.AnyAsync()) return;

        var ports = new[]
        {
            new Port { Code = "INNSA", Name = "Nhava Sheva",  Country = "India" },
            new Port { Code = "LKCMB", Name = "Port of Colombo", Country = "Sri Lanka" },
            new Port { Code = "SGSIN", Name = "Singapore",    Country = "Singapore" },
            new Port { Code = "AEJEA", Name = "Jebel Ali",    Country = "UAE" },
        };
        db.Ports.AddRange(ports);
        await db.SaveChangesAsync();

        var types = new[]
        {
            new ContainerType { Code = "20GP",    Description = "20ft General Purpose" },
            new ContainerType { Code = "40GP",    Description = "40ft General Purpose" },
            new ContainerType { Code = "40HC",    Description = "40ft High Cube" },
            new ContainerType { Code = "ISOTANK", Description = "ISO Tank Container" },
        };
        db.ContainerTypes.AddRange(types);
        await db.SaveChangesAsync();

        var typeId = types.ToDictionary(t => t.Code, t => t.Id);

        void Tier(string code, ChargeType ct, int from, int? to, decimal rate) =>
            db.TariffTiers.Add(new TariffTier
            {
                ContainerTypeId = typeId[code],
                ChargeType = ct,
                FromDay = from,
                ToDay = to,
                RatePerDay = rate,
                Currency = "USD"
            });

        Tier("40HC", ChargeType.Demurrage, 1, 5, 0);
        Tier("40HC", ChargeType.Demurrage, 6, 10, 25);
        Tier("40HC", ChargeType.Demurrage, 11, null, 50);
        Tier("40HC", ChargeType.Detention, 1, 7, 0);
        Tier("40HC", ChargeType.Detention, 8, 14, 30);
        Tier("40HC", ChargeType.Detention, 15, null, 60);

        Tier("40GP", ChargeType.Demurrage, 1, 4, 0);
        Tier("40GP", ChargeType.Demurrage, 5, 8, 20);
        Tier("40GP", ChargeType.Demurrage, 9, null, 40);
        Tier("40GP", ChargeType.Detention, 1, 6, 0);
        Tier("40GP", ChargeType.Detention, 7, 12, 25);
        Tier("40GP", ChargeType.Detention, 13, null, 50);

        Tier("20GP", ChargeType.Demurrage, 1, 3, 0);
        Tier("20GP", ChargeType.Demurrage, 4, 7, 15);
        Tier("20GP", ChargeType.Demurrage, 8, null, 30);
        Tier("20GP", ChargeType.Detention, 1, 5, 0);
        Tier("20GP", ChargeType.Detention, 6, 10, 20);
        Tier("20GP", ChargeType.Detention, 11, null, 40);

        Tier("ISOTANK", ChargeType.Demurrage, 1, 5, 0);
        Tier("ISOTANK", ChargeType.Demurrage, 6, 10, 40);
        Tier("ISOTANK", ChargeType.Demurrage, 11, null, 80);
        Tier("ISOTANK", ChargeType.Detention, 1, 7, 0);
        Tier("ISOTANK", ChargeType.Detention, 8, 14, 50);
        Tier("ISOTANK", ChargeType.Detention, 15, null, 100);

        await db.SaveChangesAsync();

        var portId = ports.ToDictionary(p => p.Code, p => p.Id);
        var now = DateTime.UtcNow;

        Container Make(
            string no, string booking, string typeCode, string portCode,
            int? dischargedDaysAgo = null,
            int? gatedOutDaysAgo = null,
            int? returnedDaysAgo = null)
        {
            var c = new Container
            {
                ContainerNo = no,
                BookingRef = booking,
                ContainerTypeId = typeId[typeCode],
                DestinationPortId = portId[portCode],
                CreatedAt = now,
                Status = returnedDaysAgo.HasValue ? ContainerStatus.Closed
                       : gatedOutDaysAgo.HasValue  ? ContainerStatus.OutForDelivery
                       : ContainerStatus.AtPort
            };

            if (dischargedDaysAgo.HasValue)
                c.Events.Add(new ContainerEvent { EventType = EventType.Discharged,    OccurredAt = now.AddDays(-dischargedDaysAgo.Value) });
            if (gatedOutDaysAgo.HasValue)
                c.Events.Add(new ContainerEvent { EventType = EventType.GatedOut,      OccurredAt = now.AddDays(-gatedOutDaysAgo.Value) });
            if (returnedDaysAgo.HasValue)
                c.Events.Add(new ContainerEvent { EventType = EventType.EmptyReturned, OccurredAt = now.AddDays(-returnedDaysAgo.Value) });

            return c;
        }

        db.Containers.Add(Make("ECNU1234567", "BK-1001", "40HC", "INNSA", dischargedDaysAgo: 0));
        db.Containers.Add(Make("MSCU1111111", "BK-1002", "20GP", "LKCMB", dischargedDaysAgo: 0));
        db.Containers.Add(Make("COSU3333333", "BK-1014", "40HC", "LKCMB", dischargedDaysAgo: 1));
        db.Containers.Add(Make("CMAU2222222", "BK-1003", "40HC", "SGSIN", dischargedDaysAgo: 3));
        db.Containers.Add(Make("HLXU3333333", "BK-1004", "40GP", "AEJEA", dischargedDaysAgo: 3));
        db.Containers.Add(Make("TGHU4444444", "BK-1005", "40HC", "INNSA", dischargedDaysAgo: 5));
        db.Containers.Add(Make("OOLU5555555", "BK-1006", "20GP", "LKCMB", dischargedDaysAgo: 9));
        db.Containers.Add(Make("HLBU6666666", "BK-1007", "40GP", "SGSIN", dischargedDaysAgo: 11));
        db.Containers.Add(Make("TCLU7777777", "BK-1008", "ISOTANK","AEJEA", dischargedDaysAgo: 14));
        db.Containers.Add(Make("EGHU8888888", "BK-1009", "40HC", "INNSA", dischargedDaysAgo: 7,  gatedOutDaysAgo: 2));
        db.Containers.Add(Make("TCNU9999999", "BK-1010", "20GP", "LKCMB", dischargedDaysAgo: 5,  gatedOutDaysAgo: 1));
        db.Containers.Add(Make("SUDU1010101", "BK-1011", "40HC", "SGSIN", dischargedDaysAgo: 14, gatedOutDaysAgo: 7));
        db.Containers.Add(Make("MNBU1111111", "BK-1012", "40GP", "AEJEA", dischargedDaysAgo: 20, gatedOutDaysAgo: 15, returnedDaysAgo: 1));
        db.Containers.Add(Make("MEDU2222222", "BK-1013", "20GP", "INNSA", dischargedDaysAgo: 25, gatedOutDaysAgo: 20, returnedDaysAgo: 2));

        await db.SaveChangesAsync();
    }
}