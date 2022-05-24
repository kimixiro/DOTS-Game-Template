using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace DOTSTemplate.Tweening
{
    [UpdateInGroup(typeof(TweenSystemGroup))]
    public partial class TranslationTweenSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithNone<Delay>()
                .ForEach((in TweenProgress progress, in TranslationTween scaleTween, in Target target) =>
                {
                    if (progress.TargetDestroyed) return;
                    var scale = GetComponent<Translation>(target.Entity);
                    scale.Value = math.lerp(scaleTween.StartValue, scaleTween.EndValue,
                        progress.NormalizedTime);
                    SetComponent(target.Entity, scale);
                }).Schedule();
        }
    }
}