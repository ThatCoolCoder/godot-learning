using Godot;
using System;

public class SceneManager : Node
{
    // Scene and level manager class.
    // Main methods are static but you need to add an instance to your scene
    // as the singleton pattern is used to facilitate calls to GetTree()

    public static int CurrentLevelNumber { get; private set; } // Starts from 1
    private static readonly string scenePathFormat = "res://Scenes/Levels/Level{0:00}.tscn";
    private static readonly string mainMenuPath = "res://Scenes/UI/MainMenu.tscn";
    private static SceneManager instance; // needed so we can make calls to GetTree etc

    public SceneManager()
    {
        instance = this;
    }

    public static void LoadLevel(int levelNumber)
    {
        if (WarnIfInstanceNull()) return;

        GD.Print(String.Format(scenePathFormat, levelNumber));
        var result = instance.GetTree().ChangeScene(String.Format(scenePathFormat, levelNumber));
        if (result == Error.Ok) CurrentLevelNumber = levelNumber;
        else GD.Print($"Failed loading level #{levelNumber}: {result}");
    }

    public static void RestartCurrentLevel()
    {
        if (WarnIfInstanceNull()) return;

        instance.GetTree().ReloadCurrentScene();
    }

    public static void LoadMainMenu()
    {
        if (WarnIfInstanceNull()) return;

        instance.GetTree().ChangeScene(mainMenuPath);
    }

    private static bool WarnIfInstanceNull()
    {
        if (instance == null) GD.PrintErr("SceneManager: instance is null, cannot load scenes");
        return instance == null;
    }
}