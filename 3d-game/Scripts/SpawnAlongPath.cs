using Godot;
using System;

public class SpawnAlongPath : Spatial
{
	[Export] private bool spawnOnReady = true; 
	[Export] private NodePath pathPath; // (The path to the path to spawn along)
	private Path path;
	[Export] private bool randomizeRotation = true;
	[Export] private int itemCount = 10;
	[Export] private PackedScene itemPrefab;
	[Export] private float minItemScale = 1;
	[Export] private float maxItemScale = 2;
	
	public override void _Ready()
	{
		path = GetNode<Path>(pathPath);
		if (spawnOnReady) SpawnItems();
	}
	
	public void SpawnItems()
	{
		for (int i = 0; i < itemCount; i ++)
		{
			var pathFollower = new PathFollow();
			var item = itemPrefab.Instance<Spatial>();
			if (randomizeRotation) item.Rotation = new Vector3(item.Rotation.x, (float) GD.RandRange(0, Mathf.Tau), item.Rotation.z);
			item.Scale = Vector3.One * (float) GD.RandRange(minItemScale, maxItemScale);
			float distanceAlongPath = (float) GD.RandRange(0, path.Curve.GetBakedLength());
			
			path.AddChild(pathFollower);
			pathFollower.Offset = distanceAlongPath;
			pathFollower.AddChild(item);
		}
	}

	
}
