using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResourceService.Domain.ResourceReservation;

namespace ResourceService.Infrastructure.Data.Config;

public class ResourceReservationConfig : IEntityTypeConfiguration<ResourceReservation>
{
    public void Configure(EntityTypeBuilder<ResourceReservation> builder)
    {
        builder.ToTable("ResourceReservations");

        // Primary Key
        builder.HasKey(rr => rr.Id);

        // Properties
        builder.Property(rr => rr.Id)
            .ValueGeneratedOnAdd();

        builder.Property(rr => rr.ReservedBy)
            .IsRequired();

        builder.Property(rr => rr.TaskItemId);

        builder.Property(rr => rr.StartTime)
            .IsRequired();

        builder.Property(rr => rr.EndTime)
            .IsRequired();

        builder.Property(rr => rr.Notes);

        // Relationships
        builder.HasOne(rr => rr.Resource)
            .WithMany(r => r.ResourceReservations)
            .HasForeignKey(rr => rr.ResourceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}