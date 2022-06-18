using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public enum Suit
{
	Hearts,
	Clubs,
	Diamonds,
	Spades
}

public enum SuitColor
{
	Red,
	Black
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

public class Card : Area2D
{
	[Export] public Rank Rank;
	[Export] public Suit Suit;
	[Export] public bool FrontSideUp = true;

	#region Textures
	private Texture frontTexture = ResourceLoader.Load<Texture>("res://assets/Cards/card_front.svg");
	private Texture backTexture = ResourceLoader.Load<Texture>("res://assets/Cards/card_back.svg");
	private Dictionary<Suit, Texture> suitToTexture = new()
	{
		{Suit.Hearts, ResourceLoader.Load<Texture>("res://assets/Cards/Suits/hearts.svg")},
		{Suit.Clubs, ResourceLoader.Load<Texture>("res://assets/Cards/Suits/clubs.svg")},
		{Suit.Spades, ResourceLoader.Load<Texture>("res://assets/Cards/Suits/spades.svg")},
		{Suit.Diamonds, ResourceLoader.Load<Texture>("res://assets/Cards/Suits/diamonds.svg")}
	};
	#endregion Textures

	#region NodeReferences
	private IGameManager gameManager;
	private Sprite mainSprite;
	private Node2D labelHolder;

	// (Top left and bottom right)
	private Label topLeftLabel;
	private Label bottomRightLabel;

	private Sprite topLeftSuit;
	private Sprite bottomRightSuit;
	#endregion NodeReferences

	public override void _Ready()
	{
		gameManager = GetTree().GetNodesInGroup("GameManager").Cast<Node>().First() as IGameManager;
		mainSprite = GetNode<Sprite>("MainSprite");
		labelHolder = GetNode<Node2D>("LabelHolder");
		topLeftLabel = GetNode<Label>("LabelHolder/TopLeftLabel");
		bottomRightLabel = GetNode<Label>("LabelHolder/BottomRightLabel");
		topLeftSuit = GetNode<Sprite>("LabelHolder/TopLeftSuit");
		bottomRightSuit = GetNode<Sprite>("LabelHolder/BottomRightSuit");
		Update();
	}

	private new void Update()
	{
		// todo: is overriding CanvasItem.Update bad in any way?
		mainSprite.Texture = FrontSideUp ? frontTexture : backTexture;
		if (FrontSideUp) labelHolder.Show();
		else labelHolder.Hide();

		topLeftLabel.Text = CardHelpers.RankToLetter[Rank];
		bottomRightLabel.Text = CardHelpers.RankToLetter[Rank];
		topLeftSuit.Texture = suitToTexture[Suit];
		bottomRightSuit.Texture = suitToTexture[Suit];
	}

	#region GameLogic

	public CardStack ParentStack { get { return GetParent<CardStack>(); } }

	private bool CanBePickedUp()
	{
		return gameManager.CanPickUpCard(this);
	}

	private void OnDropped()
	{
		ZIndex -= 100; // todo: proper system for raising cards

		var spaceState = GetWorld2d().DirectSpaceState;
		var cardStack = GetTree().GetNodesInGroup("CardStackDropArea").Cast<Area2D>().Where(area => {
			var shape = area.ShapeOwnerGetShape(0, 0);
			if (shape is RectangleShape2D rectShape)
			{
				var rect = new Rect2(area.GlobalPosition - rectShape.Extents / 2, rectShape.Extents);
				if (rect.HasPoint(GlobalPosition)) return true;
			}
			return false;
		}).Select(x => x.GetParent()).Cast<CardStack>().FirstOrDefault();
		if (cardStack != null && gameManager.CanPutDownCard(this, cardStack))
		{
			ParentStack.RemoveCard(this);
			cardStack.AddTopCard(this);
		}
		else ParentStack.UpdateCardPosition(this);
	}

	#endregion GameLogic

	#region Dragging

	private bool beingDragged = false;
	private Vector2 touchPosition = Vector2.Zero;

	public override void _Input(InputEvent _event)
	{
		if (! beingDragged) return;

		if (_event.IsActionReleased("ui_touch"))
		{
			beingDragged = false;
			OnDropped();
		}

		if (_event is InputEventMouseMotion eventMouseMotion)
		{
			Position -= (touchPosition - eventMouseMotion.Position) / Utils.AverageOfVector(GlobalScale);
			touchPosition = eventMouseMotion.Position;
		}
	}

	private void OnInputEvent(Viewport _viewport, InputEvent _event, int _shapeIdx)
	{
		if (_event is InputEventMouseButton eventMouseButton)
		{
			if (eventMouseButton.IsActionPressed("ui_touch") && IsOnTop() && CanBePickedUp())
			{
				ZIndex += 100; // todo: proper system for raising cards
				beingDragged = true;
				touchPosition = eventMouseButton.Position;
			}
		}
	}

	private bool IsOnTop()
	{
		foreach (var node in GetTree().GetNodesInGroup("HoveredCard").Cast<Node2D>())
		{
			if (node.ZIndex > ZIndex) return false;
		}
		return true;
	}

	private void OnMouseEntered()
	{
		AddToGroup("HoveredCard");
	}

	private void OnMouseExited()
	{
		RemoveFromGroup("HoveredCard");
	}


	#endregion Dragging
}
