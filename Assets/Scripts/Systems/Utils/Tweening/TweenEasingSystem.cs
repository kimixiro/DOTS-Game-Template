using Unity.Entities;

namespace DOTSTemplate.Tweening
{
    [UpdateInGroup(typeof(TweenSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(TweenProgressSystem))]
    [UpdateAfter(typeof(TweenLoopProgressSystem))]
    public partial class TweenEasingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<Delay>()
                .ForEach((Entity entity, ref TweenProgress progress, in Easing easing) =>
                {
                    progress.NormalizedTime = Easings.Interpolate(progress.NormalizedTime, easing.Type);
                }).Schedule();
        }
    }
}