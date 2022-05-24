using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTSTemplate.Tweening
{
    [UpdateInGroup(typeof(TweenSystemGroup))]
    public partial class RotationTweenSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<Delay>()
                .ForEach((in TweenProgress progress, in Target target, in RotationTween scaleTween) =>
                {
                    if (progress.TargetDestroyed) return;
                    var scale = GetComponent<Rotation>(target.Entity);
                    scale.Value = quaternion.Euler(math.lerp(scaleTween.StartValue, scaleTween.EndValue,
                        progress.NormalizedTime));
                    SetComponent(target.Entity, scale);
                }).Schedule();
        }
    }
}