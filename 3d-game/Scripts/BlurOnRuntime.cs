using Godot;
using System;

public class BlurOnRuntime : WorldEnvironment
{
	// Turns on depth of field blur only when game is played so that it doesn't mess up the editor.
	// Do DoF settings as usual in editor but then set the amount variables to 0
	[Export] private float amountNear = 0.1f;
	[Export] private float amountFar = 0.1f;

	public override void _Ready()
	{
		Environment.DofBlurNearAmount = amountNear;
		Environment.DofBlurFarAmount = amountFar;
	}
}
