using System.Numerics;

namespace RenderEngine.Core.Components.Models;

public abstract class ModelBase
{
    internal Matrix4x4 ModelsTransform;
    internal Vector4 Color;

    public Vector3[] Vertices
    {
        get;
        protected set;
    }

    public int[] TriangleIndex
    {
        get;
        protected set;
    }

    public ModelBase()
    {
        Vertices = Array.Empty<Vector3>();
        TriangleIndex = Array.Empty<int>();
    }
}