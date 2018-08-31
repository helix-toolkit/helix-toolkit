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
    /// <summary>
    /// 
    /// </summary>
    public static class DefaultSamplers
    {
        /// <summary>
        /// The linear sampler wrap anisotropy =16
        /// </summary>
        public static SamplerStateDescription LinearSamplerWrapAni16 = new SamplerStateDescription()
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
        public static SamplerStateDescription LinearSamplerWrapAni8 = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap, AddressV = TextureAddressMode.Wrap, AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagLinearMipPoint, MaximumAnisotropy = 8,
        };
        /// <summary>
        /// The linear sampler wrap anisotropy = 4
        /// </summary>
        public static SamplerStateDescription LinearSamplerWrapAni4 = new SamplerStateDescription()
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
        public static SamplerStateDescription LinearSamplerWrapAni2 = new SamplerStateDescription()
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
        public static SamplerStateDescription LinearSamplerWrapAni1 = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Wrap,
            AddressV = TextureAddressMode.Wrap,
            AddressW = TextureAddressMode.Wrap,
            Filter = Filter.MinMagLinearMipPoint,
            MaximumLod = float.MaxValue
        };

        public static SamplerStateDescription LinearSamplerClampAni1 = new SamplerStateDescription()
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
        public static SamplerStateDescription PointSamplerWrap = new SamplerStateDescription()
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
        public static SamplerStateDescription ShadowSampler = new SamplerStateDescription()
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
        public static SamplerStateDescription EnvironmentSampler = new SamplerStateDescription()
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
        public static SamplerStateDescription IBLSampler = new SamplerStateDescription()
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
        public static SamplerStateDescription LinearSamplerClampAni4 = new SamplerStateDescription()
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
        public static SamplerStateDescription ScreenDupSampler = new SamplerStateDescription()
        {
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            Filter = Filter.MinMagMipLinear           
        };
    }
}
