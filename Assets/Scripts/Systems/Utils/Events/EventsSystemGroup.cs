using Unity.Entities;

namespace DOTSTemplate
{
    [UpdateInGroup(typeof(EndPresentationSyncPointGroup))]
    public class EventsSystemGroup : ComponentSystemGroup
    {
        
    }
    
    [UpdateInGroup(typeof(EndPresentationSyncPointGroup))]
    [UpdateBefore(typeof(EventsSystemGroup))]
    public class PreEventsSystemGroup : ComponentSystemGroup
    {
        
    }

    [UpdateInGroup(typeof(EndPresentationSyncPointGroup))]
    [UpdateAfter(typeof(EventsSystemGroup))]
    public class PostEventsSystemGroup : ComponentSystemGroup
    {
        
    }
}