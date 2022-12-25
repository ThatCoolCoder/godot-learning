using Godot;
using System;

namespace Physics.Fluids
{
    public class Air : Spatial, ISpatialFluid
    {
        [Export] public Vector3 Flow { get; set; } = Vector3.Zero;

        public float DensityAtPoint(Vector3 _point)
        {
            return 1.293f;
        }

        public float HeightAtPoint(Vector3 _point)
        {
            return 0;
        }

        public Vector3 VelocityAtPoint(Vector3 _point)
        {
            return Flow;
        }

        public Vector3 NormalAtPoint(Vector3 _point)
        {
            return Vector3.Up;
        }

        public FluidType Type { get; set; } = FluidType.Gas;
    }
}