using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecAll.Core.List.Domain.AggregateModels.SetAggregate;

namespace RecAll.Core.List.Infrastructure.EntityConfigurations;

public class SetConfiguration : IEntityTypeConfiguration<Set> {
    public void Configure(EntityTypeBuilder<Set> builder) {
        builder.ToTable("sets");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id)
            .UseHiLo("setseq", ListContext.DefaultSchema);
        builder.Ignore(p => p.DomainEvents);

        builder.Property<string>("_name")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Name").IsRequired();

        builder.Property<int>("_typeId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("TypeId").IsRequired();

        builder.HasOne(p => p.Type).WithMany().HasForeignKey("_typeId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property<int>("_listId")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("ListId").IsRequired();

        builder.HasOne<Domain.AggregateModels.ListAggregate.List>().WithMany()
            .IsRequired().HasForeignKey("_listId");

        builder.Property(p => p.UserIdentityGuid).HasField("_userIdentityGuid")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("UserIdentityGuid").IsRequired();

        builder.Property(p => p.IsDeleted).HasField("_isDeleted")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("IsDeleted").IsRequired();
    }
}