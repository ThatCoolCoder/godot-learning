using Godot;
using System;

public class Main : Spatial
{
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("reset")) GetTree().ReloadCurrentScene();
    }
}
