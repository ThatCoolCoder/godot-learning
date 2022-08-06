using Godot;
using System;

public static class LevelManager
{
    static int crntLevelNumber = 1;

    public static void LoadLevel(int levelNumber, SceneTree tree)
    {
        // Level numbers start from 1
        string levelPath = $"res://Scenes/Levels/{levelNumber:D2}.tscn";
        tree.ChangeScene(levelPath);
        crntLevelNumber = levelNumber;
    }

    public static void LoadNextLevel(SceneTree tree)
    {
        LoadLevel(++crntLevelNumber, tree);
    }

    public static void RestartCurrentLevel(SceneTree tree)
    {
        LoadLevel(crntLevelNumber, tree);
    }
}