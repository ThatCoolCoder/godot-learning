using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Vehicle : RigidBody
{
    [Export] public List<NodePath> Steerables { get; set; } = new();
    private List<Spatial> steerables;
    [Export] public List<NodePath> Powereds { get; set; } = new();
    private List<Wheel> powereds;
    [Export] public List<NodePath> Brakeds { get; set; } = new();
    private List<Wheel> brakeds;

    [Export] public float PowerTorque { get; set; } = 10;
    [Export] public float BrakeTorque { get; set; } = 10;

    [Export] public float SteerAngleDegrees { get; set; } = 30;

    public override void _Ready()
    {
        steerables = Steerables.Select(x => GetNode<Spatial>(x)).ToList();
        powereds = Powereds.Select(x => GetNode<Wheel>(x)).ToList();
        brakeds = Brakeds.Select(x => GetNode<Wheel>(x)).ToList();
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionPressed("forward")) powereds.ForEach(x => x.ApplyPowerTorque(PowerTorque / powereds.Count));
        if (Input.IsActionPressed("back")) powereds.ForEach(x => x.ApplyPowerTorque(-PowerTorque / powereds.Count));
        if (Input.IsActionPressed("brake")) brakeds.ForEach(x => x.ApplyBrakeTorque(BrakeTorque / brakeds.Count));

        float angle = 0;
        if (Input.IsActionPressed("left")) angle = SteerAngleDegrees;
        else if (Input.IsActionPressed("right")) angle = -SteerAngleDegrees;
        steerables.ForEach(x => x.Rotation = x.Rotation.WithY(Mathf.Deg2Rad(angle)));
    }
}
