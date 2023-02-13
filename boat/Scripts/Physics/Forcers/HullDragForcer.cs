using Godot;
using System;

namespace Physics.Forcers
{
    public class HullDragForcer : AbstractSpatialFluidForcer
    {
        // Thing that calculates drag in x and z dimensions (because buoyancy forcer does that in Y)

        [Export] public DragCube DragCube { get; set; } = DragCube.LoadDefault();

        public override Vector3 CalculateForce(Fluids.ISpatialFluid fluid, PhysicsDirectBodyState state)
        {
            // var density = fluid.DensityAtPoint(GlobalTranslation);
            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalTranslation);

            var basis = GlobalTransform.basis;
            basis.Scale = Vector3.One;

            // Velocity relative to the rotation of self
            var localVelocity = basis.XformInv(relativeVelocity);

            return Vector3.Zero;
        }
    }
}
