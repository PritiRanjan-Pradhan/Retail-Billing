using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RetailPOS.Domain.Entities;

namespace RetailPOS.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .HasMaxLength(120)
                .IsRequired();

            builder.Property(c => c.Email)
                .HasMaxLength(120);

            builder.Property(c => c.Mobile)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(c => c.Address)
                .HasMaxLength(250);
        }
    }
}
