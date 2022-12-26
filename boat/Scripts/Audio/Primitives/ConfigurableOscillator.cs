using Godot;
using System;

namespace Audio.Primitives
{
    public class ConfigurableOscillator : AbstractOscillator
    {
        public Func<float, float> OscillatorFunc { get; set; } = Mathf.Sin;

        protected override float OscillatorValue(float phase) => OscillatorFunc(phase);
    }
}