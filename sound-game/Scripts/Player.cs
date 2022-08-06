using Godot;
using System;

public class Player : KinematicBody
{
	private Vector3 velocity = new Vector3();
	private float turnSpeedDegrees = 90;
	private float walkSpeed = 5;
	public event Action FoundGoal;
	public event Action Died;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		velocity = Vector3.Zero;
		if (Input.IsActionPressed("walk_forward")) velocity.z -= walkSpeed;
		if (Input.IsActionPressed("walk_backward")) velocity.z += walkSpeed;
		if (Input.IsActionPressed("walk_left")) velocity.x -= walkSpeed;
		if (Input.IsActionPressed("walk_right")) velocity.x += walkSpeed;
		if (Input.IsActionPressed("turn_left")) RotationDegrees = new Vector3(RotationDegrees.x, RotationDegrees.y + turnSpeedDegrees * delta, RotationDegrees.z);
		if (Input.IsActionPressed("turn_right")) RotationDegrees = new Vector3(RotationDegrees.x, RotationDegrees.y - turnSpeedDegrees * delta, RotationDegrees.z);

		velocity = velocity.Rotated(Vector3.Up, Rotation.y);

		MoveAndSlide(velocity, Vector3.Up, true);
		HandleSlideCollisions();
	}

	private void HandleSlideCollisions() {
		// for (int i = 0; i < GetSlideCount(); i ++) {
		// 	var collider = GetSlideCollision(i).Collider as Spatial;
		// 	OnBodyEntered(collider);
		// }
	}

	private void OnBodyEntered(Node body)
	{
		if (body is IAudibleCollision ac) ac.OnCollision();
		if (body.IsInGroup("Goal")) FoundGoal.Invoke();
		if (body.IsInGroup("Enemy")) Died.Invoke();
	}
}
