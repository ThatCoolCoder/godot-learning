using Godot;
using System;

namespace Audio.Primitives
{
    public class SquareOscillator : AbstractOscillator
    {
        protected override float OscillatorValue(float phase) => phase < 0.5f ? -1 : 1;
    }
}