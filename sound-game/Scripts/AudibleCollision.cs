using Godot;
using System;

public class AudibleCollision : Spatial, IAudibleCollision {
	private AudioStreamPlayer3D player;
	
	public override void _Ready()
	{
		player = GetNode<AudioStreamPlayer3D>("AudioStreamPlayer3D");
	}

	public void OnCollision()
	{
		player.Play();
	}
}
