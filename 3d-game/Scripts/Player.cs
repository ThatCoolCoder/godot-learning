using Godot;
using System;
using System.Collections.Generic;

public enum PlayerState
{
	OnGround,
	FreeFall,
	Grappling,
	Dashing,
}

public class Player : KinematicBody
{
	// 1st person player controller
	
	// Movement exports
	[Export] private float sprintMultiplier = 2.0f;
	[Export] private float maxWalkSpeed = 10;
	[Export] private float walkAcceleration = 20;
	[Export] private float walkFriction = 40;
	[Export] private float dashSpeed = 100;
	[Export] private float jumpSpeed = 40;
	[Export] private float grappleDragCoefficient = 0.1f;
	[Export] private float maxGrappleAcceleration = 400;
	[Export] private float maxGrappleForceDist = 10;
	[Export] private float maxGrappleDist = 150;
	
	// Viewing exports
	[Export] private Vector2 mouseSensitivity = Vector2.One * 0.002f;
	[Export] private float maxLookDown = -1.2f;
	[Export] private float maxLookUp = 1.2f;

	// State management
	private Dictionary<PlayerState, Func<float, Vector3>> stateActions = new();
	public PlayerState State = PlayerState.OnGround;
	private Vector3 grappleAnchorPosition = Vector3.Zero;
	private Vector3 velocity;
	private float gravity = (float) ProjectSettings.GetSetting("physics/3d/default_gravity");

	// Node references
	private Spatial head;
	private AnimationPlayer animationPlayer;
	private Timer dashFinishTimer;
	private Timer dashRechargeTimer;
	private RayCast grappleRayCast;

	public override void _Ready()
	{
		head = GetNode<Spatial>("Head");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		dashFinishTimer = GetNode<Timer>("DashFinishTimer");
		dashRechargeTimer = GetNode<Timer>("DashRechargeTimer");
		grappleRayCast = GetNode<RayCast>("Head/GrappleRayCast");
		Input.SetMouseMode(Input.MouseMode.Captured);

		animationPlayer.CurrentAnimation = "head_bobbing";

		// Set here because we can't do it in field initialisers
		stateActions = new()
		{
			{PlayerState.OnGround, delta => OnGroundAction(delta)},
			{PlayerState.FreeFall, delta => FreeFallAction(delta)},
			{PlayerState.Grappling, delta => GrapplingAction(delta)},
			{PlayerState.Dashing, delta => DashingAction(delta)}
		};
	}

	public override void _Input(InputEvent _event)
	{
		// Rotate view with mouse
		if (_event is InputEventMouseMotion && Input.GetMouseMode() == Input.MouseMode.Captured)
		{
			var motionEvent = _event as InputEventMouseMotion;
			RotateY(-motionEvent.Relative.x * mouseSensitivity.x);
			head.RotateX(motionEvent.Relative.y * mouseSensitivity.y);
			head.Rotation = new Vector3(Mathf.Clamp(head.Rotation.x, -maxLookUp, -maxLookDown),
				head.Rotation.y, head.Rotation.z);
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		var acceleration = stateActions[State](delta);
		acceleration.y -= gravity;
		velocity += acceleration * delta;
		velocity = MoveAndSlide(velocity, Vector3.Up);
		
		UpdateAnimation();
	}

	private void ResetState()
	{
		State = IsOnFloor() ? PlayerState.OnGround : PlayerState.FreeFall;
	}

	#region Actions
	private Vector3 OnGroundAction(float delta)
	{
		var acceleration = CheckWalkKeybinds();
		CheckJumpKeybinds();
		ClampHorizontalVelocity();
		FeelFloorFriction(acceleration, delta);
		CheckGrappleStart();
		CheckDashKeybinds();
		return acceleration;
	}

	private Vector3 FreeFallAction(float delta)
	{
		if (IsOnFloor()) State = PlayerState.OnGround;
		
		var acceleration = CheckWalkKeybinds();
		ClampHorizontalVelocity();
		CheckDashKeybinds();
		CheckGrappleStart();

		return acceleration;
	}

	private Vector3 GrapplingAction(float delta)
	{
		var distance = Translation.DistanceTo(grappleAnchorPosition);
		var distanceMultiplier = Math.Min(distance / maxGrappleForceDist, 1);
		var acceleration = (grappleAnchorPosition - Translation).Normalized() * distanceMultiplier * maxGrappleAcceleration;
		acceleration -= velocity.Normalized() * velocity.LengthSquared() * grappleDragCoefficient * delta;
		CheckGrappleEnd();
		return acceleration;
	}
	private Vector3 DashingAction(float delta)
	{
		if (dashFinishTimer.TimeLeft == 0)
		{
			ResetState();
			dashRechargeTimer.Start();
		}
		return Vector3.Zero;
	}

	#endregion Actions

	#region UsedByActions
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

	private void CheckGrappleStart()
	{
		if (Input.IsActionJustPressed("grapple"))
		{
			grappleRayCast.CastTo = grappleRayCast.CastTo.Normalized() * maxGrappleDist;
			grappleRayCast.ForceRaycastUpdate();
			if (grappleRayCast.IsColliding())
			{
				var anchorPosition = grappleRayCast.GetCollisionPoint();
				if (Translation.DistanceSquaredTo(anchorPosition) < maxGrappleDist * maxGrappleDist)
				{
					State = PlayerState.Grappling;
					grappleAnchorPosition = anchorPosition;
				}
			}
		}
	}

	private void CheckGrappleEnd()
	{
		if (Input.IsActionJustReleased("grapple") && State == PlayerState.Grappling)
		{
			ResetState();
		}
	}

	private void CheckJumpKeybinds()
	{
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.y = jumpSpeed;
			State = PlayerState.FreeFall;
		}
	}

	private void CheckDashKeybinds()
	{
		if (Input.IsActionJustPressed("dash") && dashRechargeTimer.TimeLeft == 0)
		{
			State = PlayerState.Dashing;
			velocity = Vector3.Back.Rotated(Vector3.Up, Rotation.y) * dashSpeed;
			dashFinishTimer.Start();
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

	#endregion UsedByActions
	
	private void UpdateAnimation()
	{
		if (velocity.LengthSquared() < 0.01 || ! IsOnFloor())
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
