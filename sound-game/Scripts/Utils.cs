using Godot;

public static class Utils
{
    public static float RandRangeFloat(float min, float max)
    {
        return (float) GD.RandRange(min, max);
    }
}