using System.Numerics;

namespace RenderEngine.Core.Components
{
    public enum ProjectionMode
    {
        Orthographic,
        Perspective
    }

    public struct Camera
    {
        public Vector4 Position, Up, Direction;
        public float Right, Left, Top, Bottom, Far, Near;
        public ProjectionMode Mode;
    }
}

