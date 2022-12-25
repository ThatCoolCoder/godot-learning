using Godot;
using System;

public class TestBox : Spatial
{
    private Physics.Fluids.ISpatialFluid fluid;
    public override void _Ready()
    {
        fluid = GetParent() as Physics.Fluids.ISpatialFluid;
    }

    public override void _Process(float delta)
    {
        GlobalTranslation = new Vector3(GlobalTranslation.x, fluid.HeightAtPoint(GlobalTranslation), GlobalTranslation.z);
    }
}