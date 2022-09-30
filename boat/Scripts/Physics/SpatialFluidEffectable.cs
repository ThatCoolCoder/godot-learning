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
			// Get the global velocity of a specific point on this, taking into account the rotation
			// Todo: find the code for this when I can access reddit

			return LinearVelocity;
		}
	}
}
