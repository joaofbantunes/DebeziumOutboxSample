using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Producer.Data;

public class OutboxMessageConfiguration: IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Payload)
            .HasColumnType("jsonb");

        // required for EF to map bind them in the ctor, even though the properties are readonly
        builder.Property(e => e.AggregateId);
        builder.Property(e => e.AggregateType);
        builder.Property(e => e.Type);
        builder.Property(e => e.Timestamp).HasColumnType("Timestamp");
    }
}