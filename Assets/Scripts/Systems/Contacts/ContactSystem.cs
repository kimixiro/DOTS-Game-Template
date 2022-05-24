using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
    [AlwaysUpdateSystem]
    public partial class ContactSystem : SystemBase
    {
        private EntityArchetype newContactArchetype;
        private EntityCommandBufferSystem entityCommandBufferSystem;
        private CollectCollisionsSystem collectCollisionsSystem;
        internal NativeHashMap<EntityPair, Entity> Contacts;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            entityCommandBufferSystem = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            collectCollisionsSystem = World.GetOrCreateSystem<CollectCollisionsSystem>();
            Contacts = new NativeHashMap<EntityPair, Entity>(128, Allocator.Persistent);
            
            newContactArchetype = EntityManager.CreateArchetype(typeof(Contact), typeof(Enter));
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Contacts.Dispose();
        }
        
        protected override void OnUpdate()
        {
            var contacts = this.Contacts;
            var collisions = collectCollisionsSystem.Collisions;
            
            var commands = entityCommandBufferSystem.CreateCommandBuffer();
            var newContactArchetype = this.newContactArchetype;
            Dependency = Job.WithCode(() =>
            {
                if (!contacts.IsEmpty)
                {
                    using var contactKeys = contacts.GetKeyArray(Allocator.Temp);
                    foreach (var contactKey in contactKeys)
                    {
                        if (collisions.ContainsKey(contactKey)) continue;
                        // Столкновение прекратилось
                        var contactEntity = contacts[contactKey];
                        if (HasComponent<Exit>(contactEntity))
                        {
                            // Если уже есть Exit, то просто удаляем Entity 
                            commands.DestroyEntity(contactEntity);
                            contacts.Remove(contactKey);
                        } 
                        else
                        {
                            // Если Exit еще нет, то добавляем и на всякий случай удаляем Enter
                            commands.RemoveComponent<Enter>(contactEntity);
                            commands.AddComponent<Exit>(contactEntity);
                        }
                    }
                }
                if (!collisions.IsEmpty)
                {
                    using var collisionsKeys = collisions.GetKeyArray(Allocator.Temp);
                    foreach (var collisionsKey in collisionsKeys)
                    {
                        if (!contacts.ContainsKey(collisionsKey))
                        {
                            // Новое столкновение, добавляем контакт
                            var contactEntity = commands.CreateEntity(newContactArchetype);
                            commands.SetComponent(contactEntity, new Contact
                            {
                                EntityPair = collisionsKey,
                                CollisionData = collisions[collisionsKey]
                            });
                            // Не получится выполнить здесь
                            // contacts.Add(collisionsKey, contactEntity);
                        }
                        else
                        {
                            // Столкновение продолжается
                            var contactEntity = contacts[collisionsKey];
                            if (HasComponent<Enter>(contactEntity))
                            {
                                // Если только что вошли в контакт, то убираем Enter
                                commands.RemoveComponent<Enter>(contactEntity);
                            } 
                            else if (HasComponent<Exit>(contactEntity))
                            {
                                // Если только что вышли из контакта, то убираем Exit, снова добавляем Enter
                                commands.RemoveComponent<Exit>(contactEntity);
                                commands.AddComponent<Enter>(contactEntity);
                            }
                        }
                    }
                }
            }).Schedule(JobHandle.CombineDependencies(collectCollisionsSystem.OutputDependency, Dependency));
            
            entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}