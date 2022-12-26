using Godot;
using System;
using System.Collections.Generic;


namespace Physics
{
	public class SpatialFluidEffectable : RigidBody
	{
		public List<Fluids.ISpatialFluid> Fluids { get; private set; } = new();
		private List<Forcers.AbstractSpatialFluidForcer> registeredForcers = new();

		public override void _Ready()
		{
			Fluids = Physics.Fluids.SpatialFluidRepository.FindSpatialFluidRepository(GetTree()).Fluids;
		}

		public override void _IntegrateForces(PhysicsDirectBodyState state)
		{
			registeredForcers.ForEach(f => f.Apply(state));
		}

		public void RegisterForcer(Forcers.AbstractSpatialFluidForcer forcer)
		{
			registeredForcers.Add(forcer);
		}

		public void UnregisterForcer(Forcers.AbstractSpatialFluidForcer forcer)
		{
			registeredForcers.Remove(forcer);
		}

	}
}
