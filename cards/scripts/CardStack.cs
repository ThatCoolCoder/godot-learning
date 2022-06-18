using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CardStack : Sprite
{
	private List<Card> cards = new List<Card>();
	[Export] public bool OffsetCards = true;
	[Export] public Vector2 CardOffset = new Vector2(0, 20);
	public Vector2 AppliedCardOffset { get { return OffsetCards ? CardOffset : Vector2.Zero; } }
	[Export] public bool FullDeckOnStart = false;
	[Export] public bool JokersInFullDeck = false;
	[Export] public bool ShuffleOnStart = false;

	private Area2D dropArea;
	private PackedScene cardPrefab = ResourceLoader.Load<PackedScene>("res://scenes/Card.tscn");

	public CardStack()
	{
		;
	}

	public CardStack(List<Card> _cards)
	{
		foreach (var card in _cards) AddTopCard(card);
	}

	public override void _Ready()
	{
		dropArea = GetNode<Area2D>("DropArea");
		if (FullDeckOnStart)
		{
			MakeFullDeck(JokersInFullDeck);
			if (ShuffleOnStart) Shuffle();
		}
	}

	public void MakeFullDeck(bool includeJokers = false)
	{
		// Fill self so that it is a full deck
		foreach (var suit in Enum.GetValues(typeof(Suit)))
		{
			foreach (var rank in Enum.GetValues(typeof(Rank)))
			{
				if ((Rank) rank == Rank.Joker && ! JokersInFullDeck) continue;
				var card = cardPrefab.Instance<Card>();
				card.Rank = (Rank) rank;
				card.Suit = (Suit) suit;
				AddTopCard(card);
			}
		}
	}

	#region CardManagement

	public int NumCards { get { return cards.Count(); } }

	public Card TopCard { get {return cards.LastOrDefault(); } }

	public void AddTopCard(Card card)
	{
		// Add a card to the top of the stack
		AddChild(card);
		cards.Add(card);
		UpdateCardPosition(card);
		card.ZIndex = cards.IndexOf(card);
		UpdateDropAreaPosition();
	}

	public Card RemoveTopCard()
	{
		// Pop top card and return it.
		// If there was no card on top returns null

		var card = cards.LastOrDefault();
		if (card != null) cards.RemoveAt(cards.Count - 1);
		UpdateDropAreaPosition();
		RemoveChild(card);
		return card;
	}

	public bool IsTopCard(Card card)
	{
		return cards.IndexOf(card) == cards.Count() - 1;
	}

	public void RemoveCard(Card card)
	{
		cards.Remove(card);
		UpdateDropAreaPosition();
		RemoveChild(card);
	}


	public void Shuffle()
	{
		cards.Shuffle();
		UpdateZIndexes();
	}

	#endregion CardManagement

	#region Visuals

	public void UpdateCardPosition(Card card)
	{
		card.Position = cards.IndexOf(card) * AppliedCardOffset;
	}

	public void UpdateCardPositions()
	{
		int index = 0;
		foreach (var card in cards)
		{
			card.Position = index * AppliedCardOffset;
			index ++;
		}
	}

	private void UpdateDropAreaPosition()
	{
		if (cards.Count() > 0) dropArea.Position = cards.Last().Position;
		else dropArea.Position = Vector2.Zero;
	}

	private void UpdateZIndexes()
	{
		// Need to call this after reorganising the cards in the stack
		int index = 0;
		foreach (var card in cards)
		{
			card.ZIndex = index;
			index ++;
		}
		UpdateCardPositions();
	}

	#endregion Visuals
}
