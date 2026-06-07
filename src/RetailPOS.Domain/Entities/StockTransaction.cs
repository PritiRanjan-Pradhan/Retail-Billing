using System;
using RetailPOS.Domain.Common;
using RetailPOS.Domain.Enums;
using RetailPOS.Domain.Exceptions;

namespace RetailPOS.Domain.Entities
{
    public class StockTransaction : Entity
    {
        public Guid ProductId { get; private set; }
        public Product? Product { get; private set; }
        public int QuantityChange { get; private set; }
        public StockTransactionType TransactionType { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public string? Note { get; private set; }

        public StockTransaction(Guid productId, int quantityChange, StockTransactionType transactionType, string? note = null)
        {
            if (quantityChange == 0) throw new InvalidEntityStateException("Quantity change cannot be zero.");

            ProductId = productId;
            QuantityChange = quantityChange;
            TransactionType = transactionType;
            Note = note?.Trim();
            Date = DateTimeOffset.UtcNow;
        }
    }
}
