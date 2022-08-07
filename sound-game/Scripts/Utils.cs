using Godot;

public static class Utils
{
    public static float RandRangeFloat(float min, float max)
    {
        return (float) GD.RandRange(min, max);
    }

    public static float ConvergeValue(float value, float target, float increment)
    {
        // Move value towards target in steps of size increment.
        // If increment is negative can also be used to do the opposite

        float difference = value - target;
        if (Mathf.Abs(difference) < increment) return target;
        else return value + -Mathf.Sign(difference) * increment;
    }

}