using System;
using Unity.Entities;

namespace DOTSTemplate
{
    public readonly struct TriggerEvent : IComponentData, IEquatable<TriggerEvent>
    {
        public readonly Entity Trigger;
        public readonly Entity Source;

        public TriggerEvent(Entity trigger, Entity source)
        {
            Trigger = trigger;
            Source = source;
        }
        
        public bool Equals(TriggerEvent other)
        {
            return Trigger.Equals(other.Trigger) && Source.Equals(other.Source);
        }

        public override bool Equals(object obj)
        {
            return obj is TriggerEvent other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Trigger.GetHashCode() * 397) ^ Source.GetHashCode();
            }
        }
    }
}