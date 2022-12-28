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
        public Vector3 Position, Up, Direction;
        public float zNearPlane, zFarPlane, width, height;
        public ProjectionMode Mode;
    }
}

