using Godot;
using System;

namespace Audio.Primitives
{
    public abstract class AbstractOscillator
    {
        public float Frequency { get; set; } = 440;
        private float phase = 0;

        protected abstract float OscillatorValue(float phase);

        public float Sample(float sampleHz)
        {
            var val = OscillatorValue(phase * Mathf.Tau);
            var increment = Frequency / sampleHz;
            phase = Mathf.PosMod(phase + increment, 1);
            return val;
        }
    }
}