using Godot;
using System;
using System.Collections.Generic;

namespace Physics
{
	public class SpatialFluidEffectable : RigidBody
	{
		public List<Fluids.ISpatialFluid> Fluids { get; set; } = new();

		public Vector3 VelocityAtPoint(Vector3 point)
		{
			var offset = point - GlobalTransform.origin;
			// AngularVelocity.Cross(offset);
			return LinearVelocity + AngularVelocity.Cross(offset);
		}
	}
}
