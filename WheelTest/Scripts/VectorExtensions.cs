using Godot;
using System;

public static class VectorExtensions
{
    public static Vector3 WithX(this Vector3 vec, float x) => new Vector3(x, vec.y, vec.z);
    public static Vector3 WithY(this Vector3 vec, float y) => new Vector3(vec.x, y, vec.z);
    public static Vector3 WithZ(this Vector3 vec, float z) => new Vector3(vec.x, vec.y, z);

    public static Vector3 Random(Vector3 min, Vector3 max)
    {
        return new Vector3(
            (float)GD.RandRange(min.x, max.x),
            (float)GD.RandRange(min.y, max.y),
            (float)GD.RandRange(min.z, max.z)
        );
    }
}