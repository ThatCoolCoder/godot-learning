using System;
using Godot;

namespace Physics.Forcers
{
    public abstract class AbstractSpatialFluidForcer : Spatial
    {
        // Path to the target of this forcer. If parent is a SpatialFluidEffectable and path is null, then parent is used
        public NodePath TargetPath { get; set; }
        protected SpatialFluidEffectable target { get; private set; }

        public override void _Ready()
        {
            if (GetParent() is SpatialFluidEffectable t && TargetPath == null) target = t;
            else target = GetNode<SpatialFluidEffectable>(TargetPath);
            base._Ready();
        }

        public abstract Vector3 CalculateForce();

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
        }
    }
}