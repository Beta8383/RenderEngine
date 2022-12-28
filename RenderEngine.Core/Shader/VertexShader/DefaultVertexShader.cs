using System;
using System.Numerics;

namespace RenderEngine.Core.Shader
{
	public class DefaultVertexShader : IVertexShader
	{
        public Vector4 main(Vector3 position, in Matrix4x4 modelTransform, in Matrix4x4 transform, out Vector4[] para)
        {
            para = Array.Empty<Vector4>();
            return Vector4.Transform(Vector4.Transform(new Vector4(position, 1), modelTransform), transform);
        }
    }
}