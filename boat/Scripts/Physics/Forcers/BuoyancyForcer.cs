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

		private MeshInstance mesh;

		public override void _Ready()
		{
			mesh = GetNode<MeshInstance>("Mesh");
			base._Ready();
		}

		public override Vector3 CalculateForce(Fluids.ISpatialFluid fluid)
		{
			var waterLevel = fluid.HeightAtPoint(GlobalTransform.origin);
			var waterDensity = fluid.DensityAtPoint(GlobalTransform.origin);

			var boundingBox = mesh.GetTransformedAabb();
			var boundingBoxSize = boundingBox.Size;
			var top = boundingBox.Position.y + boundingBoxSize.y / 2;
			var bottom = boundingBox.Position.y - boundingBoxSize.y / 2;

			float immersion = 1;
			if (top > waterLevel)
			{
				immersion = Mathf.Max(0, waterLevel - bottom) / boundingBoxSize.y;
			}
			var buoyantForce = Vector3.Zero;
			buoyantForce.y = boundingBox.GetArea() * immersion * waterDensity;

			var dragForce = CalculateDrag(fluid, immersion, GetArea(boundingBox));

			return buoyantForce + dragForce;
		}

		private Vector3 CalculateDrag(Fluids.ISpatialFluid fluid, float immersion, float area)
		{
			return 0.5f * 1 * area * -(target.LinearVelocity.Normalized() * target.LinearVelocity.LengthSquared()) * immersion;
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
	}
}
