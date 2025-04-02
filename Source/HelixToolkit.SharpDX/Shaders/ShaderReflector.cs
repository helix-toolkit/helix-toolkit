﻿using SharpDX.D3DCompiler;
using SharpDX.Direct3D;

namespace HelixToolkit.SharpDX.Shaders;

public sealed class ShaderReflector : IShaderReflector
{
    public FeatureLevel FeatureLevel
    {
        get; private set;
    }

    public Dictionary<string, ConstantBufferMapping> ConstantBufferMappings { get; } = new();

    public Dictionary<string, TextureMapping> TextureMappings { get; } = new();

    public Dictionary<string, UAVMapping> UAVMappings { get; } = new();

    public Dictionary<string, SamplerMapping> SamplerMappings { get; } = new();

    public ShaderReflector()
    {

    }

    public void Parse(byte[] byteCode, ShaderStage stage)
    {
        ConstantBufferMappings.Clear();
        TextureMappings.Clear();
        UAVMappings.Clear();
        SamplerMappings.Clear();
        using (var reflection = new ShaderReflection(byteCode))
        {
            FeatureLevel = reflection.MinFeatureLevel;
            for (var i = 0; i < reflection.Description.BoundResources; ++i)
            {
                var res = reflection.GetResourceBindingDescription(i);
                switch (res.Type)
                {
                    case ShaderInputType.ConstantBuffer:
                        var cb = reflection.GetConstantBuffer(res.Name);
                        var cbDesc = new ConstantBufferDescription(cb) { Stage = stage, Slot = res.BindPoint };
                        ConstantBufferMappings.Add(res.Name, cbDesc.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.Texture:
                        var tDescT = new TextureDescription(res.Name, stage, TextureType.Texture);
                        TextureMappings.Add(res.Name, tDescT.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.Structured:
                        var tDescStr = new TextureDescription(res.Name, stage, TextureType.Structured);
                        TextureMappings.Add(res.Name, tDescStr.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.TextureBuffer:
                        var tDescTB = new TextureDescription(res.Name, stage, TextureType.TextureBuffer);
                        TextureMappings.Add(res.Name, tDescTB.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.UnorderedAccessViewAppendStructured:
                        var uDescAppend = new UAVDescription(res.Name, stage, UnorderedAccessViewType.AppendStructured);
                        UAVMappings.Add(res.Name, uDescAppend.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.UnorderedAccessViewConsumeStructured:
                        var uDescConsume = new UAVDescription(res.Name, stage, UnorderedAccessViewType.ConsumeStructured);
                        UAVMappings.Add(res.Name, uDescConsume.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.UnorderedAccessViewRWByteAddress:
                        var uDescByte = new UAVDescription(res.Name, stage, UnorderedAccessViewType.RWByteAddress);
                        UAVMappings.Add(res.Name, uDescByte.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.UnorderedAccessViewRWStructuredWithCounter:
                        var uDescStr = new UAVDescription(res.Name, stage, UnorderedAccessViewType.RWStructuredWithCounter);
                        UAVMappings.Add(res.Name, uDescStr.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.UnorderedAccessViewRWTyped:
                        var uDescTyped = new UAVDescription(res.Name, stage, UnorderedAccessViewType.RWTyped);
                        UAVMappings.Add(res.Name, uDescTyped.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.UnorderedAccessViewRWStructured:
                        var uDescRWStr = new UAVDescription(res.Name, stage, UnorderedAccessViewType.RWStructured);
                        UAVMappings.Add(res.Name, uDescRWStr.CreateMapping(res.BindPoint));
                        break;
                    case ShaderInputType.Sampler:
                        SamplerMappings.Add(res.Name, new SamplerMapping(res.BindPoint, res.Name, stage));
                        break;
                }
            }
        }
    }
}
