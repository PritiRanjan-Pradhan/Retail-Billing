using System;

namespace RetailPOS.Domain.Common
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }

        public override bool Equals(object? obj)
        {
            return obj is Entity entity && Id.Equals(entity.Id);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
