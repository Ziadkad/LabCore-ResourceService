using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResourceService.Domain.Resource;

namespace ResourceService.Infrastructure.Data.Config;

public class ResourceConfig : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();
        
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.QuantityAvailable);
        
        builder.Property(x => x.Status)
            .IsRequired();
        
        builder.ToTable("Resources");
    }
}