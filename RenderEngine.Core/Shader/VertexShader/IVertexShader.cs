using System;
using System.Numerics;

namespace RenderEngine.Core.Shader
{
	public interface IVertexShader
	{
        Vector4 main(Vector3 position, in Matrix4x4 modelTransform, in Matrix4x4 transform, out Vector4[] parameters);
    }
}