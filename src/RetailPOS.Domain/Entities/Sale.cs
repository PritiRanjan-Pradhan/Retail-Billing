using System;
using System.Collections.Generic;
using System.Linq;
using RetailPOS.Domain.Common;
using RetailPOS.Domain.Enums;
using RetailPOS.Domain.Exceptions;

namespace RetailPOS.Domain.Entities
{
    public class Sale : AggregateRoot
    {
        private readonly List<SaleItem> _items = new();

        public DateTimeOffset Date { get; private set; }
        public Guid? CustomerId { get; private set; }
        public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();
        public decimal TaxAmount { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public string InvoiceNumber { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public bool IsPaid { get; private set; }

        public Sale(Guid? customerId = null, PaymentMethod paymentMethod = PaymentMethod.Cash)
        {
            Date = DateTimeOffset.UtcNow;
            CustomerId = customerId;
            PaymentMethod = paymentMethod;
            InvoiceNumber = GenerateInvoiceNumber();
        }

        public void AddItem(SaleItem item)
        {
            if (item == null) throw new InvalidEntityStateException("Sale item is required.");
            _items.Add(item);
        }

        public void RemoveItem(SaleItem item)
        {
            if (item == null) return;
            _items.Remove(item);
        }

        public decimal Subtotal => _items.Sum(i => i.LineTotal);
        public decimal Total => Subtotal + TaxAmount - DiscountAmount;

        public void SetTax(decimal taxAmount)
        {
            if (taxAmount < 0) throw new InvalidEntityStateException("Tax cannot be negative.");
            TaxAmount = taxAmount;
        }

        public void SetDiscount(decimal discountAmount)
        {
            if (discountAmount < 0) throw new InvalidEntityStateException("Discount cannot be negative.");
            DiscountAmount = discountAmount;
        }

        public void MarkPaid()
        {
            IsPaid = true;
        }

        public void SetPaymentMethod(PaymentMethod paymentMethod)
        {
            PaymentMethod = paymentMethod;
        }

        public void Validate()
        {
            if (!_items.Any()) throw new InvalidEntityStateException("Sale must have at least one item.");
            foreach (var item in _items)
            {
                if (item.Quantity <= 0) throw new InvalidEntityStateException("Item quantity must be greater than zero.");
            }
        }

        private static string GenerateInvoiceNumber()
        {
            var timestamp = DateTimeOffset.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"INV-{timestamp}-{random}";
        }
    }
}
