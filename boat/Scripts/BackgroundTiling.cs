using Godot;
using System;

public class BackgroundTiling : Sprite
{
	public override void _Ready()
	{
		// This gets reset by godot if we set it in the editor, so we need a script to fix it
		Texture.Flags = (uint)Texture.FlagsEnum.Repeat;
	}
}
