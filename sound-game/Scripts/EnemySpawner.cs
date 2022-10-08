using Godot;
using System;

public class EnemySpawner : Spatial
{
    [Export] public bool Enabled = true;
    [Export] public float EnemyChance = 0.03f; // chance of spawning an enemy on any given physics frame
    [Export] public float DelayBeforeFirstEnemy = 5.0f; // delay in seconds after scene is loaded, to give player some time to work out what's happening
    [Export] public float EnemyDistance = 10;
    [Export] public PackedScene EnemyPrefab;
    private float timeSinceSceneLoad = 0;

    public override void _PhysicsProcess(float delta)
    {
        timeSinceSceneLoad += delta;
        if (Enabled && timeSinceSceneLoad >= DelayBeforeFirstEnemy && GD.Randf() <= EnemyChance)
        {
            var instance = EnemyPrefab.Instance<Spatial>();
            GetParent().AddChild(instance);
            var trans = GlobalTransform;
            trans.origin.x += Utils.RandRangeFloat(-EnemyDistance, EnemyDistance);
            trans.origin.z += Utils.RandRangeFloat(-EnemyDistance, EnemyDistance);
            instance.GlobalTransform = trans;
        }

        base._PhysicsProcess(delta);
    }
}
