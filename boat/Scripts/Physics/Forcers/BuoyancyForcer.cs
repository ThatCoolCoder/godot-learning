using Godot;

namespace Physics.Forcers
{
    public class BuonancyForcer : AbstractSpatialFluidForcer
    {
        // todo: port this over: https://github.com/saurus/Buoyancy.
        // or possibly this to start: https://godotforums.org/d/27725-creating-a-water-buoyancy-system/33

        [Export] public Curve AoaToCoefficient { get; set; }
        [Export] public Curve AoaToAreaProportion { get; set; }
        [Export] public Mesh Shape { get; set; }

        [Export] public float AreaMultiplier = 1;

        private float GetSeaLevelAtPoint(Vector3 point)
        {
            return target.Fluid.HeightAtPoint(point);
        }

        // need to implement GetSeaPlane?
        // need to implement GetSeaTransformAtPoint?
        
        public override Vector3 CalculateForce()
        {
            // get sea plane
            // calculate buoyancy data
            // apply the buoyancy
            return Vector3.Zero;
        }
    }
}