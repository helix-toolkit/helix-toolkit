/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.IO;
using SharpDX;
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
    public class VolumeTextureDDS3DMaterial : Material, IVolumeTextureMaterial
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
                new PropertyMetadata(DefaultSamplers.VolumeSampler, (d, e) =>
                {
                    ((d as VolumeTextureDDS3DMaterial).Core as VolumeTextureDDS3DMaterialCore).Sampler = (SamplerStateDescription)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the sample distance. Default = 1.0
        /// </summary>
        /// <value>
        /// The size of the step.
        /// </value>
        public double SampleDistance
        {
            get { return (double)GetValue(SampleDistanceProperty); }
            set { SetValue(SampleDistanceProperty, value); }
        }
        
        public static readonly DependencyProperty SampleDistanceProperty =
            DependencyProperty.Register("SampleDistance", typeof(double), typeof(VolumeTextureDDS3DMaterial), 
                new PropertyMetadata(1.0, (d, e) =>
                {
                    ((d as VolumeTextureDDS3DMaterial).Core as VolumeTextureDDS3DMaterialCore).SampleDistance = (double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the max iterations. Usually equal to the Texture Depth. Default = INT.MAX for automatic iteration
        /// </summary>
        /// <value>
        /// The iterations.
        /// </value>
        public int MaxIterations
        {
            get { return (int)GetValue(MaxIterationsProperty); }
            set { SetValue(MaxIterationsProperty, value); }
        }

        public static readonly DependencyProperty MaxIterationsProperty =
            DependencyProperty.Register("MaxIterations", typeof(int), typeof(VolumeTextureDDS3DMaterial), 
                new PropertyMetadata(int.MaxValue, (d, e) =>
                {
                    ((d as VolumeTextureDDS3DMaterial).Core as VolumeTextureDDS3DMaterialCore).MaxIterations = (int)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the color. It can also used to adjust opacity
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color4 Color
        {
            get { return (Color4)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }


        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color4", typeof(Color4), typeof(VolumeTextureDDS3DMaterial), 
                new PropertyMetadata(new Color4(1,1,1,1),
                (d, e) =>
                {
                    ((d as VolumeTextureDDS3DMaterial).Core as VolumeTextureDDS3DMaterialCore).Color = (Color4)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the Color Transfer Map.
        /// </summary>
        /// <value>
        /// The gradient map.
        /// </value>
        public Color4[] TransferMap
        {
            get { return (Color4[])GetValue(TransferMapProperty); }
            set { SetValue(TransferMapProperty, value); }
        }

        public static readonly DependencyProperty TransferMapProperty =
            DependencyProperty.Register("TransferMap", typeof(Color4[]), typeof(VolumeTextureDDS3DMaterial),
                new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as VolumeTextureDDS3DMaterial).Core as VolumeTextureDDS3DMaterialCore).TransferMap = (Color4[])e.NewValue;
                }));



        protected override MaterialCore OnCreateCore()
        {
            return new VolumeTextureDDS3DMaterialCore()
            {
                VolumeTexture = Texture,
                SampleDistance = SampleDistance,
                MaxIterations = MaxIterations,
                Sampler = Sampler,
                Color = Color,
                TransferMap = TransferMap
            };
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return new VolumeTextureDDS3DMaterial()
            {
                Texture = Texture,
                SampleDistance = SampleDistance,
                MaxIterations = MaxIterations,
                Sampler = Sampler,
                Color = Color,
                TransferMap = TransferMap
            };
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
    public class VolumeTextureRawDataMaterial : Material, IVolumeTextureMaterial
    {
        /// <summary>
        /// Gets or sets the texture.
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
                new PropertyMetadata(DefaultSamplers.VolumeSampler, (d, e) =>
                {
                    ((d as VolumeTextureRawDataMaterial).Core as VolumeTextureRawDataMaterialCore).Sampler = (SamplerStateDescription)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the sample distance. Default = 1.0
        /// </summary>
        /// <value>
        /// The size of the step.
        /// </value>
        public double SampleDistance
        {
            get { return (double)GetValue(SampleDistanceProperty); }
            set { SetValue(SampleDistanceProperty, value); }
        }

        public static readonly DependencyProperty SampleDistanceProperty =
            DependencyProperty.Register("SampleDistance", typeof(double), typeof(VolumeTextureRawDataMaterial),
                new PropertyMetadata(1.0, (d, e) =>
                {
                    ((d as VolumeTextureRawDataMaterial).Core as VolumeTextureRawDataMaterialCore).SampleDistance = (double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the max iterations. Usually equal to the Texture Depth. Default = INT.MAX for automatic iteration
        /// </summary>
        /// <value>
        /// The iterations.
        /// </value>
        public int MaxIterations
        {
            get { return (int)GetValue(MaxIterationsProperty); }
            set { SetValue(MaxIterationsProperty, value); }
        }

        public static readonly DependencyProperty MaxIterationsProperty =
            DependencyProperty.Register("MaxIterations", typeof(int), typeof(VolumeTextureRawDataMaterial),
                new PropertyMetadata(int.MaxValue, (d, e) =>
                {
                    ((d as VolumeTextureRawDataMaterial).Core as VolumeTextureRawDataMaterialCore).MaxIterations = (int)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the color. It can also used to adjust opacity
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color4 Color
        {
            get { return (Color4)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }


        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color4", typeof(Color4), typeof(VolumeTextureRawDataMaterial), 
                new PropertyMetadata(new Color4(1, 1, 1, 1),
                (d, e) =>
                {
                    ((d as VolumeTextureRawDataMaterial).Core as VolumeTextureRawDataMaterialCore).Color = (Color4)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the Color Transfer Map.
        /// </summary>
        /// <value>
        /// The gradient map.
        /// </value>
        public Color4[] TransferMap
        {
            get { return (Color4[])GetValue(TransferMapProperty); }
            set { SetValue(TransferMapProperty, value); }
        }

        public static readonly DependencyProperty TransferMapProperty =
            DependencyProperty.Register("TransferMap", typeof(Color4[]), typeof(VolumeTextureRawDataMaterial),
                new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as VolumeTextureRawDataMaterial).Core as VolumeTextureRawDataMaterialCore).TransferMap = (Color4[])e.NewValue;
                }));

        protected override MaterialCore OnCreateCore()
        {
            return new VolumeTextureRawDataMaterialCore()
            {
                VolumeTexture = Texture,
                SampleDistance = SampleDistance,
                MaxIterations = MaxIterations,
                Sampler = Sampler,
                Color = Color,
                TransferMap = TransferMap
            };
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return new VolumeTextureRawDataMaterial()
            {
                Texture = Texture,
                SampleDistance = SampleDistance,
                MaxIterations = MaxIterations,
                Sampler = Sampler,
                Color = Color,
                TransferMap = TransferMap
            };
        }
#endif
    }


    /// <summary>
    /// Used to use gradient data as Volume 3D texture. 
    /// User must create their own data reader to read texture files as pixel byte[] and pass the necessary information as <see cref="VolumeTextureParams"/>
    /// <para>
    /// Pixel Byte[] is equal to Width * Height * Depth * BytesPerPixel.
    /// </para>
    /// </summary>
    public class VolumeTextureDiffuseMaterial : Material, IVolumeTextureMaterial
    {
        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        /// <value>
        /// The texture.
        /// </value>
        public VolumeTextureGradientParams Texture
        {
            get { return (VolumeTextureGradientParams)GetValue(TextureProperty); }
            set { SetValue(TextureProperty, value); }
        }


        public static readonly DependencyProperty TextureProperty =
            DependencyProperty.Register("Texture", typeof(VolumeTextureGradientParams), typeof(VolumeTextureDiffuseMaterial),
                new PropertyMetadata(new VolumeTextureGradientParams(), (d, e) =>
                {
                    ((d as VolumeTextureDiffuseMaterial).Core as VolumeTextureDiffuseMaterialCore).VolumeTexture = (VolumeTextureGradientParams)e.NewValue;
                }));



        public SamplerStateDescription Sampler
        {
            get { return (SamplerStateDescription)GetValue(SamplerProperty); }
            set { SetValue(SamplerProperty, value); }
        }


        public static readonly DependencyProperty SamplerProperty =
            DependencyProperty.Register("Sampler", typeof(SamplerStateDescription), typeof(VolumeTextureDiffuseMaterial),
                new PropertyMetadata(DefaultSamplers.VolumeSampler, (d, e) =>
                {
                    ((d as VolumeTextureDiffuseMaterial).Core as VolumeTextureDiffuseMaterialCore).Sampler = (SamplerStateDescription)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the sample distance. Default = 1.0
        /// </summary>
        /// <value>
        /// The size of the step.
        /// </value>
        public double SampleDistance
        {
            get { return (double)GetValue(SampleDistanceProperty); }
            set { SetValue(SampleDistanceProperty, value); }
        }

        public static readonly DependencyProperty SampleDistanceProperty =
            DependencyProperty.Register("SampleDistance", typeof(double), typeof(VolumeTextureDiffuseMaterial),
                new PropertyMetadata(1.0, (d, e) =>
                {
                    ((d as VolumeTextureDiffuseMaterial).Core as VolumeTextureDiffuseMaterialCore).SampleDistance = (double)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the max iterations. Usually equal to the Texture Depth. Default = INT.MAX for automatic iteration
        /// </summary>
        /// <value>
        /// The iterations.
        /// </value>
        public int MaxIterations
        {
            get { return (int)GetValue(MaxIterationsProperty); }
            set { SetValue(MaxIterationsProperty, value); }
        }

        public static readonly DependencyProperty MaxIterationsProperty =
            DependencyProperty.Register("MaxIterations", typeof(int), typeof(VolumeTextureDiffuseMaterial),
                new PropertyMetadata(int.MaxValue, (d, e) =>
                {
                    ((d as VolumeTextureDiffuseMaterial).Core as VolumeTextureDiffuseMaterialCore).MaxIterations = (int)e.NewValue;
                }));


        /// <summary>
        /// Gets or sets the color. It can also used to adjust opacity
        /// </summary>
        /// <value>
        /// The color.
        /// </value>
        public Color4 Color
        {
            get { return (Color4)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }


        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color4", typeof(Color4), typeof(VolumeTextureDiffuseMaterial),
                new PropertyMetadata(new Color4(1, 1, 1, 1),
                (d, e) =>
                {
                    ((d as VolumeTextureDiffuseMaterial).Core as VolumeTextureDiffuseMaterialCore).Color = (Color4)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the Color Transfer Map.
        /// </summary>
        /// <value>
        /// The transfer map.
        /// </value>
        public Color4[] TransferMap
        {
            get { return (Color4[])GetValue(TransferMapProperty); }
            set { SetValue(TransferMapProperty, value); }
        }

        public static readonly DependencyProperty TransferMapProperty =
            DependencyProperty.Register("TransferMap", typeof(Color4[]), typeof(VolumeTextureDiffuseMaterial),
                new PropertyMetadata(null,
                (d, e) =>
                {
                    ((d as VolumeTextureDiffuseMaterial).Core as VolumeTextureDiffuseMaterialCore).TransferMap = (Color4[])e.NewValue;
                }));

        protected override MaterialCore OnCreateCore()
        {
            return new VolumeTextureDiffuseMaterialCore()
            {
                VolumeTexture = Texture,
                SampleDistance = SampleDistance,
                MaxIterations = MaxIterations,
                Sampler = Sampler,
                Color = Color,
                TransferMap = TransferMap
            };
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return new VolumeTextureDiffuseMaterial()
            {
                Texture = Texture,
                SampleDistance = SampleDistance,
                MaxIterations = MaxIterations,
                Sampler = Sampler,
                Color = Color,
                TransferMap = TransferMap
            };
        }
#endif
    }
}
