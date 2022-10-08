using Godot;
using System;

public class Player : KinematicBody
{
	[Export] public bool Blinded;
	[Export] public bool CanMove = true;
	[Export] public float Gravity = 150;
	[Export] public float JumpSpeed = 40;
	[Export] public float MaxWalkSpeed = 10;
	[Export] public float WalkFriction = 40;
	[Export] public float WalkAcceleration = 20;
	[Export] public float SprintMultiplier = 2;
	[Export] public float TurnSpeedDegrees = 90;
	[Export] public float SlideFactor = 0; // 0 means it stays still on slopes, 1 means it slides uncontrollably down slopes
	private Vector3 velocity = new Vector3();
	public event Action FoundGoal;
	public event Action Died;

	public override void _Ready()
	{
		var cameraBlocker = GetNode<Spatial>("Camera/Blocker");
		GD.Print(Blinded);
		if (Blinded) cameraBlocker.Show();
		else cameraBlocker.Hide();
	}

	public override void _PhysicsProcess(float delta)
	{
		var acceleration = Vector3.Zero;
		if (CanMove)
		{
			acceleration += CheckWalkKeybinds();
			CheckViewKeybinds(delta);
			CheckJumpKeybinds();
		}
		FeelFloorFriction(acceleration, delta);
		ClampHorizontalVelocity();

		acceleration.y -= Gravity;
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
		if (Input.IsActionPressed("walk_forward")) acceleration.z -= WalkAcceleration;
		if (Input.IsActionPressed("walk_backward")) acceleration.z += WalkAcceleration;
		if (Input.IsActionPressed("walk_left")) acceleration.x -= WalkAcceleration;
		if (Input.IsActionPressed("walk_right")) acceleration.x += WalkAcceleration;
		acceleration = acceleration.Rotated(Vector3.Up, Rotation.y);

		return acceleration;
	}

	private void CheckJumpKeybinds()
	{
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.y = JumpSpeed;
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
		if (frictionX) rotatedVelocity.x = Utils.ConvergeValue(rotatedVelocity.x, 0, WalkFriction * delta);
		if (frictionZ) rotatedVelocity.z = Utils.ConvergeValue(rotatedVelocity.z, 0, WalkFriction * delta);
		velocity = rotatedVelocity.Rotated(Vector3.Up, -Rotation.y);
	}

	private void ClampHorizontalVelocity()
	{
		var currentSprintMultiplier = Input.IsActionPressed("sprint") ? SprintMultiplier : 1.0f;
		var horizontalVelocity = new Vector2(velocity.x, velocity.z);
		horizontalVelocity = horizontalVelocity.LimitLength(MaxWalkSpeed * currentSprintMultiplier);
		velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.y);
	}

	private void CheckViewKeybinds(float delta)
	{
		if (Input.IsActionPressed("turn_left")) RotationDegrees = new Vector3(RotationDegrees.x, RotationDegrees.y + TurnSpeedDegrees * delta, RotationDegrees.z);
		if (Input.IsActionPressed("turn_right")) RotationDegrees = new Vector3(RotationDegrees.x, RotationDegrees.y - TurnSpeedDegrees * delta, RotationDegrees.z);
	}

	private Vector3 GetSlideCorrection(float delta)
	{
		var correction = new Vector3(0,0,0);
		if (IsOnFloor())
		{
			correction = Vector3.Up - GetFloorNormal();
			correction *= Gravity * delta;
			correction *= (1 - SlideFactor);
		}
		return correction;
	}
}
