/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Model
#else
namespace HelixToolkit.UWP.Model
#endif
{
    using Shaders;
    using System.IO;
    using Utilities;

    public struct VolumeTextureParams
    {
        public byte[] VolumeTextures;
        public int Width;
        public int Height;
        public int Depth;
        public global::SharpDX.DXGI.Format Format;
    }
    /// <summary>
    /// Abstract class for VolumeTextureMaterial
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class VolumeTextureMaterialCoreBase<T> : MaterialCore
    {
        private T volumeTexture;
        public T VolumeTexture
        {
            set { Set(ref volumeTexture, value); }
            get { return volumeTexture; }
        }

        private global::SharpDX.Direct3D11.SamplerStateDescription sampler = DefaultSamplers.LinearSamplerClampAni1;
        public global::SharpDX.Direct3D11.SamplerStateDescription Sampler
        {
            set { Set(ref sampler, value); }
            get { return sampler; }
        }

        private float stepSize = 0.1f;
        /// <summary>
        /// Gets or sets the step size, usually set to 1 / VolumeDepth.
        /// </summary>
        /// <value>
        /// The size of the step.
        /// </value>
        public float StepSize
        {
            set { Set(ref stepSize, value); }
            get { return stepSize; }
        }

        private int iterations = 10;
        /// <summary>
        /// Gets or sets the iteration. Usually set to VolumeDepth.
        /// </summary>
        /// <value>
        /// The iteration.
        /// </value>
        public int Iterations
        {
            set { Set(ref iterations, value); }
            get { return iterations; }
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

        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new VolumeMaterialVariable<T>(manager, technique, this)
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
            return manager.MaterialTextureManager.Register(VolumeTexture);
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
            return ShaderResourceViewProxy.CreateViewFromPixelData(manager.Device, VolumeTexture.VolumeTextures,
                VolumeTexture.Width, VolumeTexture.Height, VolumeTexture.Depth, VolumeTexture.Format);
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
                var ret = new VolumeTextureParams()
                {
                    VolumeTextures = buffer,
                    Width = width,
                    Height = height,
                    Depth = depth,
                };
                switch (bytePerPixel)
                {
                    case 1:
                        ret.Format = global::SharpDX.DXGI.Format.R8_UNorm;
                        break;
                    case 2:
                        ret.Format = global::SharpDX.DXGI.Format.R16_UNorm;
                        break;
                    case 4:
                        ret.Format = global::SharpDX.DXGI.Format.R32_Float;
                        break;
                }
                return ret;
            }              
        }
    }
}
