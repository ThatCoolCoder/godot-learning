using Godot;
using System;

public class HUD : Control
{
	[Export] private NodePath playerPath;
	private Player player;
	private Label label;
	public override void _Ready()
	{
		player = GetNode<Player>(playerPath);
		label = GetNode<Label>("StateLabel");
	}

	public override void _Process(float delta)
	{
		label.Text = $"State: {player.State}";
	}
}
