using Unity.Collections;
using Unity.Mathematics;

namespace DOTSTemplate.Contacts
{
    public struct CollisionData
    {
        public float Impulse;
        public float3 Normal;
        public float3 AverageContactPoint;
        public FixedList64Bytes<float3> ContactPoints;
    }
}