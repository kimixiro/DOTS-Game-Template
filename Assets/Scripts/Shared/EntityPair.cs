using System;
using Unity.Entities;

namespace DOTSTemplate
{
    public readonly struct EntityPair : IEquatable<EntityPair>
    {
        public readonly Entity A;
        public readonly Entity B;
        
        public EntityPair(Entity a, Entity b)
        {
            if (a.Index < b.Index)
            {
                A = a;
                B = b;
            }
            else
            {
                A = b;
                B = a;
            }
        }
        
        public bool Equals(EntityPair other)
        {
            return A.Equals(other.A) && B.Equals(other.B);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityPair other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (A.Index * 397) ^ B.Index;
            }
        }
    }
}