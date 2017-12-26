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
        };

        public static SamplerStateDescription LinearSamplerWrapAni2 = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipLinear,
            MaximumAnisotropy = 2,
        };

        public static SamplerStateDescription LinearSamplerWrapAni1 = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipLinear,
        };

        public static SamplerStateDescription PointSamplerWrap = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagMipPoint,
        };

        public static SamplerStateDescription ShadowSampler = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Mirror,
            AddressV = TextureAddressMode.Mirror,
            AddressW = TextureAddressMode.Mirror,
            Filter = Filter.ComparisonMinMagMipLinear,
            ComparisonFunction = Comparison.LessEqual
        };
    }
}
