using Godot;
using System;

namespace Physics
{
    public static class PhysicsDirectBodyStateExtensions
    {
        public static Vector3 GetVelocityAtGlobalPosition(this PhysicsDirectBodyState state, RigidBody body, Vector3 globalPosition)
        {
            // Unfortunately looks like we can't lookup body position from the state, so we have to pass in the body separately - ugh.
            return state.GetVelocityAtLocalPosition(globalPosition - body.GlobalTranslation);
        }

        public static Vector3 GetVelocityAtGlobalPosition(this PhysicsDirectBodyState state, RigidBody body, Spatial measuringSpatial)
        {
            // shortcut override for when the global position you want is that of a spatial
            return state.GetVelocityAtGlobalPosition(body, measuringSpatial.GlobalTranslation);
        }
    }
}