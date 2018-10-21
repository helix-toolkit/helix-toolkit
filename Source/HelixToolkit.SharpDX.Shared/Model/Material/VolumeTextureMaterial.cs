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

    public struct VolumeTextureParams
    {
        public byte[] VolumeTextures;
        public int Width;
        public int Height;
        public int Depth;
        public global::SharpDX.DXGI.Format Format;
    }

    public sealed class VolumeTextureMaterial : MaterialCore
    {
        private Stream volumeTexture;
        public Stream VolumeTexture
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

        public override MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique)
        {
            return new VolumeMaterialVariable(manager, technique, this);
        }
    }
}
