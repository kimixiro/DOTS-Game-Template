using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace DOTSTemplate.Events
{
    public abstract partial class EventBufferSystem<TEvent> : SystemBase where TEvent : struct
    {
        private List<EventBuffer> buffers = new List<EventBuffer>();
        private JobHandle producerJobs;
        
        public EventBuffer CreateBuffer()
        {
            var buffer = new EventBuffer(Allocator.TempJob);
            buffers.Add(buffer);
            return buffer;
        }

        public void AddProducerJob(JobHandle jobHandle)
        {
            producerJobs = JobHandle.CombineDependencies(producerJobs, jobHandle);
        }
        
        protected abstract void Handle(TEvent @event);

        protected override void OnUpdate()
        {
            try
            {
                if (buffers.Count == 0) return;
                producerJobs.Complete();

                foreach (var eventBuffer in buffers)
                {
                    while (eventBuffer.events.TryDequeue(out var @event))
                    {
                        Handle(@event);
                    }
                    eventBuffer.Dispose();
                }
            }
            finally
            {
                buffers.Clear();
                producerJobs = default;
            }
        }
        
        public struct EventBuffer : IDisposable
        {
            internal NativeQueue<TEvent> events;
            
            public NativeQueue<TEvent>.ParallelWriter AsParallelWriter => events.AsParallelWriter();
            
            internal EventBuffer(Allocator allocator)
            {
                events = new NativeQueue<TEvent>(allocator);
            }
            
            public void Enqueue(TEvent @event)
            {
                events.Enqueue(@event);
            }
            
            public void Dispose()
            {
                events.Dispose();
            }
        }        
    }
}