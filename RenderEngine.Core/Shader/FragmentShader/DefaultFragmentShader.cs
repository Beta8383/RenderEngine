using System;
using System.Numerics;

namespace RenderEngine.Core.Shader
{
	class DefaultFragmentShader : IFragmentShader
	{
        public Vector4 main(Vector3 position, Vector2 point, Vector4[] parameters) =>
            Vector4.Zero;
    }
}

