using Godot;
using System.Linq;
using System.Collections.Generic;

namespace Physics.Forcers
{
    public abstract class AbstractSpatialFluidForcer : Spatial
    {
        // Path to the target of this forcer. If parent is a SpatialFluidEffectable and path is null, then parent is used
        [Export] public NodePath TargetPath { get; set; }
        [Export] public bool Enabled { get; set; } = true;
        [Export] public bool ForLiquid { get; set; } = true; // todo: get a proper system for setting the below list from the editor.
        private List<Fluids.FluidType> fluidTypes = new();
        protected SpatialFluidEffectable target { get; private set; }

        public override void _Ready()
        {
            if (GetParent() is SpatialFluidEffectable t && TargetPath == null) target = t;
            else target = GetNode<SpatialFluidEffectable>(TargetPath);

            target.RegisterForcer(this);

            if (ForLiquid) fluidTypes.Add(Fluids.FluidType.Liquid);
            else fluidTypes.Add(Fluids.FluidType.Gas);

            base._Ready();
        }

        // Should return a force in global coordinates
        public abstract Vector3 CalculateForce(Fluids.ISpatialFluid fluid, PhysicsDirectBodyState state);

        public void Apply(PhysicsDirectBodyState state)
        {
            if (!Enabled) return;

            var totalForce = target.Fluids.Where(f => fluidTypes.Contains(f.Type)).Select(f => CalculateForce(f, state)).Aggregate(Vector3.Zero, (prev, next) => prev + next);
            var position = GlobalTranslation;
            position -= target.GlobalTransform.origin;
            state.AddForce(totalForce, position);
        }

        public override void _ExitTree()
        {
            target.UnregisterForcer(this);
        }
    }
}