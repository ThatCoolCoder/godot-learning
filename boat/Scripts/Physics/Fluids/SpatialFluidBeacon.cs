using Godot;
using System;
using System.Linq;

namespace Physics.Fluids
{
    // Class that registers a ISpatialFluid (normally its parent, but can have a custom node path) in a SpatialFluidRepository
    public class SpatialFluidBeacon : Spatial
    {
        [Export] public NodePath FluidPath { get; set; } = null;

        public override void _Ready()
        {
            var repository = SpatialFluidRepository.FindSpatialFluidRepository(GetTree());
            if (GetParent() is ISpatialFluid f)
            {
                repository.AddFluid(f);
            }
            else if (FluidPath != null)
            {
                repository.AddFluid(GetNode<ISpatialFluid>(FluidPath));
            }
            else
            {
                GD.PrintErr($"SpatialFluidBeacon is not a child of an ISpatialFluid and has no FluidPath defined ({GetPath()})");
            }
        }
    }
}