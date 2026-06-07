using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailPOS.Domain.Entities;

namespace RetailPOS.Persistence.Configurations
{
    public class SaleConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Date)
                .IsRequired();

            builder.Property(s => s.TaxAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(s => s.DiscountAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(s => s.PaymentMethod)
                .HasMaxLength(50);

            builder.HasOne(s => s.Customer)
                .WithMany()
                .HasForeignKey(s => s.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(s => s.Items)
                .WithOne(si => si.Sale)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
