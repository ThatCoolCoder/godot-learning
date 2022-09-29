using System;
using Godot;

namespace Physics
{
    public class SpatialFluidEffectable : RigidBody
    {
        public Fluids.ISpatialFluid Fluid { get; set; }

        public Vector3 VelocityAtPoint(Vector3 point)
        {
            // Get the global velocity of a specific point on this, taking into account the rotation
            // Todo: find the code for this when I can access reddit

            return LinearVelocity;
        }
    }
}