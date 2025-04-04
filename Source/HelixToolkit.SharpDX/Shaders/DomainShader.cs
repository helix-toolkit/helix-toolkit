﻿using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public sealed class DomainShader : ShaderBase
{
    private global::SharpDX.Direct3D11.DomainShader? shader;
    internal global::SharpDX.Direct3D11.DomainShader? Shader => shader;
    public static readonly DomainShader NullDomainShader = new("NULL");
    public static readonly DomainShaderType Type;
    /// <summary>
    /// Vertex Shader
    /// </summary>
    /// <param name="device"></param>
    /// <param name="name"></param>
    /// <param name="byteCode"></param>
    public DomainShader(Device device, string name, byte[] byteCode)
        : base(name, ShaderStage.Domain)
    {
        shader = new global::SharpDX.Direct3D11.DomainShader(device, byteCode)
        {
            DebugName = name
        };
    }

    private DomainShader(string name)
        : base(name, ShaderStage.Domain, true)
    {

    }

    /// <summary>
    /// Binds shader to pipeline
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="bindConstantBuffer"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Bind(DeviceContextProxy context, bool bindConstantBuffer = true)
    {
        context.SetShader(this);
    }
    /// <summary>
    /// Binds the texture.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="name">The name.</param>
    /// <param name="texture">The texture.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BindTexture(DeviceContextProxy context, string name, ShaderResourceViewProxy? texture)
    {
        var slot = this.ShaderResourceViewMapping.TryGetBindSlot(name);
        context.SetShaderResource(Type, slot, texture);
    }
    /// <summary>
    /// Binds the texture.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="texture">The texture.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BindTexture(DeviceContextProxy context, int slot, ShaderResourceViewProxy? texture)
    {
        context.SetShaderResource(Type, slot, texture);
    }
    /// <summary>
    /// Binds the textures.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="textures">The textures.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BindTextures(DeviceContextProxy context, IList<KeyValuePair<int, ShaderResourceViewProxy>> textures)
    {
        foreach (var texture in textures)
        {
            context.SetShaderResource(Type, texture.Key, texture.Value);
        }
    }
    /// <summary>
    /// Binds the sampler.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="sampler">The sampler.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BindSampler(DeviceContextProxy context, int slot, SamplerStateProxy? sampler)
    {
        context.SetSampler(Type, slot, sampler);
    }
    /// <summary>
    /// Binds the sampler.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="name">The name.</param>
    /// <param name="sampler">The sampler.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BindSampler(DeviceContextProxy context, string name, SamplerStateProxy? sampler)
    {
        var slot = this.SamplerMapping.TryGetBindSlot(name);
        context.SetSampler(Type, slot, sampler);
    }

    /// <summary>
    /// Binds the samplers.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="samplers">The samplers.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void BindSamplers(DeviceContextProxy context, IList<KeyValuePair<int, SamplerStateProxy>> samplers)
    {
        foreach (var sampler in samplers)
        {
            context.SetSampler(Type, sampler.Key, sampler.Value);
        }
    }

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref shader);
        base.OnDispose(disposeManagedResources);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator DomainShaderType(DomainShader s)
    {
        return Type;
    }
}
