using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace RenderEngine.Core.Shader
{
    public abstract class FragmentShader : ShaderBase
    {
        uint Color;
        public virtual void main()
        {
            Color = 0;
        }
    }
}
