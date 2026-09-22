using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OmnesoftChallenge.DAL.Entities;

namespace OmnesoftChallenge.DAL.EntitiyMapping;

public class ProductMapping : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Price).HasPrecision(15, 2).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.InsertionDate).IsRequired();
        builder.Property(x => x.LastUpdateDate).IsRequired();

        builder.ToTable("product");
    }
}
