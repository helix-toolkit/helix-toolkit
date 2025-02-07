﻿using HelixToolkit.SharpDX.Render;
using HelixToolkit.SharpDX.Utilities;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// Vertex Shader
/// </summary>
public sealed class VertexShader : ShaderBase
{
    private global::SharpDX.Direct3D11.VertexShader? shader = null;
    internal global::SharpDX.Direct3D11.VertexShader? Shader => shader;
    public static readonly VertexShader NullVertexShader = new("NULL");
    public static readonly VertexShaderType Type;
    /// <summary>
    /// Vertex Shader
    /// </summary>
    /// <param name="device"></param>
    /// <param name="name"></param>
    /// <param name="byteCode"></param>
    public VertexShader(Device device, string name, byte[] byteCode)
        : base(name, ShaderStage.Vertex)
    {
        shader = new global::SharpDX.Direct3D11.VertexShader(device, byteCode)
        {
            DebugName = name
        };
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="VertexShader"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    private VertexShader(string name)
        : base(name, ShaderStage.Vertex, true)
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
        context.SetShader(this, bindConstantBuffer);
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
    public static implicit operator VertexShaderType(VertexShader s)
    {
        return Type;
    }
}
