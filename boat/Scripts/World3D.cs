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
		var sail = GetNode<Physics.SpatialFluidEffectable>("Boat/Sail");
		sail.Fluids.Add(air);
		var windSock = GetNode<Physics.SpatialFluidEffectable>("Boat/WindSock");
		windSock.Fluids.Add(air);
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("restart")) GetTree().ReloadCurrentScene();
	}
}
