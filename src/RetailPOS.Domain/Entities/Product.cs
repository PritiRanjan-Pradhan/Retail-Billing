using RetailPOS.Domain.Common;
using RetailPOS.Domain.Exceptions;
using System;

namespace RetailPOS.Domain.Entities
{
    public class Product : Entity
    {
        public string Name { get; private set; } = null!;
        public string SKU { get; private set; } = null!;
        public string? Barcode { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public int ReorderLevel { get; private set; }
        public Guid? CategoryId { get; private set; }

        public Product(string name, string sku, decimal price, int stockQuantity = 0, int reorderLevel = 0, string? barcode = null, Guid? categoryId = null)
        {
            SetName(name);
            SetSku(sku);
            SetPrice(price);
            SetStock(stockQuantity);
            SetReorderLevel(reorderLevel);
            Barcode = barcode?.Trim();
            CategoryId = categoryId;
        }

        public void Update(string name, string sku, decimal price, int reorderLevel, string? barcode = null, Guid? categoryId = null)
        {
            SetName(name);
            SetSku(sku);
            SetPrice(price);
            SetReorderLevel(reorderLevel);
            Barcode = barcode?.Trim();
            CategoryId = categoryId;
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidEntityStateException("Product name is required.");
            Name = name.Trim();
        }

        private void SetSku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new InvalidEntityStateException("SKU is required.");
            SKU = sku.Trim();
        }

        private void SetPrice(decimal price)
        {
            if (price <= 0)
                throw new InvalidEntityStateException("Price must be greater than zero.");
            Price = price;
        }

        private void SetStock(int quantity)
        {
            if (quantity < 0)
                throw new InvalidEntityStateException("Stock quantity cannot be negative.");
            StockQuantity = quantity;
        }

        private void SetReorderLevel(int reorderLevel)
        {
            if (reorderLevel < 0)
                throw new InvalidEntityStateException("Reorder level cannot be negative.");
            ReorderLevel = reorderLevel;
        }

        public void IncreaseStock(int amount)
        {
            if (amount <= 0) throw new InvalidEntityStateException("Increase amount must be positive.");
            StockQuantity += amount;
        }

        public void DecreaseStock(int amount)
        {
            if (amount <= 0) throw new InvalidEntityStateException("Decrease amount must be positive.");
            if (StockQuantity - amount < 0) throw new InvalidEntityStateException("Insufficient stock.");
            StockQuantity -= amount;
        }

        public bool IsLowStock() => StockQuantity <= ReorderLevel;
    }
}
