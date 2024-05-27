using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecAll.Contrib.TextItem.Api.Services;

public class TextItemContext : DbContext {
    public DbSet<Models.TextItem> TextItems { get; set; }

    public TextItemContext(DbContextOptions<TextItemContext> options) :
        base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfiguration(new TextItemConfiguration());
    }
}

public class TextItemConfiguration : IEntityTypeConfiguration<Models.TextItem> {
    public void Configure(EntityTypeBuilder<Models.TextItem> builder) {
        builder.ToTable("textitems");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseHiLo("textitem_hilo");

        builder.Property(p => p.ItemId).IsRequired(false);
        builder.HasIndex(p => p.ItemId).IsUnique();

        builder.Property(p => p.Content).IsRequired();

        builder.Property(p => p.UserIdentityGuid).IsRequired();
        builder.HasIndex(p => p.UserIdentityGuid).IsUnique(false);

        builder.Property(p => p.IsDeleted).IsRequired();
    }
}

public class
    TextListContextDesignFactory : IDesignTimeDbContextFactory<
    TextItemContext> {
    public TextItemContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<TextItemContext>()
            .UseSqlServer(
                "Server=.;Initial Catalog=RecAll.TextListDb;Integrated Security=true")
            .Options);
}