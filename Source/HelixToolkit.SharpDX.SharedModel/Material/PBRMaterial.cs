using SharpDX;
using SharpDX.Direct3D11;
using System.IO;
using System.Runtime.Serialization;
using System.ComponentModel;

#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Shaders;    
    using Utilities;

    [DataContract]
    public partial class PBRMaterial : Material
    {        
        /// <summary>
        /// Identifies the System.Windows.Media.Media3D.DiffuseMaterial.Color�dependency
        /// property.
        /// </summary>
        public static readonly DependencyProperty AlbedoColorProperty =
            DependencyProperty.Register("AlbedoColor", typeof(Color4), typeof(PBRMaterial), new PropertyMetadata((Color4)Color.White,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).AlbedoColor = (Color4)e.NewValue;
                }));

        /// <summary>
        ///         
        /// </summary>
        public static readonly DependencyProperty MetallicFactorProperty =
            DependencyProperty.Register("MetallicFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(0.0,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).MetallicFactor = (float)(double)e.NewValue;
                }));

        /// <summary>
        ///         
        /// </summary>
        public static readonly DependencyProperty RoughnessFactorProperty =
            DependencyProperty.Register("RoughnessFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(0.0,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).RoughnessFactor = (float)(double)e.NewValue;
                }));
        /// <summary>
        ///         
        /// </summary>
        public static readonly DependencyProperty AmbientOcclusionFactorProperty =
            DependencyProperty.Register("AmbientOcclusionFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(1.0,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).AmbientOcclusionFactor = (float)(double)e.NewValue;
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty AlbedoMapProperty =
            DependencyProperty.Register("AlbedoMap", typeof(Stream), typeof(PBRMaterial), new PropertyMetadata(null,
                (d, e) => { ((d as Material).Core as PBRMaterialCore).AlbedoMap = e.NewValue as Stream; }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty EmissiveMapProperty =
            DependencyProperty.Register("EmissiveMap", typeof(Stream), typeof(PBRMaterial), new PropertyMetadata(null,
                (d, e) => { ((d as Material).Core as PBRMaterialCore).EmissiveMap = e.NewValue as Stream; }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RMAMapProperty =
            DependencyProperty.Register("RMAMap", typeof(Stream), typeof(PBRMaterial), new PropertyMetadata(null,
                (d, e) => { ((d as Material).Core as PBRMaterialCore).RMAMap = e.NewValue as Stream; }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty NormalMapProperty =
            DependencyProperty.Register("NormalMap", typeof(Stream), typeof(PBRMaterial), new PropertyMetadata(null,
                (d, e) => { ((d as Material).Core as PBRMaterialCore).NormalMap = e.NewValue as Stream; }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisplacementMapProperty =
            DependencyProperty.Register("DisplacementMap", typeof(Stream), typeof(PBRMaterial), new PropertyMetadata(null,
                (d, e) => { ((d as Material).Core as PBRMaterialCore).DisplacementMap = e.NewValue as Stream; }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty IrradianceMapProperty =
            DependencyProperty.Register("IrrandianceMap", typeof(Stream), typeof(PBRMaterial), new PropertyMetadata(null,
                (d, e) => { ((d as Material).Core as PBRMaterialCore).IrradianceMap = e.NewValue as Stream; }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisplacementMapScaleMaskProperty =
            DependencyProperty.Register("DisplacementMapScaleMask", typeof(Vector4), typeof(PBRMaterial), new PropertyMetadata(new Vector4(0, 0, 0, 1),
                (d, e) => { ((d as Material).Core as PBRMaterialCore).DisplacementMapScaleMask = (Vector4)e.NewValue; }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty SurfaceMapSamplerProperty =
            DependencyProperty.Register("SurfaceMapSampler", typeof(SamplerStateDescription), typeof(PBRMaterial), new PropertyMetadata(DefaultSamplers.LinearSamplerWrapAni4,
                (d, e) => { ((d as Material).Core as PBRMaterialCore).SurfaceMapSampler = (SamplerStateDescription)e.NewValue; }));

        /// <summary>
        ///
        /// </summary>
        public static readonly DependencyProperty IBLSamplerProperty =
            DependencyProperty.Register("IBLSampler", typeof(SamplerStateDescription), typeof(PBRMaterial), new PropertyMetadata(DefaultSamplers.LinearSamplerWrapAni4,
                (d, e) => { ((d as Material).Core as PBRMaterialCore).IBLSampler = (SamplerStateDescription)e.NewValue; }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty DisplacementMapSamplerProperty =
            DependencyProperty.Register("DisplacementMapSampler", typeof(SamplerStateDescription), typeof(PBRMaterial), new PropertyMetadata(DefaultSamplers.LinearSamplerWrapAni1,
                (d, e) => { ((d as Material).Core as PBRMaterialCore).DisplacementMapSampler = (SamplerStateDescription)e.NewValue; }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderAlbedoMapProperty =
            DependencyProperty.Register("RenderAlbedoMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).RenderAlbedoMap = (bool)e.NewValue;
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderEmissiveMapProperty =
            DependencyProperty.Register("RenderEmissiveMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).RenderEmissiveMap = (bool)e.NewValue;
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderRMAMapProperty =
            DependencyProperty.Register("RenderRMAMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).RenderRMAMap = (bool)e.NewValue;
                }));
        /// <summary>
        /// 
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderIrradianceMapProperty =
            DependencyProperty.Register("RenderIrradianceMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).RenderIrradianceMap = (bool)e.NewValue;
                }));

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderNormalMapProperty =
            DependencyProperty.Register("RenderNormalMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).RenderNormalMap = (bool)e.NewValue;
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RenderDisplacementMapProperty =
            DependencyProperty.Register("RenderDisplacementMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).RenderDisplacementMap = (bool)e.NewValue;
                }));
        /// <summary>
        /// The render environment map property
        /// </summary>
        public static readonly DependencyProperty RenderEnvironmentMapProperty =
            DependencyProperty.Register("RenderEnvironmentMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(false,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).RenderEnvironmentMap = (bool)e.NewValue;
                }));
        /// <summary>
        /// The render shadow map property
        /// </summary>
        public static readonly DependencyProperty RenderShadowMapProperty =
            DependencyProperty.Register("RenderShadowMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(false,
                (d, e) =>
                {
                    ((d as Material).Core as PBRMaterialCore).RenderShadowMap = (bool)e.NewValue;
                }));

        /// <summary>
        /// The enable tessellation property
        /// </summary>
        public static readonly DependencyProperty EnableTessellationProperty = DependencyProperty.Register("EnableTessellation", typeof(bool), typeof(PBRMaterial),
            new PropertyMetadata(false, (d, e) => { ((d as Material).Core as PBRMaterialCore).EnableTessellation = (bool)e.NewValue; }));
        /// <summary>
        /// The tessellation factor at <see cref="MaxTessellationDistance"/> property
        /// </summary>
        public static readonly DependencyProperty MaxDistanceTessellationFactorProperty =
            DependencyProperty.Register("MaxDistanceTessellationFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(1.0, (d, e) =>
            { ((d as Material).Core as PBRMaterialCore).MaxDistanceTessellationFactor = (float)(double)e.NewValue; }));
        /// <summary>
        /// The tessellation factor at <see cref="MinTessellationDistance"/> property
        /// </summary>
        public static readonly DependencyProperty MinDistanceTessellationFactorProperty =
            DependencyProperty.Register("MinDistanceTessellationFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(2.0, (d, e) =>
            { ((d as Material).Core as PBRMaterialCore).MinDistanceTessellationFactor = (float)(double)e.NewValue; }));
        /// <summary>
        /// The maximum tessellation distance property
        /// </summary>
        public static readonly DependencyProperty MaxTessellationDistanceProperty =
            DependencyProperty.Register("MaxTessellationDistance", typeof(double), typeof(PBRMaterial), new PropertyMetadata(50.0, (d, e) =>
            { ((d as Material).Core as PBRMaterialCore).MaxTessellationDistance = (float)(double)e.NewValue; }));
        /// <summary>
        /// The minimum tessellation distance property
        /// </summary>
        public static readonly DependencyProperty MinTessellationDistanceProperty =
            DependencyProperty.Register("MinTessellationDistance", typeof(double), typeof(PBRMaterial), new PropertyMetadata(1.0, (d, e) =>
            { ((d as Material).Core as PBRMaterialCore).MinTessellationDistance = (float)(double)e.NewValue; }));


        /// <summary>
        /// The uv transform property
        /// </summary>
        public static readonly DependencyProperty UVTransformProperty =
            DependencyProperty.Register("UVTransform", typeof(Matrix), typeof(PBRMaterial), new PropertyMetadata(Matrix.Identity, (d, e) =>
            {
                ((d as Material).Core as PBRMaterialCore).UVTransform = (Matrix)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the diffuse color for the material.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb147175(v=vs.85).aspx
        /// </summary>
#if !NETFX_CORE
        [TypeConverter(typeof(Color4Converter))]
#endif
        public Color4 AlbedoColor
        {
            get { return (Color4)this.GetValue(AlbedoColorProperty); }
            set { this.SetValue(AlbedoColorProperty, value); }
        }

        public double MetallicFactor
        {
            get { return (double)this.GetValue(MetallicFactorProperty); }
            set { this.SetValue(MetallicFactorProperty, value); }
        }

        public double RoughnessFactor
        {
            get { return (double)this.GetValue(RoughnessFactorProperty); }
            set { this.SetValue(RoughnessFactorProperty, value); }
        }

        public double AmbientOcclusionFactor
        {
            get { return (double)this.GetValue(AmbientOcclusionFactorProperty); }
            set { this.SetValue(AmbientOcclusionFactorProperty, value); }
        }

        public Stream AlbedoMap
        {
            get { return (Stream)this.GetValue(AlbedoMapProperty); }
            set { this.SetValue(AlbedoMapProperty, value); }
        }


        public Stream EmissiveMap
        {
            get { return (Stream)this.GetValue(EmissiveMapProperty); }
            set { this.SetValue(EmissiveMapProperty, value); }
        }

        public Stream RMAMap
        {
            get { return (Stream)this.GetValue(RMAMapProperty); }
            set { this.SetValue(RMAMapProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public Stream NormalMap
        {
            get { return (Stream)this.GetValue(NormalMapProperty); }
            set { this.SetValue(NormalMapProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public Stream DisplacementMap
        {
            get { return (Stream)this.GetValue(DisplacementMapProperty); }
            set { this.SetValue(DisplacementMapProperty, value); }
        }


        public Stream IrradianceMap
        {
            get { return (Stream)this.GetValue(IrradianceMapProperty); }
            set { this.SetValue(IrradianceMapProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public SamplerStateDescription SurfaceMapSampler
        {
            get { return (SamplerStateDescription)this.GetValue(SurfaceMapSamplerProperty); }
            set { this.SetValue(SurfaceMapSamplerProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public SamplerStateDescription IBLSampler
        {
            get { return (SamplerStateDescription)this.GetValue(IBLSamplerProperty); }
            set { this.SetValue(IBLSamplerProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public SamplerStateDescription DisplacementMapSampler
        {
            get { return (SamplerStateDescription)this.GetValue(DisplacementMapSamplerProperty); }
            set { this.SetValue(DisplacementMapSamplerProperty, value); }
        }

#if !NETFX_CORE
        [TypeConverter(typeof(Vector4Converter))]
#endif
        public Vector4 DisplacementMapScaleMask
        {
            set { SetValue(DisplacementMapScaleMaskProperty, value); }
            get { return (Vector4)GetValue(DisplacementMapScaleMaskProperty); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RenderAlbedoMap
        {
            get { return (bool)this.GetValue(RenderAlbedoMapProperty); }
            set { this.SetValue(RenderAlbedoMapProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RenderNormalMap
        {
            get { return (bool)this.GetValue(RenderNormalMapProperty); }
            set { this.SetValue(RenderNormalMapProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool RenderEmissiveMap
        {
            get { return (bool)this.GetValue(RenderEmissiveMapProperty); }
            set { this.SetValue(RenderEmissiveMapProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool RenderRMAMap
        {
            get { return (bool)this.GetValue(RenderRMAMapProperty); }
            set { this.SetValue(RenderRMAMapProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool RenderIrradianceMap
        {
            get { return (bool)this.GetValue(RenderIrradianceMapProperty); }
            set { this.SetValue(RenderIrradianceMapProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool RenderDisplacementMap
        {
            get { return (bool)this.GetValue(RenderDisplacementMapProperty); }
            set { this.SetValue(RenderDisplacementMapProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [render environment map]. Default is false
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render environment map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderEnvironmentMap
        {
            get { return (bool)GetValue(RenderEnvironmentMapProperty); }
            set { SetValue(RenderEnvironmentMapProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [render shadow map]. Default is false
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render shadow map]; otherwise, <c>false</c>.
        /// </value>
        public bool RenderShadowMap
        {
            get { return (bool)GetValue(RenderShadowMapProperty); }
            set { SetValue(RenderShadowMapProperty, value); }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable tessellation].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable tessellation]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTessellation
        {
            set
            {
                SetValue(EnableTessellationProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableTessellationProperty);
            }
        }
        /// <summary>
        /// Gets or sets the tessellation factor at <see cref="MaxTessellationDistance"/>.
        /// </summary>
        /// <value>
        /// The maximum tessellation factor.
        /// </value>
        public double MaxDistanceTessellationFactor
        {
            get { return (double)GetValue(MaxDistanceTessellationFactorProperty); }
            set { SetValue(MaxDistanceTessellationFactorProperty, value); }
        }
        /// <summary>
        /// Gets or sets the tessellation factor at <see cref="MinTessellationDistance"/>
        /// </summary>
        /// <value>
        /// The minimum tessellation factor.
        /// </value>
        public double MinDistanceTessellationFactor
        {
            get { return (double)GetValue(MinDistanceTessellationFactorProperty); }
            set { SetValue(MinDistanceTessellationFactorProperty, value); }
        }
        /// <summary>
        /// Gets or sets the maximum tessellation distance.
        /// </summary>
        /// <value>
        /// The maximum tessellation distance.
        /// </value>
        public double MaxTessellationDistance
        {
            get { return (double)GetValue(MaxTessellationDistanceProperty); }
            set { SetValue(MaxTessellationDistanceProperty, value); }
        }
        /// <summary>
        /// Gets or sets the minimum tessellation distance.
        /// </summary>
        /// <value>
        /// The minimum tessellation distance.
        /// </value>
        public double MinTessellationDistance
        {
            get { return (double)GetValue(MinTessellationDistanceProperty); }
            set { SetValue(MinTessellationDistanceProperty, value); }
        }
        /// <summary>
        /// Gets or sets the texture uv transform.
        /// </summary>
        /// <value>
        /// The uv transform.
        /// </value>
        public Matrix UVTransform
        {
            get { return (Matrix)GetValue(UVTransformProperty); }
            set { SetValue(UVTransformProperty, value); }
        }

        protected override MaterialCore OnCreateCore()
        {
            return new PBRMaterialCore()
            {
                AlbedoColor = AlbedoColor,
                MetallicFactor = (float)MetallicFactor,
                RoughnessFactor = (float)RoughnessFactor,
                AmbientOcclusionFactor = (float)AmbientOcclusionFactor,
                AlbedoMap = AlbedoMap,
                NormalMap = NormalMap,
                EmissiveMap = EmissiveMap,
                RMAMap = RMAMap,
                IrradianceMap = IrradianceMap,
                DisplacementMap = DisplacementMap,
                SurfaceMapSampler = SurfaceMapSampler,
                IBLSampler = IBLSampler,
                DisplacementMapSampler = DisplacementMapSampler,
            
                RenderAlbedoMap = RenderAlbedoMap,
                RenderDisplacementMap=RenderDisplacementMap,
                RenderEmissiveMap= RenderEmissiveMap,
                RenderEnvironmentMap = RenderEnvironmentMap,
                RenderIrradianceMap = RenderIrradianceMap,
                RenderNormalMap = RenderNormalMap,
                RenderRMAMap = RenderRMAMap,
                RenderShadowMap = RenderShadowMap,

                DisplacementMapScaleMask = DisplacementMapScaleMask,
                UVTransform = UVTransform,

                EnableTessellation = EnableTessellation,
                MaxDistanceTessellationFactor = (float)MaxDistanceTessellationFactor,
                MinDistanceTessellationFactor = (float)MinDistanceTessellationFactor,
                MaxTessellationDistance = (float)MaxTessellationDistance,
                MinTessellationDistance = (float)MinTessellationDistance,
            };
        }

#if !NETFX_CORE
        protected override Freezable CreateInstanceCore()
        {
            return Clone();
        }
#endif

        public PBRMaterial CloneMaterial()
        {
            return new PBRMaterial()
            {
                AlbedoColor = AlbedoColor,
                MetallicFactor = MetallicFactor,
                RoughnessFactor = RoughnessFactor,
                AmbientOcclusionFactor = AmbientOcclusionFactor,
                AlbedoMap = AlbedoMap,
                NormalMap = NormalMap,
                EmissiveMap = EmissiveMap,
                RMAMap = RMAMap,
                IrradianceMap = IrradianceMap,
                DisplacementMap = DisplacementMap,
                SurfaceMapSampler = SurfaceMapSampler,
                IBLSampler = IBLSampler,
                DisplacementMapSampler = DisplacementMapSampler,

                RenderAlbedoMap = RenderAlbedoMap,
                RenderDisplacementMap = RenderDisplacementMap,
                RenderEmissiveMap = RenderEmissiveMap,
                RenderEnvironmentMap = RenderEnvironmentMap,
                RenderIrradianceMap = RenderIrradianceMap,
                RenderNormalMap = RenderNormalMap,
                RenderRMAMap = RenderRMAMap,
                RenderShadowMap = RenderShadowMap,

                DisplacementMapScaleMask = DisplacementMapScaleMask,
                UVTransform = UVTransform,

                EnableTessellation = EnableTessellation,
                MaxDistanceTessellationFactor = MaxDistanceTessellationFactor,
                MinDistanceTessellationFactor = MinDistanceTessellationFactor,
                MaxTessellationDistance = MaxTessellationDistance,
                MinTessellationDistance = MinTessellationDistance,
            };
        }
    }
}
