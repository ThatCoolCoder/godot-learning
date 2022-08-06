using Godot;
using System;

public class BaseLevel : Spatial
{
	[Export] private float enemyChance = 1f/360f;
	[Export] private float maxPosition = 20;
	private PackedScene enemyPrefab = ResourceLoader.Load<PackedScene>("res://Scenes/Enemy.tscn");
	private Player player;

	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.FoundGoal += () => GetNode<Timer>("NextLevelTimer").Start();
		player.Died += () => GetNode<Timer>("OnDieTimer").Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(float delta)
	{
		if (GD.Randf() < enemyChance)
		{
			var instance = enemyPrefab.Instance<Enemy>();
			instance.Translation = new Vector3(Utils.RandRangeFloat(-maxPosition, maxPosition), 0.25f, Utils.RandRangeFloat(-maxPosition, maxPosition));
			AddChild(instance);
		}
	}

	private void OnNextLevelTimerTiimeout()
	{
		LevelManager.LoadNextLevel(GetTree());
	}


	private void OnDieTimerTimeout()
	{
		LevelManager.RestartCurrentLevel(GetTree());
	}
}
