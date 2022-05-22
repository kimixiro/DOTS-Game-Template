using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace DOTSTemplate
{
    public class TaskVisualStateAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public IndicatorDefinition[] ActiveIndicators;
        public IndicatorDefinition[] CompleteIndicators;
        
        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            foreach (var indicatorDefinition in ActiveIndicators)
            {
                indicatorDefinition.Prefab.DeclarePrefab(referencedPrefabs);
            }
            foreach (var indicatorDefinition in CompleteIndicators)
            {
                indicatorDefinition.Prefab.DeclarePrefab(referencedPrefabs);
            }
        }
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponents(entity, new ComponentTypes(
                typeof(TaskVisualState),
                typeof(TaskVisualStateEntities)
            ));
            
            var state = new TaskVisualState();
            foreach (var indicatorDefinition in ActiveIndicators)
            {
                indicatorDefinition.Prefab.PreparePrefab(conversionSystem);
                state.ActiveIndicators.Add(new IndicatorPrefabDefinition
                {
                    Prefab = indicatorDefinition.Prefab,
                    SeparatedLifecycle = indicatorDefinition.SeparatedLifecycle
                });
            }
            foreach (var indicatorDefinition in CompleteIndicators)
            {
                indicatorDefinition.Prefab.PreparePrefab(conversionSystem);
                state.CompleteIndicators.Add(new IndicatorPrefabDefinition
                {
                    Prefab = indicatorDefinition.Prefab,
                    SeparatedLifecycle = indicatorDefinition.SeparatedLifecycle
                });
            }
            dstManager.SetComponentData(entity, state);
        }
        
        [Serializable]
        public struct IndicatorDefinition
        {
            public PrefabEntity Prefab;
            public bool SeparatedLifecycle;
        }
    }
    
    public struct TaskVisualState : IComponentData
    {
        public List<IndicatorPrefabDefinition> ActiveIndicators;
        public List<IndicatorPrefabDefinition> CompleteIndicators;
    }
    
    public struct IndicatorPrefabDefinition
    {
        public Entity Prefab;
        public bool SeparatedLifecycle;
    }
    
    public struct TaskVisualStateEntities : IComponentData
    {
        public List<Entity> Indicators;
    }
}