using Godot;
using System;

namespace Physics
{
    public class SailHinge : HingeJoint
    {
        // Adds a dynamic limit to the hinge joint to simulate the variable-length rope you can pull

        // Maximum value of the hinge joint when this is at maximal length
        [Export] public float MaximumOfMaximum = 60;

        // Speed at which the maximum changes
        [Export] public float MaximumChangeSpeed = 60;

        // Current setting of the maximum
        [Export] public float CurrentMaximumAngle { get; set; } = 60;

        [Export] public string IncreaseMaximumActionName { get; set; }
        [Export] public string DecreaseMaximumActionName { get; set; }

        public override void _PhysicsProcess(float delta)
        {
            if (Input.IsActionPressed(IncreaseMaximumActionName)) CurrentMaximumAngle += MaximumChangeSpeed * delta;
            if (Input.IsActionPressed(DecreaseMaximumActionName)) CurrentMaximumAngle -= MaximumChangeSpeed * delta;

            CurrentMaximumAngle = Mathf.Clamp(CurrentMaximumAngle, 0, MaximumOfMaximum);

            AngularLimit__upper = CurrentMaximumAngle;
            AngularLimit__lower = -CurrentMaximumAngle;
        }
    }
}