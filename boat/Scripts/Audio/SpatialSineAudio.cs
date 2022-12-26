using Godot;
using System;

namespace Audio
{
    public class SpatialSineAudio : ProceduralSpatialAudio
    {
        // Basic audio thing to test that it works

        private Primitives.SineOscillator oscillator = new();
        private float phase;

        protected override Vector2 ComputeAudioValue()
        {
            return Vector2.One * 0.5f * oscillator.Sample(sampleHz);
        }
    }
}