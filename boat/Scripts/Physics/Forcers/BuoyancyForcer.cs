using Godot;
using System.Collections.Generic;
using System.Linq;

namespace Physics.Forcers
{
	public class BuoyancyForcer : AbstractSpatialFluidForcer
	{
		// Basic buoyancy and also drag forcer
		// Added drag because it needs damping or oscillates too much

		// todo: port this over: https://github.com/saurus/Buoyancy.
		// or possibly this to start: https://godotforums.org/d/27725-creating-a-water-buoyancy-system/33

		[Export] public float DragCoefficient { get; set; } = 0.5f;
		[Export] public float NormalBuoyancyFactor { get; set; } = 0.1f; // Factor of normal of surface taken into account when calculating buoyant forces
		[Export]
		public Curve DepthToAreaCurve { get; set; } = null; // curve from 0 to 1 used as a multiplier of volume. todo: if object is inverted then invert curve.
															// Used as a rough approximation for tapering hulls in absence of proper mesh-based volume detection.
															// Should be seen as the running integral of the object's shape.
															// todo: make method for calculating this running integral so then this can just match the shape of the side of the boat.
		private MeshInstance mesh;

		public override void _Ready()
		{
			mesh = GetNode<MeshInstance>("Mesh");
			base._Ready();
		}

		// public override void _PhysicsProcess(float delta)
		// {
		// 	var fluid = GetNode<Fluids.ISpatialFluid>("../Water");
		// 	var waterLevel = fluid.HeightAtPoint(GlobalTransform.origin);
		// 	var waterDensity = fluid.DensityAtPoint(GlobalTransform.origin);

		// 	var boundingBox = mesh.GetTransformedAabb();
		// 	var boundingBoxSize = boundingBox.Size;
		// 	var top = boundingBox.Position.y + boundingBoxSize.y;
		// 	var bottom = boundingBox.Position.y;

		// 	float immersion = 1;
		// 	float volume = mesh.GetAabb().GetArea();
		// 	if (top > waterLevel)
		// 	{
		// 		immersion = Mathf.Max(0, waterLevel - bottom) / boundingBoxSize.y;
		// 		immersion *= ImmersionProportion(immersion, waterLevel, mesh.GetAabb());
		// 		volume *= immersion;
		// 	}
		// }

		public override Vector3 CalculateForce(Fluids.ISpatialFluid fluid)
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
				// immersion *= ImmersionProportion(immersion, waterLevel, mesh.GetAabb());
				// if (DepthToAreaCurve == null) volume *= immersion;
				volume *= immersion;
				if (DepthToAreaCurve != null) volume *= DepthToAreaCurve.Interpolate(immersion);
				buoyancyDirection += fluid.NormalAtPoint(GlobalTransform.origin) * NormalBuoyancyFactor;
			}

			var buoyantForce = buoyancyDirection * volume * waterDensity * (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

			var dragForce = CalculateDrag(fluid, immersion, GetArea(boundingBox));
			dragForce.x = 0;
			dragForce.z = 0;
			dragForce.y = 0;

			if (buoyantForce.Length() > 3000) GD.Print(immersion, ", ", buoyantForce, target.LinearVelocity);

			return buoyantForce + dragForce;
		}

		private Vector3 CalculateDrag(Fluids.ISpatialFluid fluid, float immersion, float area)
		{
			var velocityDelta = target.VelocityAtPoint(GlobalTranslation) - fluid.VelocityAtPoint(GlobalTranslation);
			return 0.5f * DragCoefficient * area * -(velocityDelta.Normalized() * velocityDelta.LengthSquared()) * immersion * fluid.DensityAtPoint(GlobalTranslation);
		}

		private float GetArea(AABB boundingBox)
		{
			var vel = target.VelocityAtPoint(GlobalTranslation);
			var components = new List<float>() { vel.x, vel.y, vel.z }.Select(x => Mathf.Abs(x));
			var largest = components.Max();
			var size = boundingBox.Size;

			if (largest == vel.x) return size.y * size.z;
			if (largest == vel.y) return size.x * size.z;
			else return size.x * size.y;
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
