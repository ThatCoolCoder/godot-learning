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
            var entrySpeed = (GlobalTransform.basis.z * velocityRelativeToFluid).z;
            var effectiveExitSpeed = ThrustProportion * ExitSpeed;
            var deltaSpeed = effectiveExitSpeed - entrySpeed;
            GD.Print(entrySpeed);

            var area = Mathf.Pi * Radius * Radius;

            var force = 0.5f * density * area * deltaSpeed;

            return GlobalTransform.basis.z * force;
        }
    }
}