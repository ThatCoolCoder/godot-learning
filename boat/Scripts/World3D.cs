using Godot;
using System;

public class World3D : Spatial
{
	private AudioStreamGenerator gen = new();
	private AudioStreamGeneratorPlayback playback = null;


	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("restart")) GetTree().ReloadCurrentScene();
	}
}
