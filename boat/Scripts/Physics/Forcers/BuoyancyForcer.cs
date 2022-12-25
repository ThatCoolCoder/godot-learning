using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Physics.Forcers
{
	public class BuoyancyForcer : AbstractSpatialFluidForcer
	{
		// Basic buoyancy and also vertical drag forcer
		// Added vertical drag because it needs damping or oscillates too much

		[Export] public float DragCoefficient { get; set; } = 0.5f;
		[Export] public float NormalBuoyancyFactor { get; set; } = 0.1f; // Factor of normal of surface taken into account when calculating buoyant forces

		// Do not change while simulation is going, must only set it at the start.
		// Curve from 0 to 1 that mimics the side profile of the hull, allowing us to approximate tapered/curved hulls without doing full mesh-based volume calculation
		[Export] public Curve HullShapeCurve { get; set; } = null;
		private Curve depthToAreaCurve = null;
		private const int numDepthSampleValues = 100;
		private MeshInstance mesh;

		public override void _Ready()
		{
			mesh = GetNode<MeshInstance>("Mesh");
			mesh.Visible = false;

			if (HullShapeCurve != null) CalculateDepthToAreaCurve();

			base._Ready();

		}

		private void CalculateDepthToAreaCurve()
		{
			// convert hull shape curve to area curve

			depthToAreaCurve = new Curve();
			float runningTotal = 0;
			float interval = 1.0f / (float)numDepthSampleValues;
			for (int i = 0; i < numDepthSampleValues + 1; i++)
			{
				float xPos = interval * i;
				runningTotal += HullShapeCurve.Interpolate(xPos) / numDepthSampleValues;
				depthToAreaCurve.AddPoint(new Vector2(xPos, runningTotal), leftMode: Curve.TangentMode.Linear, rightMode: Curve.TangentMode.Linear);
			}
		}


		public override Vector3 CalculateForce(Fluids.ISpatialFluid fluid, PhysicsDirectBodyState state)
		{
			var waterLevel = fluid.HeightAtPoint(GlobalTransform.origin);
			var waterDensity = fluid.DensityAtPoint(GlobalTransform.origin);

			var boundingBox = mesh.GetTransformedAabb();
			var boundingBoxSize = boundingBox.Size;
			var top = boundingBox.Position.y + boundingBoxSize.y;
			var bottom = boundingBox.Position.y;

			float immersion = 1;
			float volume = mesh.GetTransformedAabb().GetArea();
			var buoyancyDirection = Vector3.Up;
			if (top > waterLevel)
			{
				immersion = Mathf.Max(0, waterLevel - bottom) / boundingBoxSize.y;
				if (depthToAreaCurve != null) immersion = depthToAreaCurve.Interpolate(immersion);
				buoyancyDirection += fluid.NormalAtPoint(GlobalTransform.origin) * NormalBuoyancyFactor;
			}

			volume *= immersion;

			var buoyantForce = buoyancyDirection * volume * waterDensity * (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

			// Calculate area to be X * Z because that's the face we care about
			var dragForce = CalculateDrag(state, fluid, immersion, boundingBox.Size.x * boundingBox.Size.z);

			return buoyantForce + dragForce;
		}

		private Vector3 CalculateDrag(PhysicsDirectBodyState state, Fluids.ISpatialFluid fluid, float immersion, float area)
		{
			var velocityDelta = state.GetVelocityAtGlobalPosition(target, this) - fluid.VelocityAtPoint(GlobalTranslation);
			var force = 0.5f * DragCoefficient * immersion * area * velocityDelta.LengthSquared() * fluid.DensityAtPoint(GlobalTranslation);
			var result = velocityDelta.Normalized() * -force;

			// We only need vertical drag in this forcer
			result.x = 0;
			result.z = 0;
			return result;
		}

		private float ImmersionProportion(float immersion, float waterHeight, AABB boundingBox)
		{
			// proportion of immersion taking into account rotation of the AABB

			var rect = new Rect2(boundingBox.Position.z, boundingBox.Position.y, boundingBox.Size.z, boundingBox.Size.y);
			var (area1, area2) = Geometry.LineIntersectRotatedRectangleAreas(new Vector2(-100, waterHeight), new Vector2(100, waterHeight), rect, Rotation.x);
			var larger = Mathf.Max(area1, area2);
			var smaller = Mathf.Min(area1, area2);
			var goodArea = immersion < 0.5f ? smaller : larger;

			return goodArea / rect.Area;
		}
	}
}
