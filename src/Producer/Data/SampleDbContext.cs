using Microsoft.EntityFrameworkCore;

namespace Producer.Data;

public class SampleDbContext : DbContext
{
#pragma warning disable CS8618 // initialized by EF
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
#pragma warning restore CS8618
    {
    }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}