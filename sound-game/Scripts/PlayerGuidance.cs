using Godot;
using System;

public class PlayerGuidance : Spatial
{
	private Spatial goal;
	private float distance = 3;
	private AudioStreamPlayer3D veryClosePlayer;
	private float veryCloseDistance = 5;

	public override void _Ready()
	{
		goal = GetTree().GetNodesInGroup("Goal")[0] as Spatial;
		veryClosePlayer = GetNode<AudioStreamPlayer3D>("VeryClosePlayer");
	}

	public override void _Process(float delta)
	{
		var offset = goal.GlobalTransform.origin - GlobalTransform.origin;
		Translation = offset.Normalized().Rotated(Vector3.Up, -GetParent<Spatial>().GlobalTransform.basis.GetEuler().y) * distance;
		if (offset.LengthSquared() < veryCloseDistance * veryCloseDistance)
		{
			if (! veryClosePlayer.Playing) veryClosePlayer.Play();
		}
		else
		{
			if (veryClosePlayer.Playing) veryClosePlayer.Stop();
		}
	}
}
