using Godot;
using System;

namespace Physics
{

    public class SpatialRudder : Spatial
    {
        [Export] public string TurnLeftAction { get; set; } = "";
        [Export] public string TurnRightAction { get; set; } = "";
        [Export]
        public float MaxDeflectionDegrees { get; set; } = 0;
        [Export]
        public float DeflectionDegrees { get; set; } = 0;
        [Export] public float ActuationSpeedDegrees { get; set; } = 0;

        public override void _PhysicsProcess(float delta)
        {
            if (Input.IsActionPressed(TurnLeftAction)) DeflectionDegrees -= ActuationSpeedDegrees * delta;
            if (Input.IsActionPressed(TurnRightAction)) DeflectionDegrees += ActuationSpeedDegrees * delta;

            DeflectionDegrees = Mathf.Clamp(DeflectionDegrees, -MaxDeflectionDegrees, MaxDeflectionDegrees);
            Rotation = Rotation.WithY(Mathf.Deg2Rad(DeflectionDegrees));
        }
    }
}