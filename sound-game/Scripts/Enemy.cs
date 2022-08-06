using Godot;
using System;

public class Enemy : Area, IAudibleCollision
{
	public float speed = 2;
	private Spatial player;
	public AudioStreamPlayer3D collisionSound;

	public override void _Ready()
	{
		collisionSound = GetNode<AudioStreamPlayer3D>("CollisionSound");
		player = GetTree().GetNodesInGroup("Player")[0] as Spatial;
		// RotationDegrees = new Vector3(RotationDegrees.x, GD.Randf() * 360, RotationDegrees.z);
	}

	public override void _Process(float delta)
	{
		LookAt(player.GlobalTransform.origin, Vector3.Up);
		Translation += (Vector3.Forward * speed).Rotated(Vector3.Up, Rotation.y) * delta;
	}


	private void OnDeathTimerTimeout()
	{
		QueueFree();
	}

	public void OnCollision()
	{
		collisionSound.Play();
	}
	
}
