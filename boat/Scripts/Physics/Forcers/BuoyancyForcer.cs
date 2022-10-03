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
			float volume = mesh.GetAabb().GetArea();
			if (top > waterLevel)
			{
				immersion = Mathf.Max(0, waterLevel - bottom) / boundingBoxSize.y;
				// immersion *= ImmersionProportion(immersion, waterLevel, mesh.GetAabb());
				volume *= immersion;
			}
			var buoyantForce = fluid.NormalAtPoint(GlobalTransform.origin) * volume * waterDensity;

			var dragForce = CalculateDrag(fluid, immersion, GetArea(boundingBox));

			return buoyantForce + dragForce;
		}

		private Vector3 CalculateDrag(Fluids.ISpatialFluid fluid, float immersion, float area)
		{
			return 0.5f * DragCoefficient * area * -(target.LinearVelocity.Normalized() * target.LinearVelocity.LengthSquared()) * immersion;
		}

		private float GetArea(AABB boundingBox)
		{
			var vel = target.LinearVelocity;
			var components = new List<float>() { vel.x, vel.y, vel.z }.Select(x => Mathf.Abs(x));
			var largest = components.Max();
			var size = boundingBox.Size;

			if (largest == vel.x) return size.y * size.z;
			if (largest == vel.y) return size.x * size.z;
			else return size.y * size.y;
		}

		private float ImmersionProportion(float immersion, float waterHeight, AABB boundingBox)
		{
			// proportion of immersion taking into account rotation of the AABB

			var rect = new Rect2(boundingBox.Position.z, boundingBox.Position.y, boundingBox.Size.z, boundingBox.Size.y);
			var (area1, area2) = Geometry.LineIntersectRotatedRectangleAreas(new Vector2(-100, waterHeight), new Vector2(100, waterHeight), rect, Rotation.x);
			var larger = Mathf.Max(area1, area2);
			var smaller = Mathf.Min(area1, area2);
			var goodArea = immersion < 0.5f ? smaller : larger;

			// GD.Print(goodArea);
			return goodArea / rect.Area;
		}
	}
}
