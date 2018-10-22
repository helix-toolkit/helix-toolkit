/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.IO;
using SharpDX.Direct3D11;
#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.ComponentModel;
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Shaders;

    /// <summary>
    /// Default Volume Texture Material. Supports 3D DDS memory stream as <see cref="VolumeTextureMaterialCoreBase{T}.VolumeTexture"/>
    /// </summary>
    public class VolumeTextureDDS3DMaterial : Material
    {
        /// <summary>
        /// Gets or sets the texture. Only supports 3D DDS texture stream
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public Stream Texture
        {
            get { return (Stream)GetValue(TextureProperty); }
            set { SetValue(TextureProperty, value); }
        }


        public static readonly DependencyProperty TextureProperty =
            DependencyProperty.Register("Texture", typeof(Stream), typeof(VolumeTextureDDS3DMaterial),
                new PropertyMetadata(null, (d,e)=> 
                {
                    ((d as VolumeTextureDDS3DMaterial).Core as VolumeTextureDDS3DMaterialCore).VolumeTexture = (Stream)e.NewValue;
                }));



        public SamplerStateDescription Sampler
        {
            get { return (SamplerStateDescription)GetValue(SamplerProperty); }
            set { SetValue(SamplerProperty, value); }
        }


        public static readonly DependencyProperty SamplerProperty =
            DependencyProperty.Register("Sampler", typeof(SamplerStateDescription), typeof(VolumeTextureDDS3DMaterial), 
                new PropertyMetadata(DefaultSamplers.LinearSamplerClampAni1, (d, e) =>
                {
                    ((d as VolumeTextureDDS3DMaterial).Core as VolumeTextureDDS3DMaterialCore).Sampler = (SamplerStateDescription)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the size of the steps. Usually set to 1 / (Texture Depth/Slices)
        /// </summary>
        /// <value>
        /// The size of the step.
        /// </value>
        public double StepSize
        {
            get { return (double)GetValue(StepSizeProperty); }
            set { SetValue(StepSizeProperty, value); }
        }
        
        public static readonly DependencyProperty StepSizeProperty =
            DependencyProperty.Register("StepSize", typeof(double), typeof(VolumeTextureDDS3DMaterial), 
                new PropertyMetadata(0.1, (d, e) =>
                {
                    ((d as VolumeTextureDDS3DMaterial).Core as VolumeTextureDDS3DMaterialCore).StepSize = (float)(double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the iterations. Usually equal to the Texture Depth.
        /// </summary>
        /// <value>
        /// The iterations.
        /// </value>
        public int Iterations
        {
            get { return (int)GetValue(IterationsProperty); }
            set { SetValue(IterationsProperty, value); }
        }

        public static readonly DependencyProperty IterationsProperty =
            DependencyProperty.Register("Iterations", typeof(int), typeof(VolumeTextureDDS3DMaterial), 
                new PropertyMetadata(10, (d, e) =>
                {
                    ((d as VolumeTextureDDS3DMaterial).Core as VolumeTextureDDS3DMaterialCore).Iterations = (int)e.NewValue;
                }));

        protected override MaterialCore OnCreateCore()
        {
            return new VolumeTextureDDS3DMaterialCore()
            {
                VolumeTexture = Texture,
                StepSize = (float)StepSize,
                Iterations = Iterations,
                Sampler = Sampler
            };
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return Clone();
        }
#endif
    }

    /// <summary>
    /// Used to use raw data as Volume 3D texture. 
    /// User must create their own data reader to read texture files as pixel byte[] and pass the necessary information as <see cref="VolumeTextureParams"/>
    /// <para>
    /// Pixel Byte[] is equal to Width * Height * Depth * BytesPerPixel.
    /// </para>
    /// </summary>
    public class VolumeTextureRawDataMaterial : Material
    {
        /// <summary>
        /// Gets or sets the texture. Only supports 3D DDS texture stream
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public VolumeTextureParams Texture
        {
            get { return (VolumeTextureParams)GetValue(TextureProperty); }
            set { SetValue(TextureProperty, value); }
        }


        public static readonly DependencyProperty TextureProperty =
            DependencyProperty.Register("Texture", typeof(VolumeTextureParams), typeof(VolumeTextureRawDataMaterial),
                new PropertyMetadata(new VolumeTextureParams(), (d, e) =>
                {
                    ((d as VolumeTextureRawDataMaterial).Core as VolumeTextureRawDataMaterialCore).VolumeTexture = (VolumeTextureParams)e.NewValue;
                }));



        public SamplerStateDescription Sampler
        {
            get { return (SamplerStateDescription)GetValue(SamplerProperty); }
            set { SetValue(SamplerProperty, value); }
        }


        public static readonly DependencyProperty SamplerProperty =
            DependencyProperty.Register("Sampler", typeof(SamplerStateDescription), typeof(VolumeTextureRawDataMaterial),
                new PropertyMetadata(DefaultSamplers.LinearSamplerClampAni1, (d, e) =>
                {
                    ((d as VolumeTextureRawDataMaterial).Core as VolumeTextureRawDataMaterialCore).Sampler = (SamplerStateDescription)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the size of the steps. Usually set to 1 / (Texture Depth/Slices)
        /// </summary>
        /// <value>
        /// The size of the step.
        /// </value>
        public double StepSize
        {
            get { return (double)GetValue(StepSizeProperty); }
            set { SetValue(StepSizeProperty, value); }
        }

        public static readonly DependencyProperty StepSizeProperty =
            DependencyProperty.Register("StepSize", typeof(double), typeof(VolumeTextureRawDataMaterial),
                new PropertyMetadata(0.1, (d, e) =>
                {
                    ((d as VolumeTextureRawDataMaterial).Core as VolumeTextureRawDataMaterialCore).StepSize = (float)(double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the iterations. Usually equal to the Texture Depth.
        /// </summary>
        /// <value>
        /// The iterations.
        /// </value>
        public int Iterations
        {
            get { return (int)GetValue(IterationsProperty); }
            set { SetValue(IterationsProperty, value); }
        }

        public static readonly DependencyProperty IterationsProperty =
            DependencyProperty.Register("Iterations", typeof(int), typeof(VolumeTextureRawDataMaterial),
                new PropertyMetadata(10, (d, e) =>
                {
                    ((d as VolumeTextureRawDataMaterial).Core as VolumeTextureRawDataMaterialCore).Iterations = (int)e.NewValue;
                }));

        protected override MaterialCore OnCreateCore()
        {
            return new VolumeTextureRawDataMaterialCore()
            {
                VolumeTexture = Texture,
                StepSize = (float)StepSize,
                Iterations = Iterations,
                Sampler = Sampler
            };
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return Clone();
        }
#endif
    }
}
