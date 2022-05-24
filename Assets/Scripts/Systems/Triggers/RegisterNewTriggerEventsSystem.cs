using DOTSTemplate;
using Unity.Entities;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderFirst = true)]
    public partial class RegisterNewTriggerEventsSystem : SystemBase
    {
        private TriggerSystem triggerSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            triggerSystem = World.GetOrCreateSystem<TriggerSystem>();
        }
        
        protected override void OnUpdate()
        {
            var triggerEvents = triggerSystem.TriggerEvents;
            Entities.WithAll<Enter>().ForEach((Entity entity, in TriggerEvent contact) =>
            {
                triggerEvents[new TriggerEvent(contact.Trigger, contact.Source)] = entity;
            }).Run();
        }
    }
}