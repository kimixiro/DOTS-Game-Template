using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSTemplate
{
    public static class NativePolylineExtensions
    {
        public static BlobAssetReference<NativePolyline> ToNativePolylineBlob(this NativeArray<float3> points,
            Allocator allocator)
        {
            Assert.IsTrue(points.Length >= 2, "Not enough points to build polyline");
            
            var builder = new BlobBuilder(Allocator.Temp);
            ref var polyline = ref builder.ConstructRoot<NativePolyline>();
            var pointsBuilder = builder.Allocate(ref polyline.Points, points.Length);
            var segmentsBuilder = builder.Allocate(ref polyline.Segments, points.Length - 1);
            
            var previousPoint = default(float3);
            var length = 0.0f;
            for (int i = 0; i < points.Length; i++)
            {
                var currentPoint = points[i];
                pointsBuilder[i] = currentPoint;
                if (i > 0)
                {
                    var segmentLength = math.distance(currentPoint, previousPoint);
                    segmentsBuilder[i - 1] = new NativePolyline.Segment
                    {
                        Direction = math.normalizesafe(currentPoint - previousPoint),
                        Length = segmentLength,
                        Start = length
                    };
                    length += segmentLength;
                }
                previousPoint = currentPoint;
            }
            
            polyline.Length = length;
            
            return builder.CreateBlobAssetReference<NativePolyline>(allocator);
        }
    }
}