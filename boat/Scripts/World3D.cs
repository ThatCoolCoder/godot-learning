using Godot;
using System;

public class World3D : Spatial
{
	public override void _Ready()
	{
		var water = GetNode<Physics.Fluids.ISpatialFluid>("Tracker/Water");
		var air = GetNode<Physics.Fluids.ISpatialFluid>("Air");
		var boat = GetNode<Physics.SpatialFluidEffectable>("Boat");
		boat.Fluids.Add(water);
		boat.Fluids.Add(air);
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("restart")) GetTree().ReloadCurrentScene();
	}
}
