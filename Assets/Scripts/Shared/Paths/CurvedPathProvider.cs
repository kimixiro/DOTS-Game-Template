using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSTemplate
{
    public class CurvedPathProvider : MonoBehaviour
    {
        public PointWithRadius[] Points;
        public float DefaultRadius = 2.0f;

#if UNITY_EDITOR
        void OnValidate()
        {
            if (Points == null || Points.Length < 2)
            {
                Points = new PointWithRadius[2];
            }
        }
#endif
        
        public NativeList<float3> GetPath(ref float4x4 transformMatrix, float complexity = 1)
        {
            var job = new BuildPathPoints
            {
                Transform = transformMatrix,
                UseTransform = true,
                Complexity = complexity,
                Result = new NativeList<float3>(Allocator.TempJob),
                Points = new NativeArray<PointWithRadius>(Points, Allocator.TempJob),
                DefaultRadius = DefaultRadius
            };
            
            job.Run();

            return job.Result;
        }
        
        public NativeList<float3> GetPath(float complexity = 1)
        {
            var job = new BuildPathPoints
            {
                UseTransform = false,
                Complexity = complexity,
                Result = new NativeList<float3>(Allocator.TempJob),
                Points = new NativeArray<PointWithRadius>(Points, Allocator.TempJob),
                DefaultRadius = DefaultRadius
            };
            
            job.Run();

            return job.Result;
        }

        [BurstCompile]
        private struct BuildPathPoints : IJob
        {
            [DeallocateOnJobCompletion]
            public NativeArray<PointWithRadius> Points;
            [ReadOnly]
            public float4x4 Transform;
            [ReadOnly]
            public bool UseTransform;
            [ReadOnly]
            public float DefaultRadius;
            [ReadOnly]
            public float Complexity;
            [WriteOnly]
            public NativeList<float3> Result;
            
            public void Execute()
            {
                if (UseTransform)
                {
                    for (int i = 0; i < Points.Length; i++)
                    {
                        var point = Points[i];
                        point.Position = math.transform(Transform, point.Position);
                        Points[i] = point;
                    }
                }
                
                var jointsList = new NativeArray<Joint>(Points.Length, Allocator.Temp);
                try
                {
                    for (var index = 1; index < Points.Length - 1; index++)
                    {
                        var p0 = Points[index - 1].Position;
                        var p1 = Points[index].Position;
                        var p2 = Points[index + 1].Position;
                        var d01 = p0 - p1;
                        var d12 = p2 - p1;

                        var angle = math.radians(Vector3.Angle(d01, d12));
                        var pointRadius = Points[index].GetRadiusOrDefault(DefaultRadius);
                        
                        jointsList[index] = new Joint(
                            angle, 
                            math.abs(pointRadius / math.tan(angle * 0.5f)),
                            pointRadius);
                    }

                    for (var index = 1; index < Points.Length; index++)
                    {
                        var j0 = jointsList[index - 1];
                        var j1 = jointsList[index];
                        
                        var d = math.distance(Points[index - 1].Position, Points[index].Position);
                        var jointsSum = j0.Size + j1.Size;
                        if (jointsSum > d)
                        {
                            var scale = d / jointsSum * 0.99f;
                            jointsList[index - 1] = j0.Scaled(scale);
                            jointsList[index] = j1.Scaled(scale);
                        }
                    }
                    
                    Result.Add(Points[0].Position);
                    
                    for (var index = 1; index < Points.Length - 1; index++)
                    {
                        var joint = jointsList[index];
                        var p0 = Points[index - 1].Position;
                        var p1 = Points[index].Position;
                        var p2 = Points[index + 1].Position;
                        var d1 = math.normalize(p0 - p1);
                        var d2 = math.normalize(p2 - p1);
                        
                        var pointRadius = joint.Radius;
                        var distanceToRadiusCenter = math.abs(pointRadius / math.sin(joint.Angle * 0.5f));
                        var directionToRadiusCenter = math.normalize(d2 + d1);
                        
                        var radiusCenter = p1 + directionToRadiusCenter * distanceToRadiusCenter;
                        var p01 = p1 + d1 * joint.Size;
                        var p12 = p1 + d2 * joint.Size;
                        var startDir = p01 - radiusCenter;
                        
                        var arcAngle = math.PI - joint.Angle;
                        var steps = math.max(
                            (int) (arcAngle * 5f * math.pow(joint.Radius, 1/3f) * Complexity), 2);
                        
                        var stepAngle = arcAngle / steps;

                        Result.Add(p01);

                        var normal = math.normalize(math.cross(d2, d1));
                        var stepRotation = quaternion.AxisAngle(normal, stepAngle);
                        
                        for (int i = 0; i < steps - 1; i++)
                        {
                            startDir = math.rotate(stepRotation, startDir);
                            Result.Add(radiusCenter + startDir);
                        }
                        
                        Result.Add(p12);
                    }

                    Result.Add(Points[Points.Length - 1].Position);
                }
                finally
                {
                    jointsList.Dispose();
                }
            }
            
            private readonly struct Joint
            {
                public readonly float Angle;
                public readonly float Size;
                public readonly float Radius;

                public Joint(float angle, float size, float radius)
                {
                    Angle = angle;
                    Size = size;
                    Radius = radius;
                }

                public Joint Scaled(float scale)
                {
                    return new Joint(Angle, Size * scale, Radius * scale);
                }
            }
        }
    }
}