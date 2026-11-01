using DdTracker.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DdTracker.Api.Data.Configurations;

public class ContainerEventConfiguration : IEntityTypeConfiguration<ContainerEvent>
{
    public void Configure(EntityTypeBuilder<ContainerEvent> builder)
    {
        builder.ToTable("ContainerEvents");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EventType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.OccurredAt).IsRequired();
        builder.Property(e => e.Note).HasMaxLength(250);

        // One of each event type per container
        builder.HasIndex(e => new { e.ContainerId, e.EventType }).IsUnique();

        builder.HasOne(e => e.Container)
               .WithMany(c => c.Events)
               .HasForeignKey(e => e.ContainerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}