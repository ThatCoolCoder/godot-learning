using Godot;
using System;

public class BaseLevel : Spatial
{
	[Export] public string WelcomeText = "";
	private PackedScene enemyPrefab = ResourceLoader.Load<PackedScene>("res://Scenes/Enemy.tscn");
	private Player player;

	public override void _Ready()
	{
		player = GetNode<Player>("Player");
		player.FoundGoal += () => GetNode<Timer>("NextLevelTimer").Start();
		player.Died += () => GetNode<Timer>("OnDieTimer").Start();

		var fullWelcomeText = $"Welcome to level {LevelManager.CrntLevelNumber}. {WelcomeText}";
		// freeze player
		// say fullWelcomeText
		// unfreeze player
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
