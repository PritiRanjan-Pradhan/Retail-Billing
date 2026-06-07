using RetailPOS.Domain.Common;
using RetailPOS.Domain.Exceptions;
using RetailPOS.Domain.ValueObjects;

namespace RetailPOS.Domain.Entities
{
    public class Customer : Entity
    {
        public string Name { get; private set; } = null!;
        public string Mobile { get; private set; } = null!;
        public Email? Email { get; private set; }
        public string? Address { get; private set; }

        public Customer(string name, string mobile, string? email = null, string? address = null)
        {
            SetName(name);
            SetMobile(mobile);
            SetEmail(email);
            Address = address?.Trim();
        }

        public void UpdateContact(string name, string mobile, string? email = null, string? address = null)
        {
            SetName(name);
            SetMobile(mobile);
            SetEmail(email);
            Address = address?.Trim();
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidEntityStateException("Customer name is required.");
            Name = name.Trim();
        }

        private void SetMobile(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                throw new InvalidEntityStateException("Customer mobile is required.");
            Mobile = mobile.Trim();
        }

        private void SetEmail(string? email)
        {
            Email = string.IsNullOrWhiteSpace(email) ? null : new Email(email);
        }
    }
}
