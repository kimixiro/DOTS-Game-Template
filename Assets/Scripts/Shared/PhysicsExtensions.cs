using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace DOTSTemplate
{
    public static class PhysicsExtensions
    {
        public static void ApplyAcceleration(ref this PhysicsVelocity pv, in PhysicsMass pm, in Translation t, in Rotation r, in float3 impulse, in float3 point)
        {
            // Linear
            pv.ApplyLinearAcceleration(impulse);

            // Angular
            {
                // Calculate point impulse
                var worldFromEntity = new RigidTransform(r.Value, t.Value);
                var worldFromMotion = math.mul(worldFromEntity, pm.Transform);
                float3 angularImpulseWorldSpace = math.cross(point - worldFromMotion.pos, impulse);
                float3 angularImpulseInertiaSpace = math.rotate(math.inverse(worldFromMotion.rot), angularImpulseWorldSpace);

                pv.ApplyAngularAcceleration(angularImpulseInertiaSpace);
            }
        }

        public static void ApplyLinearAcceleration(ref this PhysicsVelocity velocityData, in float3 impulse)
        {
            velocityData.Linear += impulse;
        }

        public static void ApplyAngularAcceleration(ref this PhysicsVelocity velocityData, in float3 impulse)
        {
            velocityData.Angular += impulse;
        }
    }
}