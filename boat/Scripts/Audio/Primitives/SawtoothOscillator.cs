using Godot;
using System;

namespace Audio.Primitives
{
    public class SawtoothOscillator : AbstractOscillator
    {
        protected override float OscillatorValue(float phase) => Mathf.PosMod(phase, 1) * 2 - 1;
    }
}