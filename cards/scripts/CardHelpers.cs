using Godot;
using System;
using System.Collections.Generic;

public static class CardHelpers
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
		{Rank.Jack, "J"},
		{Rank.Queen, "Q"},
		{Rank.King, "K"},
		{Rank.Joker, "?"},
	};

	public static readonly Dictionary<Suit, string> SuitToLetter = new()
	{
		{Suit.Hearts, "H"},
		{Suit.Clubs, "C"},
		{Suit.Diamonds, "D"},
    };

	public static readonly Dictionary<Suit, SuitColor> SuitToColor = new()
	{
		{Suit.Hearts, SuitColor.Red},
		{Suit.Clubs, SuitColor.Black},
		{Suit.Diamonds, SuitColor.Red},
		{Suit.Spades, SuitColor.Black},
	};
}