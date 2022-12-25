using Godot;
using System;

public static class Utils
{
    public static float MirrorNumber(float number, float mirrorValue)
    {
        if (number < mirrorValue) return number;
        else return mirrorValue - (number - mirrorValue);
    }

    public static float WrapNumber(float number, float min, float max)
    {
        float delta = max - min;
        while (number >= max) number -= delta;
        while (number < min) number += delta;
        return number;
    }

    public static float RoundNumber(float number, float roundTo)
    {
        return Mathf.Round(number / roundTo) * roundTo;
    }
}