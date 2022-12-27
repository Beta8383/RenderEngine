using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenderEngine.Core.Shader
{
    public abstract class ShaderBase
    {
        [AttributeUsage(AttributeTargets.Property)]
        protected internal class VaryingAttribute : Attribute
        {
            public readonly string Name;

            public VaryingAttribute(string name)
            {
                Name = name;
            }
        }
    }
}
