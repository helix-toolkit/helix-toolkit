/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Shaders
    {
        /// <summary>
        /// 
        /// </summary>
        public static class DefaultSamplers
        {
            /// <summary>
            /// The linear sampler wrap anisotropy =16
            /// </summary>
            public static readonly SamplerStateDescription LinearSamplerWrapAni16 = new SamplerStateDescription()
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
            public static readonly SamplerStateDescription LinearSamplerWrapAni8 = new SamplerStateDescription()
            {
                AddressU = TextureAddressMode.Wrap, AddressV = TextureAddressMode.Wrap, AddressW = TextureAddressMode.Wrap,
                Filter = Filter.MinMagLinearMipPoint, MaximumAnisotropy = 8,
            };
            /// <summary>
            /// The linear sampler wrap anisotropy = 4
            /// </summary>
            public static readonly SamplerStateDescription LinearSamplerWrapAni4 = new SamplerStateDescription()
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
            public static readonly SamplerStateDescription LinearSamplerWrapAni2 = new SamplerStateDescription()
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
            public static readonly SamplerStateDescription LinearSamplerWrapAni1 = new SamplerStateDescription()
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = Filter.MinMagLinearMipPoint,
                MaximumLod = float.MaxValue
            };

            public static readonly SamplerStateDescription LinearSamplerClampAni1 = new SamplerStateDescription()
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
            public static readonly SamplerStateDescription PointSamplerWrap = new SamplerStateDescription()
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
            public static readonly SamplerStateDescription ShadowSampler = new SamplerStateDescription()
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
            public static readonly SamplerStateDescription EnvironmentSampler = new SamplerStateDescription()
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
            public static readonly SamplerStateDescription IBLSampler = new SamplerStateDescription()
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
            public static readonly SamplerStateDescription LinearSamplerClampAni4 = new SamplerStateDescription()
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
            public static readonly SamplerStateDescription ScreenDupSampler = new SamplerStateDescription()
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = Filter.MinMagMipLinear           
            };

            public static readonly SamplerStateDescription VolumeSampler = new SamplerStateDescription()
            {
                AddressU = TextureAddressMode.Border,
                AddressV = TextureAddressMode.Border,
                AddressW = TextureAddressMode.Border,
                Filter = Filter.MinMagLinearMipPoint,
                MaximumLod = float.MaxValue,
                BorderColor = new global::SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0)
            };

            public static readonly SamplerStateDescription SSAONoise = new SamplerStateDescription()
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                Filter = Filter.MinMagLinearMipPoint,
                MaximumLod = 0
            };

            public static readonly SamplerStateDescription SSAOSamplerClamp = new SamplerStateDescription()
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = Filter.MinMagMipPoint,
                MaximumLod = 0
            };
        }
    }

}
