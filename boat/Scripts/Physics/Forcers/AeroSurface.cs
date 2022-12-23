using Godot;
using Physics.Fluids;
using System;

namespace Physics.Forcers
{
	public class AeroSurface : AbstractSpatialFluidForcer
	{
		// Basically a wing, not restricted to operating in air though.
		// Uses a model supporting lift, induced drag, and parasitic drag.
		// Wing is oriented along the , and works symmetrically in all directions.

		// Angle of attack ranges from 0 to tau, and should be normalised to the correct range by wrapping around.

		// Curves have values from 0 to 1, where 1 is a quarter turn of positive aoa. Remaining values are interpolated by "mirroring" the curve.
		[Export] public Curve TotalLiftCoefficient { get; set; } // Total lift points perpendicular to surface and causes both true lift and induced drag
		[Export] public Curve ParasiticDragCoefficient { get; set; }
		// Area of the wing, when viewed frmo above.
		[Export] public float Area { get; set; }

		// Thickness of the wing - used for parasitic drag calculations
		[Export] public float Thickness { get; set; }

		public override Vector3 CalculateForce(ISpatialFluid fluid)
		{
			var density = fluid.DensityAtPoint(GlobalTranslation);
			var relativeVelocity = target.VelocityAtPoint(GlobalTranslation) - fluid.VelocityAtPoint(GlobalTranslation);

			// Velocity relative to the rotation of self
			var localVelocity = GlobalTransform.basis.XformInv(relativeVelocity);
			var localSpeedSquared = localVelocity.LengthSquared();

			var aoa = CalculateAngleOfAttack(localVelocity);
			var liftCoefficient = InterpolateFromQuarterAoaCurve(TotalLiftCoefficient, aoa);
			var parasiticDragCoefficient = InterpolateFromQuarterAoaCurve(ParasiticDragCoefficient, aoa);

			var liftMag = CalculateAeroForceMagnitude(liftCoefficient, Area, density, localSpeedSquared);
			var frontalArea = CalculateFrontalArea(aoa);
			var dragMag = CalculateAeroForceMagnitude(parasiticDragCoefficient, frontalArea, density, localSpeedSquared);

			var liftVector = GlobalTransform.basis.y * liftMag * (HasPositiveAoa(aoa) ? 1 : -1);
			var dragVector = -relativeVelocity.Normalized() * dragMag;

			return liftVector + dragVector;
			// return Vector3.Zero;
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
			var frontLipArea = Mathf.Abs(frontLipThickness * Mathf.Sqrt(Area));
			var bodyArea = Mathf.Sin(aoa) * Area;

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
