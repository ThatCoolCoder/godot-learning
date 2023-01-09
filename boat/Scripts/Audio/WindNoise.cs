using Godot;
using System;

namespace Audio
{
    public class WindNoise : ProceduralSpatialAudio
    {
        // sorry this is very messy due to the process of experimentation - it's basically an auditory shader, what did you expect?

        [Export] public float VolumeMultiplier { get; set; } = 0.1f;
        [Export] public NodePath AirPath { get; set; }
        private Physics.Fluids.Air air;

        private OpenSimplexNoise volumeNoise = new();
        private float volumeNoiseFrequency = 150;
        private float volumeNoiseFactor = 0.25f;

        private OpenSimplexNoise pinkNoise = new();

        private float approximatePitch = 50;

        private Vector3 previousPosition;
        private float airSpeed = 0;

        private Primitives.SineOscillator sineWave = new() { Frequency = 500 };
        private Primitives.SquareOscillator saw = new() { Frequency = 500 };
        private float sineWaveFactor = 0.05f;

        public override void _Ready()
        {
            air = GetNode<Physics.Fluids.Air>(AirPath);
            base._Ready();

            previousPosition = GetParent<Spatial>().GlobalTranslation;

            volumeNoise.Octaves = 1;

            pinkNoise.Octaves = 5;
            pinkNoise.Lacunarity = 2;
            pinkNoise.Persistence = 0.25f;
        }

        protected override Vector2 ComputeAudioValue()
        {
            var baseValue = pinkNoise.GetNoise1d(time * 50000) / 2 + 0.5f;
            var periodicVolumeMultiplier = 1 + volumeNoise.GetNoise1d(volumeNoiseFrequency * time) * volumeNoiseFactor;
            baseValue *= airSpeed;
            return Vector2.One * baseValue * VolumeMultiplier * periodicVolumeMultiplier;
        }

        public override void _Process(float delta)
        {
            var globalVelocity = (GetParent<Spatial>().GlobalTranslation - previousPosition) / delta;
            var relativeVelocity = globalVelocity - air.VelocityAtPoint(GetParent<Spatial>().GlobalTranslation);
            var localVelocity = GlobalTransform.basis.XformInv(relativeVelocity);

            airSpeed = localVelocity.Length();

            GlobalTranslation = GetParent<Spatial>().GlobalTranslation + relativeVelocity.Normalized();

            previousPosition = GetParent<Spatial>().GlobalTranslation;
            base._Process(delta);
        }
    }
}