using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecAll.Infrastructure.IntegrationEventLog.Models;

namespace RecAll.Infrastructure.IntegrationEventLog;

public class IntegrationEventLogContext : DbContext {
    public IntegrationEventLogContext(
        DbContextOptions<IntegrationEventLogContext> options) :
        base(options) { }

    public DbSet<IntegrationEventLogEntry> IntegrationEventLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.Entity<IntegrationEventLogEntry>(
            ConfigureIntegrationEventLogEntry);

    protected void ConfigureIntegrationEventLogEntry(
        EntityTypeBuilder<IntegrationEventLogEntry> builder) {
        builder.ToTable("IntegrationEventLog");
        builder.HasKey(p => p.EventId);
        builder.Property(p => p.EventId).IsRequired();
        builder.Property(p => p.ContentJson).IsRequired();
        builder.Property(p => p.CreatedTime).IsRequired();
        builder.Property(p => p.State).IsRequired();
        builder.Property(p => p.TimesSent).IsRequired();
        builder.Property(p => p.EventTypeName).IsRequired();
    }
}

public class IntegrationEventLogContextDesignFactory :
    IDesignTimeDbContextFactory<IntegrationEventLogContext> {
    public IntegrationEventLogContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<IntegrationEventLogContext>()
            .UseSqlServer(".").Options);
}