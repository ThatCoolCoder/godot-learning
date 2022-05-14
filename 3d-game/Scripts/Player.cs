using Godot;
using System;

public class Player : KinematicBody
{
	[Export] private float maxSpeed = 2;
	[Export] private float acceleration = 2;
	[Export] private float friction = 1;
	private Vector2 velocity;

	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionPressed("walk_forward"))
		{
			
		}
	}
}
