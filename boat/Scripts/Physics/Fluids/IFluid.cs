using Godot;

namespace Physics.Fluids
{

    public interface IFluid
    {
        public FluidType Type { get; set; }
        float DensityAtPoint(Vector2 point);
        Vector2 VelocityAtPoint(Vector2 point);
    }
}