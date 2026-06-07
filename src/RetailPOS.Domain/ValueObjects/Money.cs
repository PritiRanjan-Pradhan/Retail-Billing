using System;
using RetailPOS.Domain.Exceptions;

namespace RetailPOS.Domain.ValueObjects
{
    public sealed class Money
    {
        public decimal Amount { get; }
        public string Currency { get; }

        public Money(decimal amount, string currency = "USD")
        {
            if (amount < 0m)
                throw new DomainException("Money amount cannot be negative.");

            Amount = amount;
            Currency = string.IsNullOrWhiteSpace(currency) ? "USD" : currency.Trim();
        }

        public static Money Zero => new Money(0m);

        public Money Add(Money other)
        {
            if (other == null) throw new DomainException("Money operand cannot be null.");
            if (!string.Equals(Currency, other.Currency, StringComparison.OrdinalIgnoreCase))
                throw new DomainException("Cannot add amounts with different currencies.");

            return new Money(Amount + other.Amount, Currency);
        }

        public override string ToString() => $"{Currency} {Amount:N2}";
    }
}
