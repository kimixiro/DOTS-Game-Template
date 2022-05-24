using DOTSTemplate;
using Unity.Entities;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class MissionIndicatorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithNone<Active>().WithAll<Enter, Trigger, Mission>()
                .ForEach((in MissionIndicator indicator) =>
                {
                    indicator.SetState(true);
                }).WithoutBurst().Run();
            Entities.WithAll<Exit, Trigger, Mission>()
                .ForEach((in MissionIndicator indicator) =>
                {
                    indicator.SetState(false);
                }).WithoutBurst().Run();
        }
    }
}