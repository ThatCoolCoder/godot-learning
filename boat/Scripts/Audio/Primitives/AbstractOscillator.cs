using Godot;
using System;

namespace Audio.Primitives
{
    public abstract class AbstractOscillator
    {
        public float Frequency { get; set; } = 440;
        private float phase = 0;

        // Phase ranges from 0 to 1, where the period beteween 0 to 1 is 1/frequency.
        // Should return a value from -1 to 1
        protected abstract float OscillatorValue(float phase);

        public float Sample(float sampleHz)
        {
            var val = OscillatorValue(phase);
            var increment = Frequency / sampleHz;
            phase = Mathf.PosMod(phase + increment, 1);
            return val;
        }
    }
}