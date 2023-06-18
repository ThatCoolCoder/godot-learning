using Godot;
using System;

namespace Locations
{
    public class FreeCamera : Camera
    {
        // Keyboard movement stuff
        private Vector3 velocity; // (in local space)
        [Export] public float MaxSpeed { get; set; } = 10;
        [Export] public float Acceleration { get; set; } = 40;

        // Keyboard rotation stuff
        private Vector2 crntRotation;
        private Vector2 angularVelocityDegrees;
        [Export] public float MaxAngularSpeedDegrees { get; set; } = 120;
        [Export] public float AngularAccelerationDegrees { get; set; } = 480;

        // Mouse rotation stuff
        [Export] public float MouseSensitivity { get; set; } = 1;
        private Vector2 crntDragRotation;
        private Vector2? clickStartPos;

        public override void _Process(float delta)
        {
            if (Current)
            {
                KeyboardMovement(delta);

                Rotation = new Vector3(crntRotation.x + crntDragRotation.x, crntRotation.y + crntDragRotation.y, 0);
            }
        }

        private void KeyboardMovement(float delta)
        {
            var crntAcceleration = new Vector3(Input.GetActionStrength("cam_right") - Input.GetActionStrength("cam_left"),
                Input.GetActionStrength("cam_up") - Input.GetActionStrength("cam_down"),
                Input.GetActionStrength("cam_backward") - Input.GetActionStrength("cam_forward")) * Acceleration;

            velocity += crntAcceleration * delta;
            velocity = velocity.LimitLength(MaxSpeed);

            if (crntAcceleration.x == 0) velocity.x = Utils.ConvergeValue(velocity.x, 0, Acceleration * delta);
            if (crntAcceleration.y == 0) velocity.y = Utils.ConvergeValue(velocity.y, 0, Acceleration * delta);
            if (crntAcceleration.z == 0) velocity.z = Utils.ConvergeValue(velocity.z, 0, Acceleration * delta);

            Translation += Transform.basis.Xform(velocity * delta);
        }

        public override void _Input(InputEvent _event)
        {
            if (!Current) return;

            if (_event is InputEventMouseButton clickEvent)
            {
                if (clickEvent.Pressed)
                {
                    clickStartPos = clickEvent.Position;
                }
                else
                {
                    crntRotation.x += crntDragRotation.x;
                    crntRotation.y += crntDragRotation.y;
                    crntDragRotation = Vector2.Zero;
                    clickStartPos = null;
                }
            }
            else if (_event is InputEventMouseMotion motionEvent)
            {
                if (clickStartPos != null)
                {
                    var movement = motionEvent.Position - (Vector2)clickStartPos;
                    crntDragRotation.x = -movement.y * MouseSensitivity * 0.001f;
                    crntDragRotation.y = -movement.x * MouseSensitivity * 0.001f;
                }
            }
        }
    }
}
