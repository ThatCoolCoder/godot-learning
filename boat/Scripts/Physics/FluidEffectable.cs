using Godot;
using System;
using System.Collections.Generic;

namespace Physics
{
    public class FluidEffectable : RigidBody2D
    {
        // Object which can be effected by fluids

        public Fluids.IFluid Fluid { get; set; }


        public Vector2 GetPointVelocity(Vector2 worldPoint)
        {
            var localPoint = ToLocal(worldPoint);

            var localAngularSpeed = AngularVelocity * localPoint.Length();

            var localAngularVelocity = localPoint.Rotated(Mathf.Deg2Rad(90) + GlobalRotation).Normalized() // velocity is tangenital to the position on the object
                 * localAngularSpeed;
            return LinearVelocity + localAngularVelocity;
        }
    }
}