using Godot;
using System;

public class Tracker : Spatial
{
	// Object that moves itself to follow another object
	[Export] public bool Enabled { get; set; } = true;
	[Export] public bool XAxis { get; set; } = true;
	[Export] public bool YAxis { get; set; } = true;
	[Export] public bool ZAxis { get; set; } = true;
	[Export] public NodePath TargetPath { get; set; }
	[Export] public float SnappingIncrement { get; set; } = 0; // snapping increment of the tracker's movement. Set to 0 for no snapping
	private Spatial target;

	public override void _Ready()
	{
		target = GetNode<Spatial>(TargetPath);
	}

	public override void _PhysicsProcess(float delta)
	{
		if (Enabled)
		{
			var trans = GlobalTransform;
			var targetTrans = target.GlobalTransform.origin;
			if (XAxis) trans.origin.x = RoundAxis(targetTrans.x);
			if (YAxis) trans.origin.y = RoundAxis(targetTrans.y);
			if (ZAxis) trans.origin.z = RoundAxis(targetTrans.z);
			GlobalTransform = trans;
		}
	}

	private float RoundAxis(float axisValue)
	{
		if (SnappingIncrement == 0) return axisValue;
		else return Utils.RoundNumber(axisValue, SnappingIncrement);
	}
}
