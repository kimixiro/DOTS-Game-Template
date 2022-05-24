using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;

namespace DOTSTemplate
{
    public struct CollisionData
    {
        public float Impulse;
        public float3 Normal;
        public float3 AverageContactPoint;
        public List<float3> ContactPoints;
    }
}