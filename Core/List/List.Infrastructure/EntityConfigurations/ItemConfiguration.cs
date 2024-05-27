using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecAll.Core.List.Domain.AggregateModels.ItemAggregate;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;

namespace RecAll.Core.List.Infrastructure.EntityConfigurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item> {
    public void Configure(EntityTypeBuilder<Item> builder) {
        builder.ToTable("items");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .UseHiLo("itemseq", ListContext.DefaultSchema);
        builder.Ignore(p => p.DomainEvents);

        builder.Property<int>("_typeId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("TypeId").IsRequired();

        builder.HasOne(p => p.Type).WithMany().HasForeignKey("_typeId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property<int>("_setId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("SetId").IsRequired();

        builder.HasOne<Set>().WithMany().IsRequired().HasForeignKey("_setId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(p => p.ContribId).HasField("_contribId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("ContribId").IsRequired();

        builder.HasIndex(p => p.ContribId).IsUnique(false);

        builder.Property(p => p.UserIdentityGuid).HasField("_userIdentityGuid")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("UserIdentityGuid").IsRequired();

        builder.Property(p => p.IsDeleted).HasField("_isDeleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("IsDeleted").IsRequired();
    }
}