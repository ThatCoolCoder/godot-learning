using Godot;
using System;

public class Player : KinematicBody
{
	// 3rd person player controller

	[Export] private float maxWalkSpeed = 10;
	[Export] private float walkAcceleration = 20;
	[Export] private float walkFriction = 40;
	[Export] private float cameraMaxTurnSpeed = Mathf.Pi;
	[Export] private float cameraTurnAcceleration = Mathf.Pi * 6;
	[Export] private float cameraTurnFriction = Mathf.Pi * 4;
	[Export] private float jumpSpeed = 40;
	[Export] private NodePath cameraPath;
	private Spatial cameraHolder;
	private Vector3 velocity;
	private float cameraAngularVelocity;
	private float gravity = (float) ProjectSettings.GetSetting("physics/3d/default_gravity");

	public override void _Ready()
	{
		cameraHolder = GetNode<Spatial>(cameraPath);
	}

	public override void _PhysicsProcess(float delta)
	{
		var acceleration = CheckWalkKeybinds();
		CheckJumpKeybinds();
		acceleration.y -= gravity;

		CheckCameraRotationKeybinds(delta);

		velocity += acceleration * delta;
		ClampHorizontalVelocity();
		FeelFloorFriction(acceleration, delta);

		FaceMovementDirection();

		velocity = MoveAndSlide(velocity, Vector3.Up);
	}

	private Vector3 CheckWalkKeybinds()
	{
		var acceleration = Vector3.Zero;
		if (Input.IsActionPressed("walk_forward")) acceleration.z += walkAcceleration;
		if (Input.IsActionPressed("walk_backward")) acceleration.z -= walkAcceleration;
		if (Input.IsActionPressed("walk_left")) acceleration.x += walkAcceleration;
		if (Input.IsActionPressed("walk_right")) acceleration.x -= walkAcceleration;
		acceleration = acceleration.Rotated(Vector3.Up, cameraHolder.Rotation.y);

		return acceleration;
	}

	private void CheckJumpKeybinds()
	{
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{

		}
	}

	private void CheckCameraRotationKeybinds(float delta)
	{
		float angularAcceleration = 0;
		if (Input.IsActionPressed("turn_left")) angularAcceleration += cameraTurnAcceleration;
		if (Input.IsActionPressed("turn_right")) angularAcceleration -= cameraTurnAcceleration;


		cameraAngularVelocity += angularAcceleration * delta;

		if (angularAcceleration == 0)
		{
			cameraAngularVelocity = Utils.ConvergeValue(cameraAngularVelocity, 0, cameraTurnFriction * delta);
		}

		cameraAngularVelocity = Mathf.Clamp(cameraAngularVelocity, -cameraMaxTurnSpeed, cameraMaxTurnSpeed);
		cameraHolder.Rotation = new Vector3(cameraHolder.Rotation.x, cameraHolder.Rotation.y + cameraAngularVelocity * delta, cameraHolder.Rotation.z);
	}

	private void FaceMovementDirection()
	{
		var yRotation = new Vector2(-velocity.x, velocity.z).Angle() - Mathf.Pi / 2;
		Rotation = new Vector3(Rotation.x, yRotation, Rotation.z);
	}

	private void FeelFloorFriction(Vector3 acceleration, float delta)
	{
		if (acceleration.x == 0) velocity.x = Utils.ConvergeValue(velocity.x, 0, walkFriction * delta);
		if (acceleration.z == 0) velocity.z = Utils.ConvergeValue(velocity.z, 0, walkFriction * delta);
	}

	private void ClampHorizontalVelocity()
	{
		var horizontalVelocity = new Vector2(velocity.x, velocity.z);
		horizontalVelocity = horizontalVelocity.Clamped(maxWalkSpeed);
		velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.y);
	}
}
