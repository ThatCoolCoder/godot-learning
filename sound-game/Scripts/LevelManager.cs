using Godot;
using System;

public static class LevelManager
{
    public static int CrntLevelNumber { get; private set; } = 1;

    public static void LoadLevel(int levelNumber, SceneTree tree)
    {
        // Level numbers start from 1
        string levelPath = $"res://Scenes/Levels/{levelNumber:D2}.tscn";
        tree.ChangeScene(levelPath);
        CrntLevelNumber = levelNumber;
    }

    public static void LoadNextLevel(SceneTree tree)
    {
        LoadLevel(++CrntLevelNumber, tree);
    }

    public static void RestartCurrentLevel(SceneTree tree)
    {
        LoadLevel(CrntLevelNumber, tree);
    }
}