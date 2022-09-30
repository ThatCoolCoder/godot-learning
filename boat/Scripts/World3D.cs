using Godot;
using System;

public class World3D : Spatial
{
	public override void _Ready()
	{
		var water = GetNode<Physics.Fluids.ISpatialFluid>("Water");
		var boat = GetNode<Physics.SpatialFluidEffectable>("Boat");
		boat.Fluids.Add(water);
	}
}
