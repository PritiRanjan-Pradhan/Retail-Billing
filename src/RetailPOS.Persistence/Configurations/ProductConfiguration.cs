using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailPOS.Domain.Entities;

namespace RetailPOS.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(250);
            builder.Property(p => p.SKU).IsRequired().HasMaxLength(100);
            builder.Property(p => p.Barcode).HasMaxLength(120);
            builder.Property(p => p.Price).IsRequired().HasPrecision(18, 2);
            builder.Property(p => p.StockQuantity).IsRequired();
            builder.Property(p => p.ReorderLevel).IsRequired();
            builder.HasIndex(p => p.SKU).IsUnique();
            builder.HasIndex(p => p.Barcode).IsUnique();
        }
    }
}
