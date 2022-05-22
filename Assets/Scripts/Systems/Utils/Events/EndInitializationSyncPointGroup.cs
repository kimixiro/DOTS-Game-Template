using Unity.Entities;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public class EndInitializationSyncPointGroup : ComponentSystemGroup
    {
        
    }
}