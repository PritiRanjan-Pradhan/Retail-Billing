using RetailPOS.Domain.Exceptions;
using RetailPOS.Domain.Common;
using System;

namespace RetailPOS.Domain.Entities
{
    public class SaleItem : Entity
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public string SKU { get; private set; }
        public decimal UnitPrice { get; private set; }
        public int Quantity { get; private set; }

        public SaleItem(Guid productId, string productName, string sku, decimal unitPrice, int quantity)
        {
            if (quantity <= 0) throw new InvalidEntityStateException("Quantity must be greater than zero.");
            if (unitPrice < 0) throw new InvalidEntityStateException("Unit price cannot be negative.");

            ProductId = productId;
            ProductName = productName;
            SKU = sku;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }

        public decimal LineTotal => UnitPrice * Quantity;

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0) throw new InvalidEntityStateException("Quantity must be greater than zero.");
            Quantity = quantity;
        }
    }
}
