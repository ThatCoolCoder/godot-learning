using Godot;
using System;
using System.Collections.Generic;


namespace Physics
{
	public class SpatialFluidEffectable : RigidBody
	{
		public List<Fluids.ISpatialFluid> Fluids { get; set; } = new();
		public override void _Ready()
		{
			GravityScale = 0;
		}

		public Vector3 VelocityAtPoint(Vector3 point)
		{
			// Point should be in global coordinates

			var offset = point - GlobalTransform.origin;
			return LinearVelocity + AngularVelocity.Cross(offset);
		}

		public override void _PhysicsProcess(float delta)
		{
			// Godot inbuilt gravity appears to not be correct, so do it in a custom way

			var force = (float)ProjectSettings.GetSetting("physics/3d/default_gravity") * Mass;
			// ApplyCentralImpulse(Vector3.Down * force * delta);
		}
	}
}
