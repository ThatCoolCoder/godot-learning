using Godot;
using System;

public class Boat : RigidBody2D
{
	[Export] public float ForwardMotorPower { get; set; } = 40;
	[Export] public float BackwardMotorPower { get; set; } = 20;

	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionPressed("drive_forward"))
			ApplyCentralImpulse((ForwardMotorPower * Vector2.Up).Rotated(Rotation));
		if (Input.IsActionPressed("drive_backward"))
			ApplyCentralImpulse((BackwardMotorPower * Vector2.Down).Rotated(Rotation));

		base._PhysicsProcess(delta);
	}
}
