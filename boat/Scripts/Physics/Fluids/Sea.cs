using System;
using Godot;

namespace Physics.Fluids
{
    public class Sea : Spatial, ISpatialFluid
    {
        // Pretty ordinary sea. 
        // Currently has no waves.

        [Export] public float BaseDensity { get; set; } = 1.0f;
        [Export] public Vector3 Flow { get; set; } = Vector3.Zero;

        public float HeightAtPoint(Vector3 point)
        {
            return GlobalPosition.y;
        }

        public float DensityAtPoint(Vector3 point)
        {
            return BaseDensity;
        }

        public float VelocityAtPoint(Vector3 point)
        {
            return Flow;
        }
    }
}