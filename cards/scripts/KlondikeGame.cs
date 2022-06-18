using Godot;
using System;
using System.Linq;

public class KlondikeGame : Node2D, IGameManager
{
	public event Action<GameResult> OnGameFinished;

	private Node2D foundationStackHolder;
	private Node2D mainStackHolder;
	private CardStack deck;

	public override void _Ready()
	{
		foundationStackHolder = GetNode<Node2D>("FoundationStacks");
		mainStackHolder = GetNode<Node2D>("MainStacks");
		deck = GetNode<CardStack>("Deck");
		
		LayoutCards();
	}
	
	private void LayoutCards()
	{
		deck.MakeFullDeck();
		deck.Shuffle();
		var mainStacks = mainStackHolder.GetChildren().Cast<CardStack>().ToList();
		for (int row = 0; row < 7; row ++)
		{
			for (int col = row; col < 7; col ++)
			{
				mainStacks[col].AddTopCard(deck.RemoveTopCard());
			}
		}
		GD.Print(mainStacks[0].NumCards);
	}

	public bool CanPickUpCard(Card card)
	{
		return card.ParentStack.IsTopCard(card);
	}

	public bool CanPutDownCard(Card card, CardStack cardStack)
	{
		if (foundationStackHolder.GetChildren().Contains(cardStack))
		{
			if (cardStack.NumCards == 0 && card.Rank == Rank.Ace) return true;
			else if (cardStack.TopCard.Rank + 1 == card.Rank) return true;
			else return false;
		}
		else if (mainStackHolder.GetChildren().Contains(cardStack))
		{
			if (cardStack.NumCards == 0 && card.Rank == Rank.King) return true;
			else if (CardHelpers.SuitToColor[cardStack.TopCard.Suit] != CardHelpers.SuitToColor[card.Suit]
				&& cardStack.TopCard.Rank - 1 == card.Rank) return true;
			else return false;
		}
		else return false;
	}
}
