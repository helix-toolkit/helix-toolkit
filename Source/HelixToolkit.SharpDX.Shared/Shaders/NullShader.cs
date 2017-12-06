using System;
using System.Collections.Generic;
using System.Text;
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public sealed class NullShader : ShaderBase
    {
        public const string NULL = "NULL";
        public NullShader(ShaderStage type) : base(NULL, type)
        {
        }

        public override void Bind(DeviceContext context)
        {
            switch (ShaderType)
            {
                case ShaderStage.Compute:
                    context.ComputeShader.Set(null);
                    break;
                case ShaderStage.Domain:
                    context.DomainShader.Set(null);
                    break;
                case ShaderStage.Geometry:
                    context.GeometryShader.Set(null);
                    break;
                case ShaderStage.Hull:
                    context.HullShader.Set(null);
                    break;
                case ShaderStage.Pixel:
                    context.PixelShader.Set(null);
                    break;
                case ShaderStage.Vertex:
                    context.VertexShader.Set(null);
                    break;
            }
        }

        public override void BindConstantBuffers(DeviceContext context)
        {

        }

        public override void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {

        }

        public override void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {

        }

        public override void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures)
        {

        }
    }
}
