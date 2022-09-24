using Godot;

namespace Physics.Fluids
{
    public interface IFluid
    {
        float DensityAtPoint(Vector2 point);
        Vector2 VelocityAtPoint(Vector2 point);
    }
}