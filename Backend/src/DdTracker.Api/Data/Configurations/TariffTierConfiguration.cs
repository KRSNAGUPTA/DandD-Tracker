using DdTracker.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DdTracker.Api.Data.Configurations;

public class TariffTierConfiguration : IEntityTypeConfiguration<TariffTier>
{
    public void Configure(EntityTypeBuilder<TariffTier> builder)
    {
        builder.ToTable("TariffTiers", tb => tb.HasCheckConstraint(
            "CK_TariffTier_Range",
            "ToDay IS NULL OR ToDay >= FromDay"));

        builder.HasKey(t => t.Id);
        builder.Property(t => t.ChargeType).HasConversion<string>().HasMaxLength(15).IsRequired();
        builder.Property(t => t.RatePerDay).HasColumnType("decimal(10,2)").IsRequired();
        builder.Property(t => t.Currency).HasMaxLength(3).IsRequired().HasDefaultValue("USD");

        // One tier per (charge type, container type, starting day)
        builder.HasIndex(t => new { t.ChargeType, t.ContainerTypeId, t.FromDay }).IsUnique();

        builder.HasOne(t => t.ContainerType)
               .WithMany()
               .HasForeignKey(t => t.ContainerTypeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}