using System.Text.RegularExpressions;
using RetailPOS.Domain.Exceptions;

namespace RetailPOS.Domain.ValueObjects
{
    public sealed class Email
    {
        public string Address { get; }

        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public Email(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Email cannot be empty.");

            var trimmed = address.Trim();
            if (!EmailRegex.IsMatch(trimmed))
                throw new DomainException("Invalid email format.");

            Address = trimmed;
        }

        public override string ToString() => Address;
    }
}
