using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public class AeroSurface : AbstractSpatialFluidForcer
    {
        [Export] public Curve LiftCoefficient { get; set; }
        [Export] public Curve DragCoefficient { get; set; }
        [Export] public float Area { get; set; }

        public override Vector3 CalculateForce(ISpatialFluid fluid)
        {
            return Vector3.Zero;
        }
    }
}