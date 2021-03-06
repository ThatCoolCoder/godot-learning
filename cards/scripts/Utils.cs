using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public static class Utils
{
    private static readonly Random random = new Random();
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }

    public static float AverageOfVector(Vector2 vector)
    {
        return (vector.x + vector.y) / 2;
    }
}