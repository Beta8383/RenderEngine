using System;
using System.Numerics;

namespace RenderEngine.UI
{
	public class SolidColorVertexShader : IVertexShader
	{
        public readonly Vector4 Color;

        public SolidColorVertexShader(Vector4 color) => Color = color;

        public Vector4 main(Vector3 position, Matrix4x4 modelTransform, Matrix4x4 transform, out Vector4[] parameters)
        {
            parameters = new Vector4[] { Color };
            return Vector4.Transform(Vector4.Transform(new Vector4(position, 1), modelTransform), transform);
        }
    }
}