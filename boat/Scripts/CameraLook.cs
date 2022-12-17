using Godot;
using System;

public class CameraLook : Spatial
{
    [Export] private Vector2 mouseSensitivity = Vector2.One * 0.002f;
    [Export] private float maxLookDown = -1.2f;
    [Export] private float maxLookUp = 1.2f;
    private Spatial head;

    public override void _Ready()
    {
        Input.MouseMode = Input.MouseModeEnum.Captured;
        head = GetNode<Spatial>("Camera");
    }

    public override void _Input(InputEvent _event)
    {
        // Rotate view with mouse
        if (_event is InputEventMouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            var motionEvent = _event as InputEventMouseMotion;
            RotateY(-motionEvent.Relative.x * mouseSensitivity.x);
            head.RotateX(motionEvent.Relative.y * mouseSensitivity.y);
            head.Rotation = new Vector3(Mathf.Clamp(head.Rotation.x, -maxLookUp, -maxLookDown),
                head.Rotation.y, head.Rotation.z);
        }
    }
}