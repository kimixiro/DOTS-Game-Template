using System;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSTemplate
{
    [Serializable]
    public struct Remapping
    {
        public Vector2 Source;
        public Vector2 Destination;

        public Remapping(float s0, float s1, float d0, float d1) : this()
        {
            Source = new Vector2(s0, s1);
            Destination = new Vector2(d0, d1);
        }
        
        public float Remap(float value)
        {
            return math.remap(Source.x, Source.y, 
                Destination.x, Destination.y, value);
        }
        
        public float RemapClamped(float value)
        {
            return math.clamp(math.remap(Source.x, Source.y, 
                    Destination.x, Destination.y, value), 
                Destination.x, Destination.y);
        }
        
        public float RemapInversed(float value)
        {
            return math.remap(Destination.x, Destination.y, 
                Source.x, Source.y, value);
        }
        
        public float RemapInversedClamped(float value)
        {
            return math.clamp(math.remap(Destination.x, Destination.y, 
                    Source.x, Source.y, value), 
                Source.x, Source.y);
        }
    }
}