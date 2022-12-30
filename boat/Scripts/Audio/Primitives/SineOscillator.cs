using Godot;
using System;

namespace Audio.Primitives
{
    public class SineOscillator : AbstractOscillator
    {
        protected override float OscillatorValue(float phase) => Mathf.Sin(phase * Mathf.Tau);
    }
}