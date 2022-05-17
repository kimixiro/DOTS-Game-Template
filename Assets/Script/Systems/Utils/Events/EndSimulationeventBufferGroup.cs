using Unity.Entities;

namespace DOTSTemplate.Events
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    public class EndSimulationEventBufferGroup : ComponentSystemGroup
    {
        
    }
}