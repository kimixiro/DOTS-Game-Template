using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTSTemplate.Tweening
{
    [UpdateInGroup(typeof(TweenSystemGroup))]
    public partial class ScaleTweenSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<Delay>()
                .ForEach((in TweenProgress progress, in Target target, in ScaleTween scaleTween) =>
                {
                    if (progress.TargetDestroyed) return;
                    var scale = GetComponent<NonUniformScale>(target.Entity);
                    scale.Value = math.lerp(scaleTween.StartValue, scaleTween.EndValue,
                        progress.NormalizedTime);
                    SetComponent(target.Entity, scale);
                }).Schedule();
        }
    }
}