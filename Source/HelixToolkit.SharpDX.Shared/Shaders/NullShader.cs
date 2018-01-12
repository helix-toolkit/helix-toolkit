/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
        public static readonly NullShader ComputeNull = new NullShader(ShaderStage.Compute);
        public static readonly NullShader DomainNull = new NullShader(ShaderStage.Domain);
        public static readonly NullShader GeometryNull = new NullShader(ShaderStage.Geometry);
        public static readonly NullShader HullNull = new NullShader(ShaderStage.Hull);
        public static readonly NullShader PixelNull = new NullShader(ShaderStage.Pixel);
        public static readonly NullShader VertexNull = new NullShader(ShaderStage.Vertex);

        public NullShader(ShaderStage type) : base(NULL, type, true)
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

        public override void BindTextures(DeviceContext context, IEnumerable<KeyValuePair<int, ShaderResourceView>> textures)
        {

        }
    }
}
