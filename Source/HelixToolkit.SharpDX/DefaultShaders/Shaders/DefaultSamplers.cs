using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Shaders;

/// <summary>
/// 
/// </summary>
public static class DefaultSamplers
{
    /// <summary>
    /// The linear sampler wrap anisotropy =16
    /// </summary>
    public static readonly SamplerStateDescription LinearSamplerWrapAni16 = new()
    {
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumAnisotropy = 16,
        MaximumLod = float.MaxValue
    };
    /// <summary>
    /// The linear sampler wrap anisotropy =8
    /// </summary>
    public static readonly SamplerStateDescription LinearSamplerWrapAni8 = new()
    {
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumAnisotropy = 8,
    };
    /// <summary>
    /// The linear sampler wrap anisotropy = 4
    /// </summary>
    public static readonly SamplerStateDescription LinearSamplerWrapAni4 = new()
    {
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumAnisotropy = 4,
        MaximumLod = float.MaxValue
    };
    /// <summary>
    /// The linear sampler wrap anisotropy =2
    /// </summary>
    public static readonly SamplerStateDescription LinearSamplerWrapAni2 = new()
    {
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumAnisotropy = 2,
        MaximumLod = float.MaxValue
    };
    /// <summary>
    /// The linear sampler wrap anisotropy = 1
    /// </summary>
    public static readonly SamplerStateDescription LinearSamplerWrapAni1 = new()
    {
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumLod = float.MaxValue
    };

    public static readonly SamplerStateDescription LinearSamplerClampAni1 = new()
    {
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumLod = float.MaxValue
    };
    /// <summary>
    /// The point sampler wrap
    /// </summary>
    public static readonly SamplerStateDescription PointSamplerWrap = new()
    {
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap,
        Filter = Filter.MinMagMipPoint,
        MaximumLod = float.MaxValue
    };
    /// <summary>
    /// The shadow sampler
    /// </summary>
    public static readonly SamplerStateDescription ShadowSampler = new()
    {
        AddressU = TextureAddressMode.Border,
        AddressV = TextureAddressMode.Border,
        AddressW = TextureAddressMode.Border,
        Filter = Filter.ComparisonMinMagLinearMipPoint,
        ComparisonFunction = Comparison.Less,
        BorderColor = new global::SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 0),
    };
    /// <summary>
    /// The cube sampler
    /// </summary>
    public static readonly SamplerStateDescription EnvironmentSampler = new()
    {
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumAnisotropy = 1,
        MaximumLod = 0
    };

    /// <summary>
    /// The cube sampler
    /// </summary>
    public static readonly SamplerStateDescription IBLSampler = new()
    {
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumAnisotropy = 1,
        MaximumLod = float.MaxValue
    };
    /// <summary>
    /// The linear sampler clamp ani4
    /// </summary>
    public static readonly SamplerStateDescription LinearSamplerClampAni4 = new()
    {
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumAnisotropy = 4,
    };
    /// <summary>
    /// The screen dup sampler
    /// </summary>
    public static readonly SamplerStateDescription ScreenDupSampler = new()
    {
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp,
        Filter = Filter.MinMagMipLinear
    };

    public static readonly SamplerStateDescription VolumeSampler = new()
    {
        AddressU = TextureAddressMode.Border,
        AddressV = TextureAddressMode.Border,
        AddressW = TextureAddressMode.Border,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumLod = float.MaxValue,
        BorderColor = new global::SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0)
    };

    public static readonly SamplerStateDescription SSAONoise = new()
    {
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Wrap,
        AddressW = TextureAddressMode.Wrap,
        Filter = Filter.MinMagLinearMipPoint,
        MaximumLod = 0
    };

    public static readonly SamplerStateDescription SSAOSamplerClamp = new()
    {
        AddressU = TextureAddressMode.Clamp,
        AddressV = TextureAddressMode.Clamp,
        AddressW = TextureAddressMode.Clamp,
        Filter = Filter.MinMagMipPoint,
        MaximumLod = 0
    };

    /// <summary>
    /// The point sampler wrap
    /// </summary>
    public static readonly SamplerStateDescription LineSamplerUWrapVClamp = new()
    {
        AddressU = TextureAddressMode.Wrap,
        AddressV = TextureAddressMode.Border,
        AddressW = TextureAddressMode.Wrap,
        Filter = Filter.MinMagMipPoint,
        BorderColor = new global::SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0),
        MaximumLod = 0
    };
}
