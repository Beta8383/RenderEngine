using System;
using System.Numerics;

namespace RenderEngine.Core.Shader
{
	public interface IFragmentShader
	{
        Vector4 main(Vector3 position, Vector2 point, Vector4[] parameters);
    }
}