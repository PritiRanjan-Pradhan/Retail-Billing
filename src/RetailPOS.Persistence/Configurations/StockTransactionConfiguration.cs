using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailPOS.Domain.Entities;

namespace RetailPOS.Persistence.Configurations
{
    public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.HasKey(st => st.Id);

            builder.Property(st => st.Date)
                .IsRequired();

            builder.Property(st => st.QuantityChange)
                .IsRequired();

            builder.Property(st => st.TransactionType)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(st => st.Product)
                .WithMany()
                .HasForeignKey(st => st.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
