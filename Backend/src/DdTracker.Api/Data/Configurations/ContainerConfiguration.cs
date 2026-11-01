using DdTracker.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DdTracker.Api.Data.Configurations;

public class ContainerConfiguration : IEntityTypeConfiguration<Container>
{
    public void Configure(EntityTypeBuilder<Container> builder)
    {
        builder.ToTable("Containers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ContainerNo).HasMaxLength(11).IsRequired();
        builder.HasIndex(c => c.ContainerNo).IsUnique();
        builder.Property(c => c.BookingRef).HasMaxLength(30).IsRequired();
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(c => c.CreatedAt).IsRequired();

        builder.HasIndex(c => c.Status);

        builder.HasOne(c => c.ContainerType)
               .WithMany()
               .HasForeignKey(c => c.ContainerTypeId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.DestinationPort)
               .WithMany()
               .HasForeignKey(c => c.DestinationPortId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}