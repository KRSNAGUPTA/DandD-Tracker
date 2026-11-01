using DdTracker.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DdTracker.Api.Data.Configurations;

public class ContainerTypeConfiguration : IEntityTypeConfiguration<ContainerType>
{
    public void Configure(EntityTypeBuilder<ContainerType> builder)
    {
        builder.ToTable("ContainerTypes");
        builder.HasKey(ct => ct.Id);
        builder.Property(ct => ct.Code).HasMaxLength(10).IsRequired();
        builder.HasIndex(ct => ct.Code).IsUnique();
        builder.Property(ct => ct.Description).HasMaxLength(100).IsRequired();
    }
}