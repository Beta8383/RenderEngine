namespace RenderEngine.Core.Shader
{
    public abstract class FragmentShader : ShaderBase
    {
        uint Color;
        public virtual void main()
        {
            Color = 0;
        }
    }
}
