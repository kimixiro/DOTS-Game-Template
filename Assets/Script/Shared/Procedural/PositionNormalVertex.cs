using Unity.Mathematics;
using UnityEngine.Rendering;

namespace DOTSTemplate.Procedural
{
    public struct PositionNormalVertex
    {
        [VertexAttribute(VertexAttribute.Position, 3)]
        public float3 Position;
        
        [VertexAttribute(VertexAttribute.Normal, 3)]
        public float3 Normal;
    }
}