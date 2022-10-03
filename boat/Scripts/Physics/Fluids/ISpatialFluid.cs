using Godot;

namespace Physics.Fluids
{
    public interface ISpatialFluid
    {
        // 3D fluid that extends downwards to infinity but does have an upper bound.
        // All values passed in and out should be in global space

        // Density of the fluid at any given point - generally should get higher as point is lower
        // Can equal whatever if the height is above the surface level
        float DensityAtPoint(Vector3 point);

        // Y-position of the air-water boundary at any given point.
        float HeightAtPoint(Vector3 point);

        // Velocity of the fluid flow at any given point
        Vector3 VelocityAtPoint(Vector3 point);

        // Calculate the normal vector at any given point
        Vector3 NormalAtPoint(Vector3 point);

        public FluidType Type { get; set; }
    }
}