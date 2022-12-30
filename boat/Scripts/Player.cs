using Godot;
using System;

public class Player : Spatial
{
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("switch_side"))
        {
            animationPlayer.CurrentAnimation = "LeftToRight";
            animationPlayer.Seek(animationPlayer.CurrentAnimationLength * animationPlayer.PlaybackSpeed / 2 + 0.5f);
            animationPlayer.PlaybackSpeed *= -1;
        }
    }
}