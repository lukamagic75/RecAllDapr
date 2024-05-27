using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecAll.Core.List.Domain.AggregateModels;

namespace RecAll.Core.List.Infrastructure.EntityConfigurations;

public class ListTypeConfiguration : IEntityTypeConfiguration<ListType> {
    public void Configure(EntityTypeBuilder<ListType> builder) {
        builder.ToTable("listtypes");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasDefaultValue(1).ValueGeneratedNever()
            .IsRequired();
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.DisplayName).HasMaxLength(200).IsRequired();
    }
}