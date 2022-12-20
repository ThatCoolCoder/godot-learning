using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
    public class AeroSurface : AbstractSpatialFluidForcer
    {
        // Basically a wing, not restricted to operating in air though.
        // Uses a model supporting lift, induced drag, and parasitic drag.

        // Angle of attack ranges from 0 to tau, and should be normalised to the correct range by wrapping around.

        // Curves have values from 0 to half pi radians of AOA. Remaining values are interpolated by "mirroring" the curve.
        [Export] public Curve TotalLiftCoefficient { get; set; } // Total lift points perpendicular to surface and causes both true lift and induced drag
        [Export] public Curve ParasiticDragCoefficient { get; set; }
        // Area of the wing, when viewed frmo above.
        [Export] public float Area { get; set; }

        // Thickness of the wing - used for parasitic drag calculations
        [Export] public float Thickness { get; set; }
        // Also used for parasitic drag calculations, but optional
        [Export] public float WingSpan { get; set; }
        [Export] public bool UseWingSpan { get; set; }

        public override Vector3 CalculateForce(ISpatialFluid fluid)
        {
            var density = fluid.DensityAtPoint(GlobalTranslation);
            var relativeVelocity = target.VelocityAtPoint(GlobalTranslation) - fluid.VelocityAtPoint(GlobalTranslation);

            // todo: I have no clue if this is how you get a velocity into local rotation.
            var localVelocity = GlobalTransform.basis.z * relativeVelocity;
            var localSpeedSquared = localVelocity.LengthSquared();

            var aoa = CalculateAngleOfAttack(localVelocity);
            var liftCoefficient = InterpolateFromQuarterAoaCurve(TotalLiftCoefficient, aoa);
            var parasiticDragCoefficient = InterpolateFromQuarterAoaCurve(ParasiticDragCoefficient, aoa);

            var liftMag = CalculateAeroForceMagnitude(liftCoefficient, Area, density, localSpeedSquared);
            var frontalArea = CalculateFrontalArea(aoa);
            var dragMag = CalculateAeroForceMagnitude(parasiticDragCoefficient, frontalArea, density, localSpeedSquared);

            // todo: mult both mags by normal and inverse flow, add together, return.

            return Vector3.Zero;
        }

        private float CalculateAngleOfAttack(Vector3 localVelocity)
        {
            // todo: code this
            return Mathf.Deg2Rad(15);
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
            // possible optimisation: move the curve lookup out of here,
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
            else trueAoa = 90 - (aoa - deg270);

            return curve.Interpolate(trueAoa);
        }

        private float CalculateFrontalArea(float aoa)
        {
            // Calculate the frontal area for a given aoa, for parastic drag calculations

            // Make there be 1 axis of symmetry
            if (aoa > Mathf.Pi) aoa -= Mathf.Pi;

            var frontLipThickness = Thickness * Mathf.Cos(aoa);
            var frontLipArea = frontLipThickness * Mathf.Sqrt(Area);
            var bodyArea = Mathf.Sin(aoa) * Area;

            return frontLipArea + bodyArea;
        }
    }
}