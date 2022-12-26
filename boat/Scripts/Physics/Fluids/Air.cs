using Godot;
using System;

namespace Physics.Fluids
{
    public class Air : Spatial, ISpatialFluid
    {
        // Air, supports basic wind with gusting and direction changing

        [Export] public float DensityMultiplier { get; set; } = 1;

        [Export] public float Speed { get; set; } = 0;
        [Export] public float GustSpeedDelta { get; set; } = 0; // max gust speed = speed + gust speed delta
        [Export] public float GustFrequency { get; set; } = 1;
        [Export]
        public float DirectionDegrees
        {
            get
            {
                return Mathf.Rad2Deg(direction);
            }
            set
            {
                direction = Mathf.Deg2Rad(value);
            }
        }
        private float direction = 0;
        [Export]
        public float DirectionVariabilityDegrees // from 0 to 180
        {
            get
            {
                return Mathf.Rad2Deg(directionVariability);
            }
            set
            {
                directionVariability = Mathf.Deg2Rad(value);
            }
        }
        private float directionVariability = 0;
        [Export] public float DirectionChangeFrequency = 1;

        private OpenSimplexNoise speedNoise = new();
        private OpenSimplexNoise directionNoise = new();
        private float time = 0;

        public override void _Ready()
        {
            speedNoise.Seed = 25;
            speedNoise.Persistence = 0.75f;
        }

        public override void _PhysicsProcess(float delta)
        {
            time += delta;
        }

        public float DensityAtPoint(Vector3 _point)
        {
            return 1.293f * DensityMultiplier;
        }

        public float HeightAtPoint(Vector3 _point)
        {
            return 0;
        }

        public Vector3 VelocityAtPoint(Vector3 _point)
        {
            var gustSpeed = (speedNoise.GetNoise1d(time * GustFrequency) / 2 + 0.5f) * GustSpeedDelta;
            var finalSpeed = Speed + gustSpeed;

            var directionDelta = (directionNoise.GetNoise1d(time * DirectionChangeFrequency)) * directionVariability;
            var finalDirection = direction + directionDelta;

            var flow = new Vector3(finalSpeed, 0, 0).Rotated(Vector3.Up, finalDirection);

            return flow;
        }

        public Vector3 NormalAtPoint(Vector3 _point)
        {
            return Vector3.Up;
        }

        public FluidType Type { get; set; } = FluidType.Gas;
    }
}