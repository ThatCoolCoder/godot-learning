using Godot;
using System;

public class Body : Spatial
{
	// (as in body of the player)

	public override void _Process(float delta)
	{
		// if (Input.IsActionJustPressed("switch_side")) Translation = new Vector3(Translation.x * -1, Translation.y, Translation.z);
	}
}
