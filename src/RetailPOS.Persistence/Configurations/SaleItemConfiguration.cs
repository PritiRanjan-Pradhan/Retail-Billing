using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailPOS.Domain.Entities;

namespace RetailPOS.Persistence.Configurations
{
    public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
    {
        public void Configure(EntityTypeBuilder<SaleItem> builder)
        {
            builder.HasKey(si => si.Id);

            builder.Property(si => si.ProductId)
                .IsRequired();

            builder.Property(si => si.ProductName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(si => si.SKU)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(si => si.Quantity)
                .IsRequired();

            builder.Property(si => si.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Ignore(si => si.LineTotal);

            builder.Property(si => si.SaleId)
                .IsRequired();

            builder.HasOne(si => si.Sale)
                .WithMany(s => s.Items)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
