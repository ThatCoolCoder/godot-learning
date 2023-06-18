using Godot;
using System;

public class Wheel : SpringArm
{
    // Based heavily on https://www.gtplanet.net/forum/threads/gdsim-v0-4a-autocross-and-custom-setups.396400/

    [Export] public NodePath targetPath;
    private RigidBody target;
    private RayCast rayCast;

    [Export] public float SpringStrength { get; set; }
    [Export] public float SpringDamp { get; set; }

    [Export] public float TirePeak { get; set; } = 1;
    [Export] public float TireXShape { get; set; } = 1.35f;
    [Export] public float TireZShape { get; set; } = 1.65f;
    [Export] public float TireStiff { get; set; } = 10;
    [Export] public float TireCurve { get; set; } = 0;

    [Export] public float Mass { get; set; } = 10;

    [Export] public NodePath Mesh { get; set; }
    private Spatial mesh;

    public float Torque = 0;

    private float radius;
    private float width;
    private float prevCompression;
    private Vector3 prevPosition;
    private float prevZForce;
    private float angularVelocity;
    private float momentOfInertia
    {
        get
        {
            return .5f * Mass * radius * radius;
        }
    }
    private float crntPowerTorque;
    private float crntBrakeTorque;

    public override void _Ready()
    {
        target = GetNode<RigidBody>(targetPath);
        rayCast = GetNode<RayCast>("RayCast");
        mesh = GetNode<Spatial>(Mesh);

        radius = (Shape as SphereShape).Radius;
    }

    public override void _Process(float delta)
    {
        mesh.RotateX(-angularVelocity * delta);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (target == null) return;

        ApplyForce(delta);
    }

    private void ApplyForce(float delta)
    {
        // Torques, inertia

        var netTorque = crntPowerTorque + (radius * prevZForce);
        var pureBraking = crntBrakeTorque > Mathf.Abs(netTorque);
        netTorque -= crntBrakeTorque * Mathf.Sign(angularVelocity);

        if (pureBraking)
        {
            // Apply pure braking, converge to 0
            angularVelocity = Utils.ConvergeValue(angularVelocity, 0, Mathf.Abs(netTorque) / momentOfInertia * delta);
        }
        else
        {
            // Don't converge to 0
            angularVelocity += netTorque / momentOfInertia * delta;
        }

        crntPowerTorque = 0;
        crntBrakeTorque = 0;

        if (rayCast.IsColliding())
        {
            // Suspension
            var compression = 1 - (GetHitLength() / SpringLength);
            var springForce = SpringStrength * compression;
            springForce += SpringDamp * (compression - prevCompression) / delta;

            var contactPoint = rayCast.GetCollisionPoint() - target.GlobalTranslation;

            // tire
            var localVelocity = GlobalTransform.basis.XformInv((GlobalTranslation - prevPosition) / delta);
            var zVel = -localVelocity.y; // because raycast needs to be rotated
            var directionVector = new Vector2(localVelocity.x, localVelocity.y).Normalized();
            prevPosition = GlobalTranslation;

            var xSlip = Mathf.Asin(Mathf.Clamp(-directionVector.x, -1, 1));
            var xForce = Pacejka(springForce, TirePeak, TireXShape, TireStiff, xSlip, TireCurve);

            float zSlip = 0;
            if (zVel != 0) zSlip = (radius * angularVelocity - zVel) / Mathf.Abs(zVel);

            var zForce = Pacejka(springForce, TirePeak, TireZShape, TireStiff, zSlip, TireCurve);

            // Add all the forces
            target.AddForce(GlobalTransform.basis.x * xForce, contactPoint);
            target.AddForce(springForce * rayCast.GetCollisionNormal(), contactPoint);
            target.AddForce(-GlobalTransform.basis.y * zForce, contactPoint);

            prevZForce = -zForce;
            prevCompression = compression;
        }
        else
        {
            prevZForce = 0;
            prevCompression = 0;
        }
    }

    public void ApplyPowerTorque(float torque)
    {
        crntPowerTorque += torque;
    }

    public void ApplyBrakeTorque(float torque)
    {
        crntBrakeTorque += torque;
    }

    private float Pacejka(float normalForce, float peak, float tireShape, float stiff, float slip, float curve)
    {
        // Pacejka tire formula
        return normalForce * peak * Mathf.Sin(tireShape * Mathf.Atan(stiff * slip - curve * stiff * slip - Mathf.Atan(stiff * slip)));
    }
}
