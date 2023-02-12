using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public class AeroSurface : AbstractSpatialFluidForcer
    {
        // Basically a wing, not restricted to operating in air though.
        // Uses a model supporting lift, induced drag, and parasitic drag.
        // Wing is oriented along the x/z plane, and works equally well when moved along that plane.
        // Area/size is determined by the scale of the entity in the scene, although there is also an area multiplier.
        // The multiplier's main use is to allow you to scale this to match size of a non-square object but still have correct area. EG use 0.5 for a triangle.

        // Angle of attack ranges from 0 to tau, and should be normalised to the correct range by wrapping around.

        // Curves have values from 0 to 1, where 1 is a quarter turn of positive aoa. Remaining values are interpolated by "mirroring" the curve.
        [Export] public Curve TotalLiftCoefficient { get; set; } // Total lift points perpendicular to surface and causes both true lift and induced drag
        [Export] public Curve ParasiticDragCoefficient { get; set; }
        [Export] public float AreaMultiplier { get; set; } = 1;


        private float area
        {
            get
            {
                return Scale.x * Scale.z * AreaMultiplier;
            }
        }


        // Thickness of the wing - used for parasitic drag calculations
        [Export] public float Thickness { get; set; }

        public override void _Ready()
        {
            GetNode<Spatial>("CSGBox").Visible = false;

            base._Ready();
        }

        public override Vector3 CalculateForce(ISpatialFluid fluid, PhysicsDirectBodyState state)
        {
            var density = fluid.DensityAtPoint(GlobalTranslation);
            var relativeVelocity = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalTranslation);

            var basis = GlobalTransform.basis;
            basis.Scale = Vector3.One;

            // Velocity relative to the rotation of self
            var localVelocity = basis.XformInv(relativeVelocity);
            var localSpeedSquared = localVelocity.LengthSquared();

            var aoa = CalculateAngleOfAttack(localVelocity);
            var liftCoefficient = InterpolateFromQuarterAoaCurve(TotalLiftCoefficient, aoa);
            var parasiticDragCoefficient = InterpolateFromQuarterAoaCurve(ParasiticDragCoefficient, aoa);

            var liftMag = CalculateAeroForceMagnitude(liftCoefficient, area, density, localSpeedSquared);
            var frontalArea = CalculateFrontalArea(aoa);
            var dragMag = CalculateAeroForceMagnitude(parasiticDragCoefficient, frontalArea, density, localSpeedSquared);

            var liftVector = basis.y * liftMag * (HasPositiveAoa(aoa) ? 1 : -1);
            var dragVector = -relativeVelocity.Normalized() * dragMag;

            return liftVector + dragVector;
        }

        private float CalculateAngleOfAttack(Vector3 localVelocity)
        {
            // Probably could do this better with quaternions but this works...
            var tr = new Transform().LookingAt(localVelocity, Vector3.Up);
            var euler = tr.basis.GetEuler();
            return Utils.WrapNumber(-euler.x, 0, Mathf.Tau);
        }

        private float CalculateAeroForceMagnitude(float coefficient, float area, float density, float speedSquared)
        {
            // Since lift and drag equations are so similar, we can have one method for calculating both.

            return coefficient * area * density * speedSquared / 2;
        }

        private float InterpolateFromQuarterAoaCurve(Curve curve, float aoa)
        {
            // Interpolate a value, by presuming that the curve is "symmetric".
            // Presumes that the aoa has already been normalised
            // possible optimisation: move the curve lookup out of here and make this only mirroring,
            // so we only do the mirroring once

            // Mapping:
            // 0-90 -> 0-90
            // 90-180 -> 90-0
            // 180-270 -> 0-90
            // 270-360->90-0

            float deg90 = Mathf.Pi / 2;
            float deg180 = Mathf.Pi;
            float deg270 = deg90 + deg180;

            float trueAoa = 0;
            if (aoa < deg90) trueAoa = aoa;
            else if (aoa < deg180) trueAoa = deg90 - (aoa - deg90);
            else if (aoa < deg270) trueAoa = aoa - deg180;
            else trueAoa = deg90 - (aoa - deg270);


            return curve.Interpolate(AoaToCurveValue(trueAoa));
        }

        private float CalculateFrontalArea(float aoa)
        {
            // Calculate the frontal area for a given aoa, for parastic drag calculations

            // Make there be 1 axis of symmetry
            if (aoa > Mathf.Pi) aoa -= Mathf.Pi;

            var frontLipThickness = Thickness * Mathf.Cos(aoa);
            var frontLipArea = Mathf.Abs(frontLipThickness * Mathf.Sqrt(area));
            var bodyArea = Mathf.Sin(aoa) * area;

            return frontLipArea + bodyArea;
        }

        private float AoaToCurveValue(float aoa)
        {
            return aoa / Mathf.Tau * 4;
        }

        private bool HasPositiveAoa(float aoa)
        {
            // Checks if the lift vector should point up or down depending on the AOA
            return (aoa < Mathf.Pi / 2 || aoa > Mathf.Pi && aoa < Mathf.Pi * 1.5f);
        }
    }
}
