using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RenderEngine.Core.Shader
{
    public abstract class VertexShader : ShaderBase
    {
        public Vector4 ActualPosition, Position;
        public Matrix4x4 Rotate;

        public virtual void main()
        {
            ActualPosition = Vector4.Transform(Position, Rotate);
        }
    }
}
