using Unity.Entities;
using Unity.Mathematics;

namespace DOTSTemplate.Tweening
{
    public struct RotationTween : IComponentData
    {
        public float3 EndValue;
        public float3 StartValue;
    }
}