using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
