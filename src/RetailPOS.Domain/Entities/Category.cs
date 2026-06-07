using RetailPOS.Domain.Common;
using RetailPOS.Domain.Exceptions;

namespace RetailPOS.Domain.Entities
{
    public class Category : Entity
    {
        public string Name { get; private set; } = null!;

        public Category(string name)
        {
            SetName(name);
        }

        public void UpdateName(string name)
        {
            SetName(name);
        }

        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidEntityStateException("Category name is required.");

            Name = name.Trim();
        }
    }
}
