using Godot;
using System;

namespace Physics.Forcers
{
    [Tool]
    public class DragCube : Resource
    {
        [Export] public float CoefficientFront { get; set; } = 1;
        [Export] public float CoefficientBack { get; set; } = 1;
        [Export] public float CoefficientLeft { get; set; } = 1;
        [Export] public float CoefficientRight { get; set; } = 1;
        [Export] public float CoefficientTop { get; set; } = 1;
        [Export] public float CoefficientBottom { get; set; } = 1;

        public static DragCube LoadDefault()
        {
            return ResourceLoader.Load<DragCube>("res://Resources/PlaceholderDragCube.tres");
        }
    }
}