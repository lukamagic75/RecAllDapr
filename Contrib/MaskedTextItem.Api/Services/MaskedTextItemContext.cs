using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RecAll.Contrib.MaskedTextItem.Api.Services;

public class MaskedTextItemContext : DbContext
{
    public DbSet<Models.MaskedTextItem> MaskedTextItems { get; set; }

    public MaskedTextItemContext(DbContextOptions<MaskedTextItemContext> options) :
        base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new MaskedTextItemConfiguration());
    }
}

public class MaskedTextItemConfiguration : IEntityTypeConfiguration<Models.MaskedTextItem>
{
    public void Configure(EntityTypeBuilder<Models.MaskedTextItem> builder)
    {
        builder.ToTable("maskedtextitems");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).UseHiLo("maskedtextitem_hilo");

        builder.Property(p => p.ItemId).IsRequired(false);
        builder.HasIndex(p => p.ItemId).IsUnique();

        builder.Property(p => p.Content).IsRequired();

        builder.Property(p => p.UserIdentityGuid).IsRequired();
        builder.HasIndex(p => p.UserIdentityGuid).IsUnique(false);

        builder.Property(p => p.IsDeleted).IsRequired();
    }
}

public class
    MaskedTextListContextDesignFactory : IDesignTimeDbContextFactory<
    MaskedTextItemContext>
{
    public MaskedTextItemContext CreateDbContext(string[] args) =>
        new(new DbContextOptionsBuilder<MaskedTextItemContext>()
            .UseSqlServer(
                "Server=.;Initial Catalog=RecAll.MaskedTextListDb;Integrated Security=true")
            .Options);
}