using Godot;
using System;

namespace Audio
{
    public abstract class ProceduralSpatialAudio : AudioStreamPlayer3D
    {
        protected AudioStreamGenerator generator = new();
        protected AudioStreamGeneratorPlayback playback;

        protected static float sampleHz { get; private set; } = 44100;

        // time in seconds
        protected float time { get; private set; } = 0;

        public override void _Ready()
        {
            generator.MixRate = sampleHz;
            Stream = generator;
            playback = GetStreamPlayback() as AudioStreamGeneratorPlayback;
            FillBuffer();
            Play();
        }

        protected abstract Vector2 ComputeAudioValue();

        public override void _Process(float delta)
        {
            FillBuffer();
            // time += delta;
        }

        private void FillBuffer()
        {
            var toFill = playback.GetFramesAvailable();
            for (int i = 0; i < toFill; i++)
            {
                playback.PushFrame(ComputeAudioValue());
                time += 1 / sampleHz;
            }
        }
    }
}