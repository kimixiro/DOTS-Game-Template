using Unity.Entities;
using Unity.Mathematics;

namespace DOTSTemplate
{
    public struct NativePolyline
    {
        public float Length;
        public BlobArray<float3> Points;
        public BlobArray<Segment> Segments;

        public unsafe (DirectedPoint ClosestPoint, float ClosestPositionOnPath)
            GetClosestPoint(float3 point)
        {
            float3* points = (float3*) Points.GetUnsafePtr();
            Segment* segments = (Segment*) Segments.GetUnsafePtr();
            var segmentCount = Segments.Length;

            var closestDistanceSq = float.MaxValue;
            var closestPositionOnPath = 0.0f;
            var closestPoint = default(DirectedPoint);
            for (int i = 0; i < segmentCount; i++)
            {
                ref var segment = ref segments[i];
                ref var p0 = ref points[i];
                ref var p1 = ref points[i + 1];

                var (closestPosition, d) =
                    ClosestPointToSegment(ref p0, ref p1, ref segment, ref point);
                var distanceSq = math.distancesq(closestPosition, point);
                if (distanceSq < closestDistanceSq)
                {
                    closestPoint = new DirectedPoint(point, segment.Direction);
                    closestPositionOnPath = segment.Start + d;
                }
            }

            return (closestPoint, closestPositionOnPath);
        }

        public unsafe (DirectedPoint ClosestPoint, float ClosestPositionOnPath)
            GetClosestPoint(float positionOnPath, float3 point)
        {
            float3* points = (float3*) Points.GetUnsafePtr();
            Segment* segments = (Segment*) Segments.GetUnsafePtr();
            var segmentCount = Segments.Length;

            var activeSegmentIndex = GetSegmentIndex(segments, segmentCount, positionOnPath);

            ref var activeSegment = ref segments[activeSegmentIndex];
            var closestToCurrentSegment = ClosestPointToSegment(
                ref points[activeSegmentIndex],
                ref points[activeSegmentIndex + 1],
                ref activeSegment,
                ref point);
            var distanceToCurrentSegmentSquared =
                math.distancesq(closestToCurrentSegment.ClosestPoint, point);

            var nextClosestIndex = GetClosestSegmentInDirection(activeSegmentIndex, 1, points,
                segments, segmentCount, ref point, distanceToCurrentSegmentSquared,
                out var nextClosestPoint, out var nextClosestPosition);

            if (nextClosestIndex != activeSegmentIndex)
            {
                return (nextClosestPoint, nextClosestPosition);
            }

            var previousClosestIndex = GetClosestSegmentInDirection(activeSegmentIndex, -1, points,
                segments, segmentCount, ref point, distanceToCurrentSegmentSquared,
                out var previousClosestPoint, out var previousClosestPosition);

            if (previousClosestIndex != activeSegmentIndex)
            {
                return (previousClosestPoint, previousClosestPosition);
            }

            return (new DirectedPoint(closestToCurrentSegment.ClosestPoint, activeSegment.Direction),
                activeSegment.Start + closestToCurrentSegment.PositionOnSegment);
        }

        private static unsafe int GetClosestSegmentInDirection(int activeSegmentIndex, sbyte direction,
            float3* points, Segment* segments, int segmentCount,
            ref float3 point, float distanceToCurrentSq,
            out DirectedPoint closestPoint, out float positionOnPath)
        {
            closestPoint = default;
            positionOnPath = 0;
            var dir = direction >= 0 ? 1 : -1;
            while (dir > 0 && activeSegmentIndex < segmentCount - 1 || dir < 0 && activeSegmentIndex > 0)
            {
                var nextSegmentIndex = activeSegmentIndex + dir;
                ref var nextSegment = ref segments[nextSegmentIndex];
                var closestToNextSegment = ClosestPointToSegment(
                    ref points[nextSegmentIndex],
                    ref points[nextSegmentIndex + dir],
                    ref nextSegment,
                    ref point);
                var d = math.distancesq(closestToNextSegment.ClosestPoint, point);

                if (d > distanceToCurrentSq)
                {
                    break;
                }

                closestPoint = new DirectedPoint(closestToNextSegment.ClosestPoint, nextSegment.Direction);
                positionOnPath = nextSegment.Start + closestToNextSegment.PositionOnSegment;

                activeSegmentIndex += dir;
            }

            return activeSegmentIndex;
        }

        private unsafe int GetSegmentIndex(Segment* segments,
            int segmentCount, float positionOnPath)
        {
            if (positionOnPath < 0) return 0;
            for (int i = 0; i < segmentCount; i++)
            {
                ref var segment = ref segments[i];
                var positionOnSegment = positionOnPath - segment.Start;
                if (positionOnSegment >= 0 && positionOnSegment < segment.Length) return i;
            }

            return segmentCount - 1;
        }

        private static (float3 ClosestPoint, float PositionOnSegment)
            ClosestPointToSegment(ref float3 p0, ref float3 p1, ref Segment segment, ref float3 point)
        {
            var d = math.dot(segment.Direction, point - p0);
            if (d < 0)
                return (p0, 0);
            if (d > segment.Length)
                return (p1, segment.Length);
            return (p0 + d * segment.Direction, d);
        }

        public struct Segment
        {
            public float3 Direction;
            public float Length;
            public float Start;
        }
    }
}