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
			if (XAxis) trans.origin.x = targetTrans.x;
			if (YAxis) trans.origin.y = targetTrans.y;
			if (ZAxis) trans.origin.z = targetTrans.z;
			GlobalTransform = trans;
		}
	}
}
