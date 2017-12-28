/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Shaders
#else
namespace HelixToolkit.UWP.Shaders
#endif
{
    public static class DefaultSamplers
    {
        public static SamplerStateDescription LinearSamplerWrapAni16 = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipLinear,
            MaximumAnisotropy = 16,
            MaximumLod = float.MaxValue
        };

        public static SamplerStateDescription LinearSamplerWrapAni8 = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap, AddressV = TextureAddressMode.Wrap, AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipLinear, MaximumAnisotropy = 8,
        };

        public static SamplerStateDescription LinearSamplerWrapAni4 = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipLinear,
            MaximumAnisotropy = 4,
            MaximumLod = float.MaxValue
        };

        public static SamplerStateDescription LinearSamplerWrapAni2 = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipLinear,
            MaximumAnisotropy = 2,
            MaximumLod = float.MaxValue
        };

        public static SamplerStateDescription LinearSamplerWrapAni1 = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipLinear,
            MaximumLod = float.MaxValue
        };

        public static SamplerStateDescription PointSamplerWrap = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipPoint,
            MaximumLod = float.MaxValue
        };

        public static SamplerStateDescription ShadowSampler = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Border,
            AddressV = TextureAddressMode.Border,
            AddressW = TextureAddressMode.Border,
            Filter = Filter.ComparisonMinMagLinearMipPoint,
            ComparisonFunction = Comparison.Less,
            BorderColor = new global::SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 0),
        };
    }
}
