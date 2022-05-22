using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSTemplate
{
    public class PathTaskAuthoring : TaskAuthoring
    {
        public float Complexity = 0.5f;
        public CurvedPathProvider PathProvider;
        
        public override void Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            base.Convert(entity, dstManager, conversionSystem);
            dstManager.AddComponents(entity,
                new ComponentTypes(
                    typeof(PathTask),
                    typeof(Progress)
                ));
            
            var transformMatrix = (float4x4)transform.localToWorldMatrix;
            using var points = 
                PathProvider.GetPath(ref transformMatrix, Complexity);
            
            var pathBlob = points.AsArray().ToNativePolylineBlob(Allocator.Persistent);
            conversionSystem.BlobAssetStore.AddUniqueBlobAsset(ref pathBlob);
            
            dstManager.SetComponentData(entity, new PathTask
            {
                Path = pathBlob
            });
        }
    }
    
    public struct PathTask : IComponentData
    {
        public BlobAssetReference<NativePolyline> Path;
    }
}