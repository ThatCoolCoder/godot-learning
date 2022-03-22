using Godot;
using System;

public enum Suit
{
	Hearts,
	Clubs,
	Diamonds,
	Spades
}

public enum Rank
{
	Ace,
	Two,
	Three,
	Four,
	Five,
	Six,
	Seven,
	Eight,
	Nine,
	Ten,
	Jack,
	Queen,
	King,
	Joker
}

public class Card : Sprite
{
	[Export] public Rank Rank;
	[Export] public Suit Suit;
	[Export] public bool FrontSideUp = true;
	[Export] public Texture FrontTexture;
	[Export] public Texture BackTexture;

	[Export] public NodePath TextPath;
	private Label text;

	public override void _Ready()
	{
		text = GetNode<Label>(TextPath);
		Update();
	}

	private void Update()
	{
		Texture = FrontSideUp ? FrontTexture : BackTexture;
		if (! FrontSideUp) text.Hide();
		else text.Show();
		text.Text = $"{Rank}, {Suit}";
	}
}
