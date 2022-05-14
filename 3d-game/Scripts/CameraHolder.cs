using Godot;
using System;

public class CameraHolder : Spatial
{
	// Thing to make camera follow player
	// We can't add it as a child or it will inherit rotation

	[Export] private NodePath playerPath;
	private Spatial player;

	public override void _Ready()
	{
		player = GetNode<Spatial>(playerPath);
	}

	public override void _PhysicsProcess(float delta)
	{
		Translation = player.Translation;
	}
}
