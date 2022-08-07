using Godot;
using System;

public class Player : KinematicBody
{
	[Export] private float gravity = 150;
	[Export] private float jumpSpeed = 40;
	[Export] private float maxWalkSpeed = 10;
	[Export] private float walkFriction = 40;
	[Export] private float walkAcceleration = 20;
	[Export] private float sprintMultiplier = 2;
	[Export] private float turnSpeedDegrees = 90;
	[Export] private float slideFactor = 0; // 0 means it stays still on slopes, 1 means it slides uncontrollably down slopes
	private Vector3 velocity = new Vector3();
	public event Action FoundGoal;
	public event Action Died;

	public override void _PhysicsProcess(float delta)
	{
		CheckViewKeybinds(delta);
		var acceleration = Vector3.Zero;
		acceleration += CheckWalkKeybinds();
		CheckJumpKeybinds();
		FeelFloorFriction(acceleration, delta);
		ClampHorizontalVelocity();

		acceleration.y -= gravity;
		velocity += acceleration * delta;
		velocity = MoveAndSlide(velocity + GetSlideCorrection(delta), Vector3.Up, true);
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

	private Vector3 CheckWalkKeybinds()
	{
		var acceleration = Vector3.Zero;
		if (Input.IsActionPressed("walk_forward")) acceleration.z -= walkAcceleration;
		if (Input.IsActionPressed("walk_backward")) acceleration.z += walkAcceleration;
		if (Input.IsActionPressed("walk_left")) acceleration.x -= walkAcceleration;
		if (Input.IsActionPressed("walk_right")) acceleration.x += walkAcceleration;
		acceleration = acceleration.Rotated(Vector3.Up, Rotation.y);

		return acceleration;
	}

	private void CheckJumpKeybinds()
	{
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.y = jumpSpeed;
		}
	}

	private void FeelFloorFriction(Vector3 acceleration, float delta)
	{
		var rotatedAcceleration = acceleration.Rotated(Vector3.Up, Rotation.y);
		FeelFloorFriction(rotatedAcceleration.x == 0, rotatedAcceleration.z == 0, delta);
	}

	private void FeelFloorFriction(bool frictionX, bool frictionZ, float delta)
	{
		var rotatedVelocity = velocity.Rotated(Vector3.Up, Rotation.y);
		if (frictionX) rotatedVelocity.x = Utils.ConvergeValue(rotatedVelocity.x, 0, walkFriction * delta);
		if (frictionZ) rotatedVelocity.z = Utils.ConvergeValue(rotatedVelocity.z, 0, walkFriction * delta);
		velocity = rotatedVelocity.Rotated(Vector3.Up, -Rotation.y);
	}

	private void ClampHorizontalVelocity()
	{
		var currentSprintMultiplier = Input.IsActionPressed("sprint") ? sprintMultiplier : 1.0f;
		var horizontalVelocity = new Vector2(velocity.x, velocity.z);
		horizontalVelocity = horizontalVelocity.Clamped(maxWalkSpeed * currentSprintMultiplier);
		velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.y);
	}

	private void CheckViewKeybinds(float delta)
	{
		if (Input.IsActionPressed("turn_left")) RotationDegrees = new Vector3(RotationDegrees.x, RotationDegrees.y + turnSpeedDegrees * delta, RotationDegrees.z);
		if (Input.IsActionPressed("turn_right")) RotationDegrees = new Vector3(RotationDegrees.x, RotationDegrees.y - turnSpeedDegrees * delta, RotationDegrees.z);
	}

	private Vector3 GetSlideCorrection(float delta)
	{
		var correction = new Vector3(0,0,0);
		if (IsOnFloor())
		{
			correction = Vector3.Up - GetFloorNormal();
			correction *= gravity * delta;
			correction *= (1 - slideFactor);
		}
		return correction;
	}
}
