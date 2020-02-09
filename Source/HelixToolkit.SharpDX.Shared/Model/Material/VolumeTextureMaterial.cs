/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.IO;
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
    namespace Model
    {
        using Shaders;   
        using Utilities;

        public struct VolumeTextureParams
        {
            public byte[] VolumeTextures { get; }
            public int Width { get; }
            public int Height { get; }
            public int Depth { get; }
            public global::SharpDX.DXGI.Format Format { get; }
            public VolumeTextureParams(byte[] data, int width, int height, int depth, global::SharpDX.DXGI.Format format)
            {
                VolumeTextures = data;
                Width = width;
                Height = height;
                Depth = depth;
                Format = format;
            }
        }

        public struct VolumeTextureGradientParams
        {
            public Half4[] VolumeTextures { get; }
            public int Width { get; }
            public int Height { get; }
            public int Depth { get; }
            public global::SharpDX.DXGI.Format Format { get; }
            public VolumeTextureGradientParams(Half4[] data, int width, int height, int depth)
            {
                VolumeTextures = data;
                Width = width;
                Height = height;
                Depth = depth;
                Format = global::SharpDX.DXGI.Format.R16G16B16A16_Float;
            }
        }

        public interface IVolumeTextureMaterial
        {
            global::SharpDX.Direct3D11.SamplerStateDescription Sampler { set; get; }
            /// <summary>
            /// Gets or sets the step size, controls the quality.
            /// </summary>
            /// <value>
            /// The size of the step.
            /// </value>
            double SampleDistance { set; get; }
            /// <summary>
            /// Gets or sets the iteration. Usually set to VolumeDepth.
            /// </summary>
            /// <value>
            /// The iteration.
            /// </value>
            int MaxIterations { set; get; }
            /// <summary>
            /// Gets or sets the iteration offset. This can be used to achieve cross section
            /// </summary>
            /// <value>
            /// The iteration offset.
            /// </value>
            int IterationOffset { set; get; }
            /// <summary>
            /// Gets or sets the iso value. Only data with isovalue > sepecified iso value will be displayed.
            /// Value must be normalized to 0~1. Default = 1, show all data.
            /// </summary>
            /// <value>
            /// The iso value.
            /// </value>
            double IsoValue { set; get; }
            /// <summary>
            /// Gets or sets the color.
            /// </summary>
            /// <value>
            /// The color.
            /// </value>
            Color4 Color { set; get; }
            /// <summary>
            /// Gets or sets the transfer map.
            /// </summary>
            /// <value>
            /// The transfer map.
            /// </value>
            Color4[] TransferMap { set; get; }

            bool EnablePlaneAlignment { set; get; }
        }

        /// <summary>
        /// Abstract class for VolumeTextureMaterial
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class VolumeTextureMaterialCoreBase<T> : MaterialCore, IVolumeTextureMaterial
        {
            private T volumeTexture;
            public T VolumeTexture
            {
                set { Set(ref volumeTexture, value); }
                get { return volumeTexture; }
            }

            private global::SharpDX.Direct3D11.SamplerStateDescription sampler = DefaultSamplers.VolumeSampler;
            public global::SharpDX.Direct3D11.SamplerStateDescription Sampler
            {
                set { Set(ref sampler, value); }
                get { return sampler; }
            }

            private double sampleDistance = 1.0;
            /// <summary>
            /// Gets or sets the step size, controls the quality.
            /// </summary>
            /// <value>
            /// The size of the step.
            /// </value>
            public double SampleDistance
            {
                set { Set(ref sampleDistance, value); }
                get { return sampleDistance; }
            }

            private int maxIterations = 512;
            /// <summary>
            /// Gets or sets the iteration. Usually set to VolumeDepth.
            /// </summary>
            /// <value>
            /// The iteration.
            /// </value>
            public int MaxIterations
            {
                set { Set(ref maxIterations, value); }
                get { return maxIterations; }
            }

            private int iterationOffset = 0;
            /// <summary>
            /// Gets or sets the iteration offset. This can be used to achieve cross section
            /// </summary>
            /// <value>
            /// The iteration offset.
            /// </value>
            public int IterationOffset
            {
                set { Set(ref iterationOffset, value); }
                get { return iterationOffset; }
            }

            private double isoValue = 0;
            /// <summary>
            /// Gets or sets the iso value. Only data with isovalue > sepecified iso value will be displayed
            /// Value must be normalized to 0~1. Default = 1, show all data.
            /// </summary>
            /// <value>
            /// The iso value.
            /// </value>
            public double IsoValue
            {
                set { Set(ref isoValue, value); }
                get { return isoValue; }
            }

            private Color4 color = new Color4(1,1,1,1);
            /// <summary>
            /// Gets or sets the color.
            /// </summary>
            /// <value>
            /// The color.
            /// </value>
            public Color4 Color
            {
                set { Set(ref color, value); }
                get { return color; }
            }

            private Color4[] transferMap;
            public Color4[] TransferMap
            {
                set { Set(ref transferMap, value); }
                get { return transferMap; }
            }

            private bool enablePlaneAlignment = true;
            public bool EnablePlaneAlignment
            {
                set { Set(ref enablePlaneAlignment, value); }
                get { return enablePlaneAlignment; }
            }
            protected virtual string DefaultPassName { get; } = DefaultPassNames.Default;

            public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
            {
                return new VolumeMaterialVariable<T>(manager, technique, this, DefaultPassName)
                {
                    OnCreateTexture = (material, effectsManager) => { return OnCreateTexture(effectsManager); }
                };
            }

            protected abstract ShaderResourceViewProxy OnCreateTexture(IEffectsManager manager);
        }

        /// <summary>
        /// Default Volume Texture Material. Supports 3D DDS memory stream as <see cref="VolumeTextureMaterialCoreBase{T}.VolumeTexture"/>
        /// </summary>
        public sealed class VolumeTextureDDS3DMaterialCore : VolumeTextureMaterialCoreBase<Stream>
        {
            protected override ShaderResourceViewProxy OnCreateTexture(IEffectsManager manager)
            {
                return manager.MaterialTextureManager.Register(VolumeTexture, true);
            }
        }
        /// <summary>
        /// Used to use raw data as Volume 3D texture. 
        /// User must create their own data reader to read texture files as pixel byte[] and pass the necessary information as <see cref="VolumeTextureParams"/>
        /// <para>
        /// Pixel Byte[] is equal to Width * Height * Depth * BytesPerPixel.
        /// </para>
        /// </summary>
        public sealed class VolumeTextureRawDataMaterialCore : VolumeTextureMaterialCoreBase<VolumeTextureParams>
        {
            protected override ShaderResourceViewProxy OnCreateTexture(IEffectsManager manager)
            {
                if (VolumeTexture.VolumeTextures != null)
                {
                    return ShaderResourceViewProxy.CreateViewFromPixelData(manager.Device, VolumeTexture.VolumeTextures,
                    VolumeTexture.Width, VolumeTexture.Height, VolumeTexture.Depth, VolumeTexture.Format, true, false);
                }
                else
                {
                    return null;
                }
            }

            public static VolumeTextureParams LoadRAWFile(string filename, int width, int height, int depth)
            {
                using (FileStream file = new FileStream(filename, FileMode.Open))
                {
                    long length = file.Length;
                    var bytePerPixel = length / (width * height * depth);
                    byte[] buffer = new byte[width * height * depth * bytePerPixel];
                    using (BinaryReader reader = new BinaryReader(file))
                    {                   
                        reader.Read(buffer, 0, buffer.Length);
                    }
                    var format = global::SharpDX.DXGI.Format.Unknown;
                    switch (bytePerPixel)
                    {
                        case 1:
                            format = global::SharpDX.DXGI.Format.R8_UNorm;
                            break;
                        case 2:
                            format = global::SharpDX.DXGI.Format.R16_UNorm;
                            break;
                        case 4:
                            format = global::SharpDX.DXGI.Format.R32_Float;
                            break;
                    }
                    return new VolumeTextureParams(buffer, width, height, depth, format);               
                }              
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public sealed class VolumeTextureDiffuseMaterialCore : VolumeTextureMaterialCoreBase<VolumeTextureGradientParams>
        {
            protected override string DefaultPassName => DefaultPassNames.Diffuse;

            protected override ShaderResourceViewProxy OnCreateTexture(IEffectsManager manager)
            {
                if (VolumeTexture.VolumeTextures != null)
                {
                    return ShaderResourceViewProxy.CreateViewFromPixelData(manager.Device, VolumeTexture.VolumeTextures,
                    VolumeTexture.Width, VolumeTexture.Height, VolumeTexture.Depth, VolumeTexture.Format, true, false);
                }
                else
                {
                    return null;
                }
            }
        }
    }

}
