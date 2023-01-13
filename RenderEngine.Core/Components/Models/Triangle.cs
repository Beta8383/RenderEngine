using System;
using System.Numerics;

namespace RenderEngine.Core.Components.Models;

public class Triangle : ModelBase
{
    public Vector3 p1, p2, p3;

    public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;
        Vertices = new Vector3[] { p1, p2, p3 };
        TriangleIndex = new int[] { 0 };
    }
}

