using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    public class MissionSceneAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var authorings = GetComponentsInChildren<MissionSceneObjectAuthoring>(true);
            
            var data = MissionSceneData.Build(
                authorings.Select(conversionSystem.GetPrimaryEntity).ToList(), 
                Allocator.Persistent);
            
            conversionSystem.BlobAssetStore.AddUniqueBlobAsset(ref data);
            
            dstManager.AddComponentData(entity, new MissionScene
            {
                Data = data
            });
        }
    }

    public struct MissionScene : IComponentData
    {
        public BlobAssetReference<MissionSceneData> Data;
    }

    public struct MissionSceneData
    {
        public BlobArray<Entity> Objects;

        public static BlobAssetReference<MissionSceneData> Build(List<Entity> entities, Allocator allocator)
        {
            using var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<MissionSceneData>();
            var objects = builder.Allocate(ref root.Objects, entities.Count);
            
            for (var index = 0; index < entities.Count; index++)
            {
                objects[index] = entities[index];
            }

            return builder.CreateBlobAssetReference<MissionSceneData>(allocator);
        }
    }
}