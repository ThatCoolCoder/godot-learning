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
            var localVel = ToLocal(velocityRelativeToFluid);
            var exitVelocity = GlobalTransform.basis.z * -ExitSpeed * ThrustProportion;
            var trueExitSpeed = exitVelocity.z;

            var area = Mathf.Pi * Radius * Radius;

            // F = .5 * r * A * [Ve ^2 - V0 ^2] 

            // var force = 0.5 * density * area * (exitVelocity *)

            // var massFlowRate = density * area * airVelocity;
            return GlobalTransform.basis.z * -trueExitSpeed;
        }
    }
}