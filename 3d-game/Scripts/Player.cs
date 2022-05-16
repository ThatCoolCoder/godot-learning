using Godot;
using System;

enum PlayerState
{
	Static,
	Walking,
	Midair
}

public class Player : KinematicBody
{
	// 1st person player controller

	[Export] private float maxWalkSpeed = 10;
	[Export] private float walkAcceleration = 20;
	[Export] private float walkFriction = 40;
	[Export] private float jumpSpeed = 40;
	[Export] private Vector2 mouseSensitivity = Vector2.One * 0.002f;
	[Export] private float maxLookDown = -1.2f;
	[Export] private float maxLookUp = 1.2f;
	private Spatial head;
	private AnimationPlayer animationPlayer;
	private Vector3 velocity;
	private float gravity = (float) ProjectSettings.GetSetting("physics/3d/default_gravity");
	private PlayerState state;

	public override void _Ready()
	{
		head = GetNode<Spatial>("Head");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Input.SetMouseMode(Input.MouseMode.Captured);

		animationPlayer.CurrentAnimation = "head_bobbing";
	}

	public override void _PhysicsProcess(float delta)
	{
		var acceleration = CheckWalkKeybinds();
		CheckJumpKeybinds();
		acceleration.y -= gravity;

		velocity += acceleration * delta;
		ClampHorizontalVelocity();
		FeelFloorFriction(acceleration, delta);

		velocity = MoveAndSlide(velocity, Vector3.Up);
		
		UpdateAnimation();
	}

	public override void _UnhandledInput(InputEvent _event)
	{
		if (_event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
		{
			var motionEvent = _event as InputEventMouseMotion;
			RotateY(-motionEvent.Relative.x * mouseSensitivity.x);
			head.RotateX(motionEvent.Relative.y * mouseSensitivity.y);
			head.Rotation = new Vector3(Mathf.Clamp(head.Rotation.x, maxLookDown, maxLookUp),
				head.Rotation.y, head.Rotation.z);
		}
	}

	private Vector3 CheckWalkKeybinds()
	{
		var acceleration = Vector3.Zero;
		if (Input.IsActionPressed("walk_forward")) acceleration.z += walkAcceleration;
		if (Input.IsActionPressed("walk_backward")) acceleration.z -= walkAcceleration;
		if (Input.IsActionPressed("walk_left")) acceleration.x += walkAcceleration;
		if (Input.IsActionPressed("walk_right")) acceleration.x -= walkAcceleration;
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
		if (acceleration.x == 0) velocity.x = Utils.ConvergeValue(velocity.x, 0, walkFriction * delta);
		if (acceleration.z == 0) velocity.z = Utils.ConvergeValue(velocity.z, 0, walkFriction * delta);
	}

	private void ClampHorizontalVelocity()
	{
		var horizontalVelocity = new Vector2(velocity.x, velocity.z);
		horizontalVelocity = horizontalVelocity.Clamped(maxWalkSpeed);
		velocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.y);
	}
	
	private void UpdateAnimation()
	{
		if (velocity.LengthSquared() < 0.001 || ! IsOnFloor())
		{
			animationPlayer.Seek(0);
			animationPlayer.PlaybackActive = false;
		}
		else
		{
			animationPlayer.PlaybackActive = true;
		}
		
	}
}
