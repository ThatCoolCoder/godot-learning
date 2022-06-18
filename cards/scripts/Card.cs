using Godot;
using System;
using System.Collections.Generic;

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

static class CardHelpers
{
	public static readonly Dictionary<Rank, string> RankToLetter = new()
	{
		{Rank.Ace, "A"},
		{Rank.Two, "2"},
		{Rank.Three, "3"},
		{Rank.Four, "4"},
		{Rank.Five, "5"},
		{Rank.Six, "6"},
		{Rank.Seven, "7"},
		{Rank.Eight, "8"},
		{Rank.Nine, "9"},
		{Rank.Ten, "10"},
		{Rank.King, "11"},
		{Rank.Queen, "12"},
		{Rank.Joker, "13"},
	};

	public static readonly Dictionary<Suit, string> SuitToLetter = new()
	{
		{Suit.Hearts, "H"},
		{Suit.Clubs, "C"},
		{Suit.Diamonds, "D"},
		{Suit.Spades, "S"},
	};
}

public class Card : Sprite
{
	[Export] public Rank Rank;
	[Export] public Suit Suit;
	[Export] public bool FrontSideUp = true;

	private Texture frontTexture = ResourceLoader.Load<Texture>("res://assets/Cards/card_front.svg");
	private Texture backTexture = ResourceLoader.Load<Texture>("res://assets/Cards/card_back.svg");
	private Dictionary<Suit, Texture> suitToTexture = new()
	{
		{Suit.Hearts, ResourceLoader.Load<Texture>("res://assets/Cards/Suits/hearts.svg")},
		{Suit.Clubs, ResourceLoader.Load<Texture>("res://assets/Cards/Suits/clubs.svg")},
		{Suit.Spades, ResourceLoader.Load<Texture>("res://assets/Cards/Suits/spades.svg")},
		{Suit.Diamonds, ResourceLoader.Load<Texture>("res://assets/Cards/Suits/diamonds.svg")}
	};

	private Node2D labelHolder;

	// (Top left and bottom right)
	private Label topLeftLabel;
	private Label bottomRightLabel;

	private Sprite topLeftSuit;
	private Sprite bottomRightSuit;

	public override void _Ready()
	{
		labelHolder = GetNode<Node2D>("LabelHolder");
		topLeftLabel = GetNode<Label>("LabelHolder/TopLeftLabel");
		bottomRightLabel = GetNode<Label>("LabelHolder/BottomRightLabel");
		topLeftSuit = GetNode<Sprite>("LabelHolder/TopLeftSuit");
		bottomRightSuit = GetNode<Sprite>("LabelHolder/BottomRightSuit");
		Update();
	}

	private new void Update()
	{
		Texture = FrontSideUp ? frontTexture : backTexture;
		if (FrontSideUp) labelHolder.Show();
		else labelHolder.Hide();

		topLeftLabel.Text = CardHelpers.RankToLetter[Rank];
		bottomRightLabel.Text = CardHelpers.RankToLetter[Rank];
		topLeftSuit.Texture = suitToTexture[Suit];
		bottomRightSuit.Texture = suitToTexture[Suit];
	}
}
