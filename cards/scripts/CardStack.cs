using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CardStack : Sprite
{
	private List<Card> cards = new List<Card>();
	[Export] public bool OffsetCards = true;
	[Export] public Vector2 CardOffset = new Vector2(0, 20);

	public CardStack()
	{
		;
	}

	public CardStack(List<Card> _cards)
	{
		foreach (var card in _cards) AddTopCard(card);
	}

	public static CardStack CreateFullDeck()
	{
		List<Card> cards = new List<Card>();
		foreach (var suit in Enum.GetValues(typeof(Suit)))
		{
			foreach (var rank in Enum.GetValues(typeof(Rank)))
			{
				cards.Add(new Card()
				{
					Rank = (Rank) rank,
					Suit = (Suit) suit
				});
			}
		}
		return new CardStack(cards);
	}

	public Card TopCard()
	{
		return cards.LastOrDefault();
	}

	public void AddTopCard(Card card)
	{
		// Add a card to the top of the stack
		AddChild(card);
		card.Position = cards.Count * CardOffset;
		cards.Add(card);
	}

	public Card RemoveTopCard()
	{
		// Pop top card and return it.
		// If there was no card on top returns null

		var card = cards.LastOrDefault();
		if (card != null) cards.RemoveAt(cards.Count - 1);
		return card;
	}

	public void Shuffle()
	{
		Utils.Shuffle(cards);
	}
}
