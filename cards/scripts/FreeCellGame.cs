using Godot;
using System;
using System.Linq;

public class FreeCellGame : Node2D, IGameManager
{
	public event Action<GameResult> OnGameFinished;

	private Node2D foundationStackHolder;
	private Node2D freeCellHolder;
	private Node2D mainStackHolder;
	private CardStack deck;

	public override void _Ready()
	{
		foundationStackHolder = GetNode<Node2D>("FoundationStacks");
		freeCellHolder = GetNode<Node2D>("FreeCells");
		mainStackHolder = GetNode<Node2D>("MainStacks");
		deck = GetNode<CardStack>("Deck");
		
		LayoutCards();
	}
	
	private void LayoutCards()
	{
		deck.MakeFullDeck();
		deck.Shuffle();
		var mainStacks = mainStackHolder.GetChildren().Cast<CardStack>().ToList();
		while (deck.NumCards > 0)
		{
			foreach (var mainStack in mainStackHolder.GetChildren().Cast<CardStack>())
			{
				mainStack.AddTopCard(deck.RemoveTopCard());
				if (deck.NumCards == 0) break;
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
			else if (cardStack.TopCard.Rank + 1 == card.Rank && cardStack.TopCard.Suit == card.Suit) return true;
			else return false;
		}
		else if (freeCellHolder.GetChildren().Contains(cardStack))
		{
			return cardStack.NumCards == 0;
		}
		else if (mainStackHolder.GetChildren().Contains(cardStack))
		{
			if (cardStack.NumCards == 0) return true;
			else if (CardHelpers.SuitToColor[cardStack.TopCard.Suit] != CardHelpers.SuitToColor[card.Suit]
				&& cardStack.TopCard.Rank - 1 == card.Rank) return true;
			else return false;
		}
		else return false;
	}


	private void OnRestartButtonPressed()
	{
		GetTree().ReloadCurrentScene();
	}
}
