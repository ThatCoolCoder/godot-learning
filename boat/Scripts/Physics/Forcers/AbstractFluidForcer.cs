using Godot;

namespace Physics.Forcers
{

    public abstract class AbstractFluidForcer : Node2D
    {
        // Path to the target of this forcer. If parent is a FluidEffectable and path is null, then parent is used
        public NodePath TargetPath { get; set; }
        protected FluidEffectable target { get; private set; }

        public override void _Ready()
        {
            if (GetParent() is FluidEffectable t && TargetPath == null) target = t;
            else target = GetNode<FluidEffectable>(TargetPath);
            base._Ready();
        }

        public abstract Vector2 CalculateForce();
        public override void _PhysicsProcess(float delta)
        {
            target.ApplyImpulse(Position.Rotated(GlobalRotation), CalculateForce());
            base._PhysicsProcess(delta);
        }
    }
}