/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX.Direct3D11;
using System.Collections.Generic;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public class GeometryShader : ShaderBase
    {
        private global::SharpDX.Direct3D11.GeometryShader shader;
        public GeometryShader(Device device, string name, byte[] byteCode) : base(name, ShaderStage.Geometry)
        {
            shader = Collect(new global::SharpDX.Direct3D11.GeometryShader(device, byteCode));
        }

        public override void Bind(DeviceContext context)
        {
            context.GeometryShader.Set(shader);
        }

        public override void BindConstantBuffers(DeviceContext context)
        {
            foreach (var buff in this.ConstantBufferMapping.Mappings)
            {
                context.GeometryShader.SetConstantBuffer(buff.Item1, buff.Item2.Buffer);
            }
        }

        public override void BindTexture(DeviceContext context, int index, ShaderResourceView texture)
        {
            throw new NotImplementedException();
        }

        public override void BindTexture(DeviceContext context, string name, ShaderResourceView texture)
        {
            throw new NotImplementedException();
        }

        public override void BindTextures(DeviceContext context, IEnumerable<Tuple<int, ShaderResourceView>> textures)
        {
            throw new NotImplementedException();
        }
    }
}
