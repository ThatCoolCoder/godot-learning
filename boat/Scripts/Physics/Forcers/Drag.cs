using Godot;

namespace Physics.Forcers
{
    public class Drag : AbstractFluidForcer
    {
        [Export] public Curve AoaToCoefficient { get; set; }
        [Export] public Curve AoaToAreaProportion { get; set; }

        [Export] public float AreaMultiplier = 1;

        public override Vector2 CalculateForce()
        {
            var fluidVelocity = target.Fluid.VelocityAtPoint(Vector2.Zero);
            var velocity = target.GetPointVelocity(GlobalPosition) + fluidVelocity;
            var relativeVelocity = velocity - fluidVelocity;

            var aoa = GlobalRotation - relativeVelocity.Angle();
            var aoaProportion = Mathf.PosMod(aoa / Mathf.Tau, 1);

            var trueArea = AoaToAreaProportion.Interpolate(Utils.MirrorNumber(aoaProportion * 2, 1)) * AreaMultiplier;
            var dragCoefficient = AoaToCoefficient.Interpolate(Utils.MirrorNumber(aoaProportion * 2, 1));

            return 0.5f * dragCoefficient * trueArea * -(relativeVelocity.Normalized() * relativeVelocity.LengthSquared());
        }
    }
}