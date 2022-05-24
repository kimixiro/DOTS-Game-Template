using Unity.Mathematics;

namespace DOTSTemplate
{
    public static class MathExtensions
    {
        public static float3 ProjectOnPlane(this float3 vector, float3 normal)
        {
            return vector - math.dot(vector, normal) * normal;
        }
    }
}