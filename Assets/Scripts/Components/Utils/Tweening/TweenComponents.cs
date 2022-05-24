using Unity.Entities;

namespace DOTSTemplate.Tweening
{
    
    public struct Target : IComponentData
    {
        public Entity Entity;
    }

    public struct Duration : IComponentData
    {
        public float Value;
    }

    public struct TweenProgress : IComponentData
    {
        public float Time;
        public float NormalizedTime;
        public bool TargetDestroyed;
    }

    public struct Delay : IComponentData
    {
        public float Value;
    }

    public struct Loop : IComponentData
    {
        public int Count;
        public LoopingType Type;
    }

    public enum LoopingType
    {
        Restart,
        Yoyo
    }
    
    public struct Easing : IComponentData
    {
        public EasingType Type;
    }

}