using System;
using System.Drawing;
using System.Numerics;
using System.Security.Principal;
using RenderEngine.Core.Components;
using RenderEngine.Core.Components.Shape;
using RenderEngine.Core.Shader;
using static System.MathF;

namespace RenderEngine.Core;

public class Engine
{
    const int BytesPerPixel = 4;

    private byte[] pixels;
    private readonly uint width, height;
    private float[,] zBuffer;

    List<ShapeBase> shapes;

    Matrix4x4 viewportTransform = Matrix4x4.Identity,
              transform = Matrix4x4.Identity;

    public Engine(uint height, uint width)
    {
        this.width = width;
        this.height = height;
        Init();
    }

    public void Init()
    {
        viewportTransform = new(
            width / 2.0f, 0, 0, -width / 2.0f,
            0, height / 2.0f, 0, -height / 2.0f,
            0, 0, 1, 0,
            0, 0, 0, 1
        );

        zBuffer = new float[height, width];
        shapes = new List<ShapeBase>();
        pixels = new byte[width * height * BytesPerPixel];
    }

    #region Math Helper
    private static float Min3(float x, float y, float z) => Min(Min(x, y), z);
    private static float Max3(float x, float y, float z) => Max(Max(x, y), z);
    private static float LimitNumber(float min, float max, float x) => Min(Max(min, x), max);
    #endregion

    private void SetPixel(Vector4 color, int x, int y)
    {
        int index = (int)((y * width + x) * BytesPerPixel);
        pixels[index] = (byte)LimitNumber(0, 255, color.X);
        pixels[index + 1] = (byte)LimitNumber(0, 255, color.Y);
        pixels[index + 2] = (byte)LimitNumber(0, 255, color.Z);
        pixels[index + 3] = (byte)LimitNumber(0, 255, color.W);
    }

    private static bool GetBarycentricCoordinate(Vector4 p1, Vector4 p2, Vector4 p3, float x, float y, out float i, out float j, out float k)
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

    private static Vector4[] GetFragmentShaderParameters(Span<Vector4[]> para,int i,int j,int k,int length)
    {
        Vector4[] result = new Vector4[length];
        for (int l = 0; l < length; l++)
            result[l] = para[0][l] * i+ para[1][l] * j + para[2][l] * k;
        return result;
    }

    private void GetTriangleCoverageMask(ReadOnlySpan<Vector4> vertices)
    {
        int minx = (int)Min3(vertices[0].X, vertices[1].X, vertices[2].X);
        int miny = (int)Min3(vertices[0].Y, vertices[1].Y, vertices[2].Y);
        int maxx = (int)Max3(vertices[0].X, vertices[1].X, vertices[2].X) + 1;
        int maxy = (int)Max3(vertices[0].Y, vertices[1].Y, vertices[2].Y) + 1;

        minx = (int)LimitNumber(0, width - 1,minx);
        miny = (int)LimitNumber(0, height - 1, miny);

        float i, j, k, depth;

        for (int x = minx; x <= maxx; x++)
            for (int y = miny; y <= maxy; y++)
            {
                if (GetBarycentricCoordinate(vertices[0], vertices[1], vertices[2], (float)(x + 0.5), (float)(y + 0.5), out i, out j, out k))
                {
                    depth = vertices[0].Z * i + vertices[1].Z * j + vertices[2].Z * k;
                    if (depth <= 0 && depth > zBuffer[y,x])
                    {
                        zBuffer[y, x] = depth;
                        Vector4 fragmentShaderpara = 
                    }
                }
            }
    }

    private Vector4[] ApplyVertexShader(ShapeBase shape,out Vector4[][] para,IVertexShader shader)
    {
        int verticesCount = shape.Vertices.Length;
        var vertices = new Vector4[verticesCount];
        para = new Vector4[verticesCount][];
        for (int i = 0; i < verticesCount; i++)
            shader.main(shape.Vertices[i], shape.ModelTransform, transform, out para[i]);
        return vertices;
    }

    private void RenderShape(ShapeBase shape)
    {
        Vector4[][] para;
        var verticesSpan = new ReadOnlySpan<Vector4>(ApplyVertexShader(shape, out para));
        var paraSpan = new ReadOnlySpan<Vector4[]>(para);



        int[] triangleIndex = shape.GetTriangleIndex();
        for (int i = 0; i < triangleIndex.Length; i++)
            RenderTriangle(shape, triangleIndex[i]);
    }

    public ReadOnlySpan<byte> GetFrame()
    {
        Reset();
        foreach (ShapeBase shape in shapes)
            RenderShape(shape);

        return new ReadOnlySpan<byte>(pixels);
    }

    private void Reset()
    {
        pixels.SetValue(0, width * height * BytesPerPixel);
        zBuffer.SetValue(float.NegativeInfinity, height, width);
    }

    public void AddShape(ShapeBase shape) =>
        shapes.Add(shape);

    public void SetCamera(Camera camera)
    {
        #region camera transform
        //cameraTransform = Matrix4x4.CreateLookAt(camera.Position, camera.Direction, camera.Up);
        Vector3 xaxis, yaxis, zaxis;

        zaxis = -(camera.Direction - camera.Position);
        zaxis = Vector3.Normalize(zaxis);

        xaxis = Vector3.Multiply(camera.Up, zaxis);
        xaxis = Vector3.Normalize(xaxis);

        yaxis = Vector3.Multiply(zaxis, xaxis);
        yaxis = Vector3.Normalize(yaxis);

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

        Matrix4x4 cameraTransform = translate * rotate;
        #endregion

        #region Projection transform
        Matrix4x4 projectionTransform = Matrix4x4.Identity;
        if (camera.Mode == ProjectionMode.Orthographic)
        {
            //projectionTransform = Matrix4x4.CreateOrthographic(camera.width, camera.height, camera.zFarPlane, camera.zFarPlane);
            projectionTransform.M11 = 2f / width;
            projectionTransform.M22 = 2f / height;
            projectionTransform.M33 = 1f / (camera.zNearPlane - camera.zFarPlane);
            projectionTransform.M43 = camera.zNearPlane / (camera.zNearPlane - camera.zFarPlane);
        }
        else
        {

        }
        #endregion

        transform = viewportTransform * projectionTransform * cameraTransform;
    }
}