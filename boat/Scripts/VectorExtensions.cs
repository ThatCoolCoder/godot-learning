using Godot;
using System;

public static class VectorExtensions
{
    public static Vector3 WithX(this Vector3 vec, float x) => new Vector3(x, vec.y, vec.z);
    public static Vector3 WithY(this Vector3 vec, float y) => new Vector3(vec.x, y, vec.z);
    public static Vector3 WithZ(this Vector3 vec, float z) => new Vector3(vec.x, vec.y, z);
}