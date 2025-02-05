using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;
using System.ComponentModel;
using System.Runtime.Serialization;

#if false
#elif WINUI
#elif WPF
using HelixToolkit.Wpf.SharpDX.Utilities;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#else
#error Unknown framework
#endif

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
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.AlbedoColor = (Color4)e.NewValue;
                }
            }));

    /// <summary>
    /// The albedo color property
    /// </summary>
    public static readonly DependencyProperty EmissiveColorProperty =
        DependencyProperty.Register("EmissiveColor", typeof(Color4), typeof(PBRMaterial), new PropertyMetadata((Color4)Color.Black,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.EmissiveColor = (Color4)e.NewValue;
                }
            }));

    /// <summary>
    ///         
    /// </summary>
    public static readonly DependencyProperty MetallicFactorProperty =
        DependencyProperty.Register("MetallicFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.MetallicFactor = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    ///         
    /// </summary>
    public static readonly DependencyProperty RoughnessFactorProperty =
        DependencyProperty.Register("RoughnessFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RoughnessFactor = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    ///         
    /// </summary>
    public static readonly DependencyProperty AmbientOcclusionFactorProperty =
        DependencyProperty.Register("AmbientOcclusionFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(1.0,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.AmbientOcclusionFactor = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    ///         
    /// </summary>
    public static readonly DependencyProperty ReflectanceFactorProperty =
        DependencyProperty.Register("ReflectanceFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.ReflectanceFactor = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    ///         
    /// </summary>
    public static readonly DependencyProperty ClearCoatStrengthProperty =
        DependencyProperty.Register("ClearCoatStrength", typeof(double), typeof(PBRMaterial), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.ClearCoatStrength = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    ///         
    /// </summary>
    public static readonly DependencyProperty ClearCoatRoughnessProperty =
        DependencyProperty.Register("ClearCoatRoughness", typeof(double), typeof(PBRMaterial), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.ClearCoatRoughness = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty AlbedoMapProperty =
        DependencyProperty.Register("AlbedoMap", typeof(TextureModel), typeof(PBRMaterial), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.AlbedoMap = e.NewValue as TextureModel;
                }
            }));
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty EmissiveMapProperty =
        DependencyProperty.Register("EmissiveMap", typeof(TextureModel), typeof(PBRMaterial), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.EmissiveMap = e.NewValue as TextureModel;
                }
            }));

    /// <summary>
    /// glTF2 defines metalness as B channel, roughness as G channel, and occlusion as R channel
    /// If uses RMA map, set both <see cref="RoughnessMetallicMap"/> and <see cref="AmbientOcculsionMap"/> to the same texture.
    /// </summary>
    public static readonly DependencyProperty RoughnessMetallicMapProperty =
        DependencyProperty.Register("RoughnessMetallicMap", typeof(TextureModel), typeof(PBRMaterial), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RoughnessMetallicMap = e.NewValue as TextureModel;
                }
            }));

    /// <summary>
    /// glTF2 defines metalness as B channel, roughness as G channel, and occlusion as R channel.
    /// If uses RMA map, set both <see cref="RoughnessMetallicMap"/> and <see cref="AmbientOcculsionMap"/> to the same texture.
    /// </summary>
    public static readonly DependencyProperty AmbientOcculsionMapProperty =
        DependencyProperty.Register("AmbientOcculsionMap", typeof(TextureModel), typeof(PBRMaterial), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.AmbientOcculsionMap = e.NewValue as TextureModel;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty NormalMapProperty =
        DependencyProperty.Register("NormalMap", typeof(TextureModel), typeof(PBRMaterial), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.NormalMap = e.NewValue as TextureModel;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty DisplacementMapProperty =
        DependencyProperty.Register("DisplacementMap", typeof(TextureModel), typeof(PBRMaterial), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.DisplacementMap = e.NewValue as TextureModel;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty IrradianceMapProperty =
        DependencyProperty.Register("IrrandianceMap", typeof(TextureModel), typeof(PBRMaterial), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.IrradianceMap = e.NewValue as TextureModel;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty DisplacementMapScaleMaskProperty =
        DependencyProperty.Register("DisplacementMapScaleMask", typeof(Vector4), typeof(PBRMaterial), new PropertyMetadata(new Vector4(0, 0, 0, 1),
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.DisplacementMapScaleMask = (Vector4)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty SurfaceMapSamplerProperty =
        DependencyProperty.Register("SurfaceMapSampler", typeof(SamplerStateDescription), typeof(PBRMaterial), new PropertyMetadata(DefaultSamplers.LinearSamplerWrapAni4,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.SurfaceMapSampler = (SamplerStateDescription)e.NewValue;
                }
            }));

    /// <summary>
    ///
    /// </summary>
    public static readonly DependencyProperty IBLSamplerProperty =
        DependencyProperty.Register("IBLSampler", typeof(SamplerStateDescription), typeof(PBRMaterial), new PropertyMetadata(DefaultSamplers.LinearSamplerWrapAni4,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.IBLSampler = (SamplerStateDescription)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty DisplacementMapSamplerProperty =
        DependencyProperty.Register("DisplacementMapSampler", typeof(SamplerStateDescription), typeof(PBRMaterial), new PropertyMetadata(DefaultSamplers.LinearSamplerWrapAni1,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.DisplacementMapSampler = (SamplerStateDescription)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty RenderAlbedoMapProperty =
        DependencyProperty.Register("RenderAlbedoMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RenderAlbedoMap = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty RenderEmissiveMapProperty =
        DependencyProperty.Register("RenderEmissiveMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RenderEmissiveMap = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty RenderRoughnessMetallicMapProperty =
        DependencyProperty.Register("RenderRoughnessMetallicMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RenderRoughnessMetallicMap = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty RenderAmbientOcclusionMapProperty =
        DependencyProperty.Register("RenderAmbientOcclusionMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RenderAmbientOcclusionMap = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// 
    /// </summary>
    public static readonly DependencyProperty RenderIrradianceMapProperty =
        DependencyProperty.Register("RenderIrradianceMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RenderIrradianceMap = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty RenderNormalMapProperty =
        DependencyProperty.Register("RenderNormalMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RenderNormalMap = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty RenderDisplacementMapProperty =
        DependencyProperty.Register("RenderDisplacementMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RenderDisplacementMap = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// The render environment map property
    /// </summary>
    public static readonly DependencyProperty RenderEnvironmentMapProperty =
        DependencyProperty.Register("RenderEnvironmentMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(false,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RenderEnvironmentMap = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// The render shadow map property
    /// </summary>
    public static readonly DependencyProperty RenderShadowMapProperty =
        DependencyProperty.Register("RenderShadowMap", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(false,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.RenderShadowMap = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// The enable automatic tangent
    /// </summary>
    public static readonly DependencyProperty EnableAutoTangentProperty =
        DependencyProperty.Register("EnableAutoTangent", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(false,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.EnableAutoTangent = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// The enable tessellation property
    /// </summary>
    public static readonly DependencyProperty EnableTessellationProperty = DependencyProperty.Register("EnableTessellation", typeof(bool), typeof(PBRMaterial),
        new PropertyMetadata(false, (d, e) =>
        {
            if (d is Material { Core: PBRMaterialCore core })
            {
                core.EnableTessellation = (bool)e.NewValue;
            }
        }));

    /// <summary>
    /// The tessellation factor at <see cref="MaxTessellationDistance"/> property
    /// </summary>
    public static readonly DependencyProperty MaxDistanceTessellationFactorProperty =
        DependencyProperty.Register("MaxDistanceTessellationFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(1.0, (d, e) =>
        {
            if (d is Material { Core: PBRMaterialCore core })
            {
                core.MaxDistanceTessellationFactor = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// The tessellation factor at <see cref="MinTessellationDistance"/> property
    /// </summary>
    public static readonly DependencyProperty MinDistanceTessellationFactorProperty =
        DependencyProperty.Register("MinDistanceTessellationFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(2.0, (d, e) =>
        {
            if (d is Material { Core: PBRMaterialCore core })
            {
                core.MinDistanceTessellationFactor = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// The maximum tessellation distance property
    /// </summary>
    public static readonly DependencyProperty MaxTessellationDistanceProperty =
        DependencyProperty.Register("MaxTessellationDistance", typeof(double), typeof(PBRMaterial), new PropertyMetadata(50.0, (d, e) =>
        {
            if (d is Material { Core: PBRMaterialCore core })
            {
                core.MaxTessellationDistance = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// The minimum tessellation distance property
    /// </summary>
    public static readonly DependencyProperty MinTessellationDistanceProperty =
        DependencyProperty.Register("MinTessellationDistance", typeof(double), typeof(PBRMaterial), new PropertyMetadata(1.0, (d, e) =>
        {
            if (d is Material { Core: PBRMaterialCore core })
            {
                core.MinTessellationDistance = (float)(double)e.NewValue;
            }
        }));

    /// <summary>
    /// The uv transform property
    /// </summary>
    public static readonly DependencyProperty UVTransformProperty =
        DependencyProperty.Register("UVTransform", typeof(UVTransform), typeof(PBRMaterial), new PropertyMetadata(UVTransform.Identity, (d, e) =>
        {
            if (d is Material { Core: PBRMaterialCore core })
            {
                core.UVTransform = (UVTransform)e.NewValue;
            }
        }));

    /// <summary>
    /// The enable flat shading property
    /// </summary>
    public static readonly DependencyProperty EnableFlatShadingProperty =
        DependencyProperty.Register("EnableFlatShading", typeof(bool), typeof(PBRMaterial), new PropertyMetadata(false, (d, e) =>
        {
            if (d is Material { Core: PBRMaterialCore core })
            {
                core.EnableFlatShading = (bool)e.NewValue;
            }
        }));

    public static readonly DependencyProperty VertexColorBlendingFactorProperty =
        DependencyProperty.Register("VertexColorBlendingFactor", typeof(double), typeof(PBRMaterial), new PropertyMetadata(0.0,
            (d, e) =>
            {
                if (d is Material { Core: PBRMaterialCore core })
                {
                    core.VertexColorBlendingFactor = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    /// Gets or sets the diffuse color for the material.
    /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb147175(v=vs.85).aspx
    /// </summary>
#if false
#elif WINUI
#elif WPF
    [TypeConverter(typeof(Color4Converter))]
#else
#error Unknown framework
#endif
    public Color4 AlbedoColor
    {
        get
        {
            return (Color4)this.GetValue(AlbedoColorProperty);
        }
        set
        {
            this.SetValue(AlbedoColorProperty, value);
        }
    }

    public Color4 EmissiveColor
    {
        get
        {
            return (Color4)this.GetValue(EmissiveColorProperty);
        }
        set
        {
            this.SetValue(EmissiveColorProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the metallic factor. If RMA map is used, for each pixel, metallic factor = <see cref="MetallicFactor"/> * RMA map B Channel
    /// </summary>
    /// <value>
    /// The metallic factor.
    /// </value>
    public double MetallicFactor
    {
        get
        {
            return (double)this.GetValue(MetallicFactorProperty);
        }
        set
        {
            this.SetValue(MetallicFactorProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the roughness factor. If RMA map is used, for each pixel, roughness factor = <see cref="RoughnessFactor"/> * RMA map G Channel
    /// </summary>
    /// <value>
    /// The roughness factor.
    /// </value>
    public double RoughnessFactor
    {
        get
        {
            return (double)this.GetValue(RoughnessFactorProperty);
        }
        set
        {
            this.SetValue(RoughnessFactorProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the ambient occlusion factor. If RMA map is used, for each pixel, ambient occlusion factor = <see cref="AmbientOcclusionFactor"/> * RMA map R Channel
    /// </summary>
    /// <value>
    /// The ambient occlusion factor.
    /// </value>
    public double AmbientOcclusionFactor
    {
        get
        {
            return (double)this.GetValue(AmbientOcclusionFactorProperty);
        }
        set
        {
            this.SetValue(AmbientOcclusionFactorProperty, value);
        }
    }

    public double ReflectanceFactor
    {
        get
        {
            return (double)this.GetValue(ReflectanceFactorProperty);
        }
        set
        {
            this.SetValue(ReflectanceFactorProperty, value);
        }
    }


    public double ClearCoatStrength
    {
        get
        {
            return (double)this.GetValue(ClearCoatStrengthProperty);
        }
        set
        {
            this.SetValue(ClearCoatStrengthProperty, value);
        }
    }


    public double ClearCoatRoughness
    {
        get
        {
            return (double)this.GetValue(ClearCoatRoughnessProperty);
        }
        set
        {
            this.SetValue(ClearCoatRoughnessProperty, value);
        }
    }

    public TextureModel? AlbedoMap
    {
        get
        {
            return (TextureModel?)this.GetValue(AlbedoMapProperty);
        }
        set
        {
            this.SetValue(AlbedoMapProperty, value);
        }
    }


    public TextureModel? EmissiveMap
    {
        get
        {
            return (TextureModel?)this.GetValue(EmissiveMapProperty);
        }
        set
        {
            this.SetValue(EmissiveMapProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the Roughness, Metallic, Ambient Occlusion map. 
    /// glTF2 defines occlusion as R channel, roughness as G channel, metalness as B channel
    /// </summary>
    /// <value>
    /// The rma map.
    /// </value>
    public TextureModel? RoughnessMetallicMap
    {
        get
        {
            return (TextureModel?)this.GetValue(RoughnessMetallicMapProperty);
        }
        set
        {
            this.SetValue(RoughnessMetallicMapProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the ambient occlusion map. 
    /// glTF2 defines occlusion as R channel, roughness as G channel, metalness as B channel.
    /// If uses RMA map, set both <see cref="RoughnessMetallicMap"/> and <see cref="AmbientOcculsionMap"/> to the same texture
    /// </summary>
    /// <value>
    /// The ao map.
    /// </value>
    public TextureModel? AmbientOcculsionMap
    {
        get
        {
            return (TextureModel?)this.GetValue(AmbientOcculsionMapProperty);
        }
        set
        {
            this.SetValue(AmbientOcculsionMapProperty, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public TextureModel? NormalMap
    {
        get
        {
            return (TextureModel?)this.GetValue(NormalMapProperty);
        }
        set
        {
            this.SetValue(NormalMapProperty, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public TextureModel? DisplacementMap
    {
        get
        {
            return (TextureModel?)this.GetValue(DisplacementMapProperty);
        }
        set
        {
            this.SetValue(DisplacementMapProperty, value);
        }
    }


    public TextureModel? IrradianceMap
    {
        get
        {
            return (TextureModel?)this.GetValue(IrradianceMapProperty);
        }
        set
        {
            this.SetValue(IrradianceMapProperty, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public SamplerStateDescription SurfaceMapSampler
    {
        get
        {
            return (SamplerStateDescription)this.GetValue(SurfaceMapSamplerProperty);
        }
        set
        {
            this.SetValue(SurfaceMapSamplerProperty, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public SamplerStateDescription IBLSampler
    {
        get
        {
            return (SamplerStateDescription)this.GetValue(IBLSamplerProperty);
        }
        set
        {
            this.SetValue(IBLSamplerProperty, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public SamplerStateDescription DisplacementMapSampler
    {
        get
        {
            return (SamplerStateDescription)this.GetValue(DisplacementMapSamplerProperty);
        }
        set
        {
            this.SetValue(DisplacementMapSamplerProperty, value);
        }
    }

#if false
#elif WINUI
#elif WPF
    [TypeConverter(typeof(Vector4Converter))]
#endif
    public Vector4 DisplacementMapScaleMask
    {
        set
        {
            SetValue(DisplacementMapScaleMaskProperty, value);
        }
        get
        {
            return (Vector4)GetValue(DisplacementMapScaleMaskProperty);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool RenderAlbedoMap
    {
        get
        {
            return (bool)this.GetValue(RenderAlbedoMapProperty);
        }
        set
        {
            this.SetValue(RenderAlbedoMapProperty, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool RenderNormalMap
    {
        get
        {
            return (bool)this.GetValue(RenderNormalMapProperty);
        }
        set
        {
            this.SetValue(RenderNormalMapProperty, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool RenderEmissiveMap
    {
        get
        {
            return (bool)this.GetValue(RenderEmissiveMapProperty);
        }
        set
        {
            this.SetValue(RenderEmissiveMapProperty, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool RenderRoughnessMetallicMap
    {
        get
        {
            return (bool)this.GetValue(RenderRoughnessMetallicMapProperty);
        }
        set
        {
            this.SetValue(RenderRoughnessMetallicMapProperty, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool RenderAmbientOcclusionMap
    {
        get
        {
            return (bool)this.GetValue(RenderAmbientOcclusionMapProperty);
        }
        set
        {
            this.SetValue(RenderAmbientOcclusionMapProperty, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool RenderIrradianceMap
    {
        get
        {
            return (bool)this.GetValue(RenderIrradianceMapProperty);
        }
        set
        {
            this.SetValue(RenderIrradianceMapProperty, value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public bool RenderDisplacementMap
    {
        get
        {
            return (bool)this.GetValue(RenderDisplacementMapProperty);
        }
        set
        {
            this.SetValue(RenderDisplacementMapProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [render environment map]. Default is false
    /// </summary>
    /// <value>
    ///   <c>true</c> if [render environment map]; otherwise, <c>false</c>.
    /// </value>
    public bool RenderEnvironmentMap
    {
        get
        {
            return (bool)GetValue(RenderEnvironmentMapProperty);
        }
        set
        {
            SetValue(RenderEnvironmentMapProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [render shadow map]. Default is false
    /// </summary>
    /// <value>
    ///   <c>true</c> if [render shadow map]; otherwise, <c>false</c>.
    /// </value>
    public bool RenderShadowMap
    {
        get
        {
            return (bool)GetValue(RenderShadowMapProperty);
        }
        set
        {
            SetValue(RenderShadowMapProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [enable automatic tangent].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable automatic tangent]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableAutoTangent
    {
        get
        {
            return (bool)GetValue(EnableAutoTangentProperty);
        }
        set
        {
            SetValue(EnableAutoTangentProperty, value);
        }
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
        get
        {
            return (double)GetValue(MaxDistanceTessellationFactorProperty);
        }
        set
        {
            SetValue(MaxDistanceTessellationFactorProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the tessellation factor at <see cref="MinTessellationDistance"/>
    /// </summary>
    /// <value>
    /// The minimum tessellation factor.
    /// </value>
    public double MinDistanceTessellationFactor
    {
        get
        {
            return (double)GetValue(MinDistanceTessellationFactorProperty);
        }
        set
        {
            SetValue(MinDistanceTessellationFactorProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the maximum tessellation distance.
    /// </summary>
    /// <value>
    /// The maximum tessellation distance.
    /// </value>
    public double MaxTessellationDistance
    {
        get
        {
            return (double)GetValue(MaxTessellationDistanceProperty);
        }
        set
        {
            SetValue(MaxTessellationDistanceProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the minimum tessellation distance.
    /// </summary>
    /// <value>
    /// The minimum tessellation distance.
    /// </value>
    public double MinTessellationDistance
    {
        get
        {
            return (double)GetValue(MinTessellationDistanceProperty);
        }
        set
        {
            SetValue(MinTessellationDistanceProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the texture uv transform.
    /// </summary>
    /// <value>
    /// The uv transform.
    /// </value>
    public UVTransform UVTransform
    {
        get
        {
            return (UVTransform)GetValue(UVTransformProperty);
        }
        set
        {
            SetValue(UVTransformProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether [enable flat shading].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable flat shading]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableFlatShading
    {
        get
        {
            return (bool)GetValue(EnableFlatShadingProperty);
        }
        set
        {
            SetValue(EnableFlatShadingProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the vertex color blending factor.
    /// Final Diffuse Color = (1 - VertexColorBlendingFactor) * Diffuse + VertexColorBlendingFactor * Vertex Color
    /// </summary>
    /// <value>
    /// The vertex color blending factor.
    /// </value>
    public double VertexColorBlendingFactor
    {
        get
        {
            return (double)GetValue(VertexColorBlendingFactorProperty);
        }
        set
        {
            SetValue(VertexColorBlendingFactorProperty, value);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PBRMaterial"/> class.
    /// </summary>
    public PBRMaterial()
    {
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="PBRMaterial"/> class.
    /// </summary>
    /// <param name="core">The core.</param>
    public PBRMaterial(PBRMaterialCore core) : base(core)
    {
        AlbedoColor = core.AlbedoColor;
        MetallicFactor = core.MetallicFactor;
        RoughnessFactor = core.RoughnessFactor;
        AmbientOcclusionFactor = core.AmbientOcclusionFactor;
        ReflectanceFactor = core.ReflectanceFactor;
        ClearCoatStrength = core.ClearCoatStrength;
        ClearCoatRoughness = core.ClearCoatRoughness;

        AlbedoMap = core.AlbedoMap;
        NormalMap = core.NormalMap;
        EmissiveMap = core.EmissiveMap;
        RoughnessMetallicMap = core.RoughnessMetallicMap;
        AmbientOcculsionMap = core.AmbientOcculsionMap;
        IrradianceMap = core.IrradianceMap;
        DisplacementMap = core.DisplacementMap;
        SurfaceMapSampler = core.SurfaceMapSampler;
        IBLSampler = core.IBLSampler;
        DisplacementMapSampler = core.DisplacementMapSampler;

        RenderAlbedoMap = core.RenderAlbedoMap;
        RenderDisplacementMap = core.RenderDisplacementMap;
        RenderEmissiveMap = core.RenderEmissiveMap;
        RenderEnvironmentMap = core.RenderEnvironmentMap;
        RenderIrradianceMap = core.RenderIrradianceMap;
        RenderNormalMap = core.RenderNormalMap;
        RenderRoughnessMetallicMap = core.RenderRoughnessMetallicMap;
        RenderAmbientOcclusionMap = core.RenderAmbientOcclusionMap;
        RenderShadowMap = core.RenderShadowMap;
        EnableAutoTangent = core.EnableAutoTangent;
        DisplacementMapScaleMask = core.DisplacementMapScaleMask;
        UVTransform = core.UVTransform;

        EnableTessellation = core.EnableTessellation;
        MaxDistanceTessellationFactor = core.MaxDistanceTessellationFactor;
        MinDistanceTessellationFactor = core.MinDistanceTessellationFactor;
        MaxTessellationDistance = core.MaxTessellationDistance;
        MinTessellationDistance = core.MinTessellationDistance;
        EnableFlatShading = core.EnableFlatShading;
        VertexColorBlendingFactor = core.VertexColorBlendingFactor;
    }

    protected override MaterialCore OnCreateCore()
    {
        return new PBRMaterialCore()
        {
            AlbedoColor = AlbedoColor,
            MetallicFactor = (float)MetallicFactor,
            RoughnessFactor = (float)RoughnessFactor,
            AmbientOcclusionFactor = (float)AmbientOcclusionFactor,
            ReflectanceFactor = (float)ReflectanceFactor,
            ClearCoatStrength = (float)ClearCoatStrength,
            ClearCoatRoughness = (float)ClearCoatRoughness,

            AlbedoMap = AlbedoMap,
            NormalMap = NormalMap,
            EmissiveMap = EmissiveMap,
            RoughnessMetallicMap = RoughnessMetallicMap,
            AmbientOcculsionMap = AmbientOcculsionMap,
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
            RenderRoughnessMetallicMap = RenderRoughnessMetallicMap,
            RenderAmbientOcclusionMap = RenderAmbientOcclusionMap,
            RenderShadowMap = RenderShadowMap,
            EnableAutoTangent = EnableAutoTangent,
            DisplacementMapScaleMask = DisplacementMapScaleMask,
            UVTransform = UVTransform,

            EnableTessellation = EnableTessellation,
            MaxDistanceTessellationFactor = (float)MaxDistanceTessellationFactor,
            MinDistanceTessellationFactor = (float)MinDistanceTessellationFactor,
            MaxTessellationDistance = (float)MaxTessellationDistance,
            MinTessellationDistance = (float)MinTessellationDistance,
            EnableFlatShading = EnableFlatShading,
            VertexColorBlendingFactor = (float)VertexColorBlendingFactor,
        };
    }

#if false
#elif WINUI
#elif WPF
    protected override Freezable CreateInstanceCore()
    {
        return CloneMaterial();
    }
#else
#error Unknown framework
#endif

    public virtual PBRMaterial CloneMaterial()
    {
        return new PBRMaterial()
        {
            AlbedoColor = AlbedoColor,
            MetallicFactor = MetallicFactor,
            RoughnessFactor = RoughnessFactor,
            AmbientOcclusionFactor = AmbientOcclusionFactor,
            ReflectanceFactor = ReflectanceFactor,
            ClearCoatStrength = ClearCoatStrength,
            ClearCoatRoughness = ClearCoatRoughness,
            AlbedoMap = AlbedoMap,
            NormalMap = NormalMap,
            EmissiveMap = EmissiveMap,
            RoughnessMetallicMap = RoughnessMetallicMap,
            AmbientOcculsionMap = AmbientOcculsionMap,
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
            RenderRoughnessMetallicMap = RenderRoughnessMetallicMap,
            RenderAmbientOcclusionMap = RenderAmbientOcclusionMap,
            RenderShadowMap = RenderShadowMap,
            EnableAutoTangent = EnableAutoTangent,
            DisplacementMapScaleMask = DisplacementMapScaleMask,
            UVTransform = UVTransform,

            EnableTessellation = EnableTessellation,
            MaxDistanceTessellationFactor = MaxDistanceTessellationFactor,
            MinDistanceTessellationFactor = MinDistanceTessellationFactor,
            MaxTessellationDistance = MaxTessellationDistance,
            MinTessellationDistance = MinTessellationDistance,
            EnableFlatShading = EnableFlatShading,
            VertexColorBlendingFactor = VertexColorBlendingFactor,
        };
    }

    public static implicit operator PBRMaterial?(PBRMaterialCore? core)
    {
        return MaterialExtension.ConvertToPBRMaterial(core);
    }
}
