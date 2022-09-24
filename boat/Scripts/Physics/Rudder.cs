using Godot;
using System;

namespace Physics
{
	public class Rudder : Node2D
	{
		[Export] public float DebugSteeringForce { get; set; } = 20;
		[Export] public bool UseDebugSteering { get; set; }

		[Export] public NodePath BodyPath { get; set; }
		private RigidBody2D body;

		public override void _Ready()
		{
			body = GetNode<RigidBody2D>(BodyPath);
		}

		public override void _PhysicsProcess(float delta)
		{
			if (UseDebugSteering)
			{
				var steeringForce = Vector2.Zero;
				if (Input.IsActionPressed("turn_left"))
					steeringForce += Vector2.Down * DebugSteeringForce;
				if (Input.IsActionPressed("turn_right"))
					steeringForce += Vector2.Up * DebugSteeringForce;
				body.ApplyImpulse(Position.Rotated(GlobalRotation), steeringForce.Rotated(GlobalRotation));
			}

			base._PhysicsProcess(delta);
		}
	}
}
