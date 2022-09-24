using Godot;
using System;

namespace Physics
{
	public class Boat : FluidEffectable
	{
		[Export] public float ForwardMotorPower { get; set; } = 40;
		[Export] public float BackwardMotorPower { get; set; } = 20;

		public override void _Ready()
		{
			Fluid =  new Fluids.UniformFluid();
		}

		public override void _PhysicsProcess(float delta)
		{
			if (Input.IsActionPressed("drive_forward"))
				ApplyCentralImpulse((ForwardMotorPower * Vector2.Right).Rotated(Rotation));
			if (Input.IsActionPressed("drive_backward"))
				ApplyCentralImpulse((BackwardMotorPower * Vector2.Left).Rotated(Rotation));

			base._PhysicsProcess(delta);
		}
	}
}
