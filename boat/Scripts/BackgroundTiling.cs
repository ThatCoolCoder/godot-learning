using Godot;
using System;

public class BackgroundTiling : Sprite
{
	public override void _Ready()
	{
		// This gets reset by godot, annoyingly
		Texture.Flags = (uint) Texture.FlagsEnum.Repeat;
	}
}
