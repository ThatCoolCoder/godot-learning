using Godot;
using System;

public class World3D : Spatial
{
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("restart")) GetTree().ReloadCurrentScene();
    }
}
