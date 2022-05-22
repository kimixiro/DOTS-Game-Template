using DOTSTemplate.Tweening;
using Unity.Entities;
using Unity.Mathematics;

namespace DOTSTemplate.Tasks
{
    [UpdateInGroup(typeof(EndInitializationSyncPointGroup))]
    public partial class TouchTaskIndicatorSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithNone<Active>().ForEach((Entity entity, in TouchTaskIndicator indicator) =>
            {
                EntityManager.AddComponent<Active>(entity);
                
                EntityManager.TweenTranslation(entity, 
                        new float3(0, indicator.VerticalOffset, 0), 
                        indicator.VerticalOffsetDuration)
                    .Looped(-1, LoopingType.Yoyo)
                    .WithEasing(EasingType.SineEaseInOut)
                    .SetOffset(true);
                
                EntityManager.TweenRotation(entity, 
                        new float3(0,math.PI,0), 
                        indicator.RotationDuration)
                    .Looped(-1, LoopingType.Restart)
                    .From(float3.zero);
                
            }).WithStructuralChanges().WithoutBurst().Run();
        }
    }
}