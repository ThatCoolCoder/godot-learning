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
        [Export] public bool FreeWheelWhenOff { get; set; } // If this is true, motor will not generate any drag when at 0 thrustproportion

        public override Vector3 CalculateForce(ISpatialFluid fluid)
        {
            if (ThrustProportion == 0 && FreeWheelWhenOff) return Vector3.Zero;

            var density = fluid.DensityAtPoint(GlobalTransform.origin);
            var velocityRelativeToFluid = target.VelocityAtPoint(GlobalTranslation) - fluid.VelocityAtPoint(GlobalTransform.origin);
            var entrySpeed = (GlobalTransform.basis.z * velocityRelativeToFluid).z;
            var effectiveExitSpeed = ThrustProportion * ExitSpeed;
            var deltaSpeed = effectiveExitSpeed - entrySpeed;

            var area = Mathf.Pi * Radius * Radius;

            var force = 0.5f * density * area * deltaSpeed;

            return GlobalTransform.basis.z * force;
        }
    }
}