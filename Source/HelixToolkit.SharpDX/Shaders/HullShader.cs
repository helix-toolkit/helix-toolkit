using HelixToolkit.SharpDX.Render;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public sealed class HullShader : ShaderBase
{
    private global::SharpDX.Direct3D11.HullShader? shader;
    internal global::SharpDX.Direct3D11.HullShader? Shader => shader;
    public static readonly HullShader NullHullShader = new("NULL");
    public static readonly HullShaderType Type;
    /// <summary>
    /// Vertex Shader
    /// </summary>
    /// <param name="device"></param>
    /// <param name="name"></param>
    /// <param name="byteCode"></param>
    public HullShader(Device device, string name, byte[] byteCode)
        : base(name, ShaderStage.Hull)
    {
        shader = new global::SharpDX.Direct3D11.HullShader(device, byteCode);
        shader.DebugName = name;
    }

    private HullShader(string name)
        : base(name, ShaderStage.Hull, true)
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

    protected override void OnDispose(bool disposeManagedResources)
    {
        RemoveAndDispose(ref shader);
        base.OnDispose(disposeManagedResources);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator HullShaderType(HullShader s)
    {
        return Type;
    }
}
