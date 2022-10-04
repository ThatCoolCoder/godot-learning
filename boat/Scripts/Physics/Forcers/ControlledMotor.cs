using Godot;
using System;

namespace Physics.Forcers
{
	public class ControlledMotor : Motor
	{
		// Motor controlled by the keyboard

		[Export] public string ForwardActionName { get; set; }
		[Export] public string BackwardActionName { get; set; }

		public override void _PhysicsProcess(float delta)
		{
			ThrustProportion = 0;
			if (Input.IsActionPressed(ForwardActionName)) ThrustProportion = 1;
			else if (Input.IsActionPressed(ForwardActionName)) ThrustProportion = -1;
			base._PhysicsProcess(delta);
		}
	}
}
