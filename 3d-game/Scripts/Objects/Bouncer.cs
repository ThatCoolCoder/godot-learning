using Godot;
using System;

public class Bouncer : StaticBody, IBouncy
{
	[Export] public float Restitution { get; set; } = 2.0f;
}
