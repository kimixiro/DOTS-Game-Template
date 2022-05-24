using DOTSTemplate;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [AlwaysUpdateSystem]
    public partial class TriggerSystem : SystemBase
    {
        internal NativeHashMap<TriggerEvent, Entity> TriggerEvents;
        private EntityCommandBufferSystem entityCommandBufferSystem;
        private CollectTriggerEventsSystem collectTriggerEventsSystem;
        private EntityArchetype newTriggerEventArchetype;

        protected override void OnCreate()
        {
            base.OnCreate();
            TriggerEvents = new NativeHashMap<TriggerEvent, Entity>(32, Allocator.Persistent);
            entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            collectTriggerEventsSystem = World.GetOrCreateSystem<CollectTriggerEventsSystem>();
            newTriggerEventArchetype = EntityManager.CreateArchetype(
                typeof(TriggerEvent), typeof(Enter));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            TriggerEvents.Dispose();
        }

        protected override void OnUpdate()
        {
            var triggerEventEntities = TriggerEvents;
            var triggerEvents = collectTriggerEventsSystem.TriggerEvents;
            var commands = entityCommandBufferSystem.CreateCommandBuffer();
            var newContactArchetype = this.newTriggerEventArchetype;

            Dependency = Job.WithReadOnly(triggerEvents).WithCode(() =>
            {
                if (!triggerEventEntities.IsEmpty)
                {
                    using var triggerPairs = triggerEventEntities.GetKeyArray(Allocator.Temp);
                    foreach (var triggerPair in triggerPairs)
                    {
                        if (triggerEvents.Contains(triggerPair)) continue;
                        // Перекрытие закончилось
                        var triggerEventEntity = triggerEventEntities[triggerPair];
                        if (HasComponent<Exit>(triggerEventEntity))
                        {
                            // Если уже есть Exit, то просто удаляем Entity 
                            commands.DestroyEntity(triggerEventEntity);
                            triggerEventEntities.Remove(triggerPair);
                        }
                        else
                        {
                            // Если Exit еще нет, то добавляем и на всякий случай удаляем Enter
                            commands.RemoveComponent<Enter>(triggerEventEntity);
                            commands.AddComponent<Exit>(triggerEventEntity);
                        }
                    }
                }

                if (!triggerEvents.IsEmpty)
                {
                    using var events = triggerEvents.ToNativeArray(Allocator.Temp);
                    foreach (var triggerEvent in events)
                    {
                        if (!triggerEventEntities.ContainsKey(triggerEvent))
                        {
                            // Новое перекрытие, добавляем событие
                            var triggerEventEntity = commands.CreateEntity(newContactArchetype);
                            commands.SetComponent(triggerEventEntity, triggerEvent);
                            // Не получится выполнить здесь
                            // triggerEventEntities.Add(triggerEvent, triggerEventEntity);
                        }
                        else
                        {
                            // Перекрытие продолжается
                            var triggerEventEntity = triggerEventEntities[triggerEvent];
                            if (HasComponent<Enter>(triggerEventEntity))
                            {
                                // Если только что вошли в перекрытие, то убираем Enter
                                commands.RemoveComponent<Enter>(triggerEventEntity);
                            }
                            else if (HasComponent<Exit>(triggerEventEntity))
                            {
                                // Если только что вышли из перекрытия, то убираем Exit, снова добавляем Enter
                                commands.RemoveComponent<Exit>(triggerEventEntity);
                                commands.AddComponent<Enter>(triggerEventEntity);
                            }
                        }
                    }
                }
            }).Schedule(JobHandle.CombineDependencies(Dependency, collectTriggerEventsSystem.OutputDependency));
            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}