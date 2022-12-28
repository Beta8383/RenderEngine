using System;
using System.Numerics;
using RenderEngine.Core.Shader;

namespace RenderEngine.Core.Components.Shape;

public abstract class ShapeBase
{
	public Matrix4x4 ModelTransform = Matrix4x4.Identity;

	static readonly DefaultVertexShader defaultVectexShader = new();
	static readonly DefaultFragmentShader defaultFragmentShader = new();

	internal virtual Vector3[] Vertices
	{
		get;
		set;
	}

	private IVertexShader? vectexShader;
	public IVertexShader VectexShader
	{
		get => vectexShader ?? defaultVectexShader;
		set => vectexShader = value;
	}

	private IFragmentShader? fragmentShader;
	public IFragmentShader FragmentShader
	{
		get => fragmentShader ?? defaultFragmentShader;
		set => fragmentShader = value;
	}

    public ShapeBase()
	{
		Vertices = Array.Empty<Vector3>();
	}

	public virtual int[] GetTriangleIndex() =>
		Array.Empty<int>();
}