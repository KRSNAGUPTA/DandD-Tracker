using DdTracker.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DdTracker.Api.Data.Configurations;

public class PortConfiguration: IEntityTypeConfiguration<Port>
{
    public void Configure(EntityTypeBuilder<Port> builder)
    {
        builder.ToTable("Ports");
        builder.HasKey(p=>p.Id);
        builder.Property(p=>p.Code).HasMaxLength(5).IsRequired(); // IN-NSA: XX->Country Code, XXX->Location, NSA->Nhava Sheva
        builder.HasIndex(p=>p.Code).IsUnique();
        builder.Property(p=>p.Name).HasMaxLength(100).IsRequired();
        builder.Property(p=>p.Country).HasMaxLength(50).IsRequired();

    }
}