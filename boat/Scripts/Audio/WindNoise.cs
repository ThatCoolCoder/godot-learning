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
        private float leftAudioFactor;
        private float rightAudioFactor;

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
            baseValue *= VolumeMultiplier * periodicVolumeMultiplier;

            return new Vector2(leftAudioFactor * baseValue, rightAudioFactor * baseValue);
        }

        public override void _Process(float delta)
        {
            var globalVelocity = (GlobalTranslation - previousPosition) / delta;
            var relativeVelocity = globalVelocity - air.VelocityAtPoint(GlobalTranslation);
            var localVelocity = GlobalTransform.basis.XformInv(relativeVelocity);

            // Might as well calculate all this in process and not the audio value computer since velocity only makes sense to update that fast anyway
            airSpeed = localVelocity.Length();
            var airDirection = new Vector2(localVelocity.x, localVelocity.z).Angle();
            airDirection += Mathf.Pi / 2;
            leftAudioFactor = Mathf.Sin(-airDirection) / 2 + 0.5f;
            rightAudioFactor = Mathf.Sin(airDirection) / 2 + 0.5f;

            // GlobalTranslation = GetParent<Spatial>().GlobalTranslation + relativeVelocity.Normalized();

            previousPosition = GlobalTranslation;
            base._Process(delta);
        }
    }
}