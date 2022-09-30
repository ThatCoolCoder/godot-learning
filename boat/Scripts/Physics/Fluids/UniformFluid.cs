using System;
using Godot;

namespace Physics.Fluids
{
    public class UniformFluid : IFluid
    {
        [Export] public float Density { get; set; } = 1;
        [Export] public Vector2 Velocity { get; set; } = Vector2.Zero;

        public float DensityAtPoint(Vector2 point)
        {
            return Density;
        }

        public Vector2 VelocityAtPoint(Vector2 point)
        {
            return Velocity;
        }

        public FluidType Type { get; set; } = FluidType.Liquid;
    }
}