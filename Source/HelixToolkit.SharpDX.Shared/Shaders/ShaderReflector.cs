/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using System.Collections.Generic;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{ 
    public class ShaderReflector : IShaderReflector
    {
        public FeatureLevel FeatureLevel { get; private set; }

        public IDictionary<string, ConstantBufferMapping> ConstantBufferMappings { get; } = new Dictionary<string, ConstantBufferMapping>();

        public IDictionary<string, TextureMapping> TextureMappings { get; } = new Dictionary<string, TextureMapping>();

        public ShaderReflector()
        {

        }

        public void Parse(byte[] byteCode, ShaderStage stage)
        {
            ConstantBufferMappings.Clear();
            TextureMappings.Clear();
            using (var reflection = new ShaderReflection(byteCode))
            {
                FeatureLevel = reflection.MinFeatureLevel;
                for (int i = 0; i < reflection.Description.BoundResources; ++i)
                {
                    var res = reflection.GetResourceBindingDescription(i);
                    switch (res.Type)
                    {
                        case ShaderInputType.ConstantBuffer:
                            var cb = reflection.GetConstantBuffer(res.Name);
                            var cbDesc = new ConstantBufferDescription(res.Name, cb.Description.Size);
                            ConstantBufferMappings.Add(res.Name, cbDesc.CreateMapping(res.BindPoint));
                            break;
                        case ShaderInputType.Texture:
                            var tDesc = new TextureDescription(res.Name, stage);
                            TextureMappings.Add(res.Name, tDesc.CreateMapping(res.BindPoint));
                            break;
                    }
                }
            }
        }
    }
}
