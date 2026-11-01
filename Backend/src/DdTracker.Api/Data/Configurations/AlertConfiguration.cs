using DdTracker.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DdTracker.Api.Data.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> builder)
    {
        builder.ToTable("Alerts");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Severity).HasConversion<string>().HasMaxLength(10).IsRequired();
        builder.Property(a => a.ChargeType).HasConversion<string>().HasMaxLength(15).IsRequired();
        builder.Property(a => a.Message).HasMaxLength(300).IsRequired();
        builder.Property(a => a.Acknowledged).IsRequired();
        builder.Property(a => a.CreatedAt).IsRequired();

        builder.HasIndex(a => new { a.Acknowledged, a.ContainerId, a.Severity });

        builder.HasOne(a => a.Container)
               .WithMany(c => c.Alerts)
               .HasForeignKey(a => a.ContainerId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}