using Godot;
using System;

public enum GameResult
{
    Win,
    Lose
}

public interface IGameManager
{
    // Should be in group GameManager so that children can find it
    
    public event Action<GameResult> OnGameFinished;
    public bool CanPickUpCard(Card card);
    public bool CanPutDownCard(Card card, CardStack cardStack);
}