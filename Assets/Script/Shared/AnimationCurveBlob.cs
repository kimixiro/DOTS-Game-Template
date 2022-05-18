using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSTemplate
{
    public struct AnimationCurveBlob
    {
        private BlobArray<float> sampledValues;
        private float2 timeRange;

        public float2 TimeRange => timeRange;

        public static BlobAssetReference<AnimationCurveBlob> Build(AnimationCurve curve, int intervalCount,
            Allocator allocator)
        {
            using var blobBuilder = new BlobBuilder(Allocator.Temp);
            ref var root = ref blobBuilder.ConstructRoot<AnimationCurveBlob>();

            var sampledValues = blobBuilder.Allocate(ref root.sampledValues, intervalCount + 1);

            var timeFrom = curve.keys[0].time;
            var timeTo = curve.keys[curve.keys.Length - 1].time;
            var timeStep = (timeTo - timeFrom) / intervalCount;

            for (int i = 0; i < intervalCount + 1; i++)
            {
                sampledValues[i] = curve.Evaluate(timeFrom + i * timeStep);
            }
            
            root.timeRange = new float2(timeFrom, timeTo);

            return blobBuilder.CreateBlobAssetReference<AnimationCurveBlob>(allocator);
        }

        public float Evaluate(float time)
        {
            var intervalCount = sampledValues.Length - 1;

            var clamp01 = math.unlerp(timeRange.x, timeRange.y, math.clamp(time, timeRange.x, timeRange.y));
            var timeInInterval = clamp01 * intervalCount;
            var segmentIndex = (int) math.floor(timeInInterval);
            if (segmentIndex >= intervalCount) return sampledValues[intervalCount];

            var bottom = sampledValues[segmentIndex];
            var top = sampledValues[segmentIndex + 1];

            return math.lerp(bottom, top, timeInInterval - segmentIndex);
        }
    }
}