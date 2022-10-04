using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public class Motor : AbstractSpatialFluidForcer
    {
        [Export] public float Radius { get; set; }
        [Export] public float ExitSpeed { get; set; }
        [Export] public float ThrustProportion { get; set; } // -1 to +1

        public override Vector3 CalculateForce(ISpatialFluid fluid)
        {
            var density = fluid.DensityAtPoint(GlobalTransform.origin);
            var velocityRelativeToFluid = target.LinearVelocity - fluid.VelocityAtPoint(GlobalTransform.origin);
            var entryVelocity = 0;
            // var entryVelocity = ToLocal(velocityRelativeToFluid).z;

            var area = Mathf.Pi * Radius * Radius;

            var force = 0.5f * density * area * (ExitSpeed * ExitSpeed - entryVelocity * entryVelocity);
            force *= Mathf.Sign(ThrustProportion) * ThrustProportion * ThrustProportion;

            return GlobalTransform.basis.z * force;
        }
    }
}