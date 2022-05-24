using Unity.Entities;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(PresentationSystemGroup), OrderLast = true)]
    public class EndPresentationSyncPointGroup : ComponentSystemGroup
    {
        
    }
}