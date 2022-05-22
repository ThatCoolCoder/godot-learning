using Godot;
using System;

public class MainMenu : Spatial
{
	private void _on_PlayButton_pressed()
	{
		// SceneManager.LoadLevel(1);
		GetTree().ChangeScene("res://Scenes/Main.tscn");
	}
}
