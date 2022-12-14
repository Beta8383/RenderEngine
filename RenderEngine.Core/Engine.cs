using System.Numerics;
using RenderEngine.Core.Components;
using RenderEngine.Core.Shader;

namespace RenderEngine.Core;

public class Engine
{
    private uint[] pixels;
    private int width, height;

    private float[,] zBuffer;
    private int[,] coveredTriangle;

    private VertexShader[] vertexShader;
    private FragmentShader[] fragmentShader;

    private Vector4[] vertices;
    private readonly Dictionary<int, int> vertexToShader;

    private int[] triangles;
    private readonly Dictionary<int, int> triangleToShader;

    Matrix4x4 cameraTransform = Matrix4x4.Identity,
              viewportTransform = Matrix4x4.Identity,
              projectionTransform = Matrix4x4.Identity,
              transform = Matrix4x4.Identity;

    public Engine() : this(1600, 900) { }

    public Engine(int width, int height)
    {
        vertexToShader = new Dictionary<int, int>();
        triangleToShader = new Dictionary<int, int>();
        SetResolution(width, height);
    }

    private bool GetBarycentricCoordinate(Vector4 p1, Vector4 p2, Vector4 p3, float x, float y, out float i, out float j, out float k)
    {
        float a = p1.Y - p3.Y;
        float b = p1.Y - p3.Y;
        float c = (p2.Y - p3.Y) * b - (p2.Y - p3.Y) * a;
        j = ((y - p3.Y) * b - (x - p3.X) * a) / c;

        float a1 = p3.Y - p2.Y;
        float b1 = p3.X - p2.X;
        float c1 = (p1.Y - p2.Y) * b1 - (p1.X - p2.X) * a1;
        i = ((y - p2.Y) * b1 - (x - p2.X) * a1) / c1;

        k = 1 - i - j;
        return i > 0 && j > 0 && k > 0;
    }

    public void SetResolution(int width, int height)
    {
        this.width = width;
        this.height = height;

        viewportTransform = new(
            width / 2.0f, 0, 0, -width / 2.0f,
            0, height / 2.0f, 0, -height / 2.0f,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        zBuffer = new float[height, width];
        coveredTriangle = new int[height, width];
        pixels = new uint[width * height];
    }

    private float Min(float x, float y, float z) => MathF.Min(MathF.Min(x, y), z);
    private float Max(float x, float y, float z) => MathF.Max(MathF.Max(x, y), z);

    public void CaculateZBuffer()
    {
        for (int index = 0; index < triangles.Length; index++)
        {
            int startIndex = triangles[index];

            int minx = (int)Min(p1.X, p2.X, p3.X);
            int miny = (int)Min(p1.Y, p2.Y, p3.Y);
            int maxx = (int)Max(p1.X, p2.X, p3.X) + 1;
            int maxy = (int)Max(p1.Y, p2.Y, p3.Y) + 1;

            if (minx < 0) minx = 0;
            if (miny < 0) miny = 0;
            if (minx >= width) minx = width - 1;
            if (miny >= height) miny = height - 1;

            float i, j, k, depth;

            for (int x = minx; x <= maxx; x++)
                for (int y = miny; y <= maxy; y++)
                {
                    if (GetBarycentricCoordinate(p1, p2, p3, (float)(x + 0.5), (float)(y + 0.5), out i, out j, out k))
                    {

                    }
                }
        }
    }

    public ReadOnlySpan<uint> Render()
    {
        Reset();
        transform = viewportTransform * projectionTransform * cameraTransform;
        
        return new ReadOnlySpan<uint>(pixels);
    }

    private void Reset()
    {
        pixels.SetValue(0, width * height);
        zBuffer.SetValue(-2, height, width);
        coveredTriangle.SetValue(-1, height, width);
    }

    #region Init Vectices
    public void SetVectexShader(VertexShader[] shaders) => vertexShader = shaders;

    public void SetVertices(Vector4[] vertices)
    {
        this.vertices = vertices;
        actualVertices = new Vector4[vertices.Length];
    }

    public void BindVectexShader(int vertexIndex, int shaderIndex)
    {
        if (vertexIndex >= vertices.Length || vertexIndex < 0)
            return;
        if (shaderIndex >= vertexShader.Length || shaderIndex < 0)
            return;

        if (vertexToShader.ContainsKey(vertexIndex))
            vertexToShader[vertexIndex] = shaderIndex;
        else
            vertexToShader.Add(vertexIndex, shaderIndex);
    }
    #endregion

    #region Init Triangle
    public void SetFragmentShader(FragmentShader[] shaders) => fragmentShader = shaders;

    public void SetTriangle(int[] startIndex) => triangles = startIndex;

    public void BindTriangleShader(int triangleIndex, int shaderIndex)
    {
        if (triangleIndex >= triangles.Length || triangleIndex < 0)
            return;
        if (shaderIndex >= fragmentShader.Length || shaderIndex > 0)
            return;

        if (triangleToShader.ContainsKey(triangleIndex))
            triangleToShader[triangleIndex] = shaderIndex;
        else
            triangleToShader.Add(triangleIndex, shaderIndex);
    }
    #endregion

    public void SetCamera(Camera camera)
    {
        Vector4 xaxis, yaxis, zaxis;

        zaxis = -(camera.Direction - camera.Position);
        zaxis = Vector4.Normalize(zaxis);

        xaxis = Vector4.Multiply(camera.Up, zaxis);
        xaxis = Vector4.Normalize(xaxis);

        yaxis = Vector4.Multiply(zaxis, xaxis);
        yaxis = Vector4.Normalize(yaxis);

        var translate = new Matrix4x4(
            1, 0, 0, -camera.Position.X,
            0, 1, 0, -camera.Position.Y,
            0, 0, 1, -camera.Position.Z,
            0, 0, 0, 1);

        var rotate = new Matrix4x4(
            xaxis.X, xaxis.Y, xaxis.Z, 0,
            yaxis.X, yaxis.Y, yaxis.Z, 0,
            zaxis.X, zaxis.Y, zaxis.Z, 0,
            0, 0, 0, 1);

        cameraTransform = translate * rotate;

        if (camera.Mode == ProjectionMode.Orthographic)
        {
            projectionTransform = new Matrix4x4(
                2.0f / (camera.Right - camera.Left), 0, 0, -(camera.Right + camera.Left) / (camera.Right - camera.Left),
                0, 2.0f / (camera.Top - camera.Bottom), 0, -(camera.Top + camera.Bottom) / (camera.Top - camera.Bottom),
                0, 0, 2.0f / (camera.Near - camera.Far), -(camera.Near + camera.Far) / (camera.Near - camera.Far),
                0, 0, 0, 1);
        }
    }
}