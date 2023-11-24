using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model;
using HelixToolkit.SharpDX.Shaders;
using SharpDX;
using SharpDX.Direct3D11;
using System.ComponentModel;

#if WINUI
#else
using HelixToolkit.Wpf.SharpDX.Utilities;
#endif

#if WINUI
namespace HelixToolkit.WinUI.SharpDX;
#else
namespace HelixToolkit.Wpf.SharpDX;
#endif

public class DiffuseMaterial : Material
{
    /// <summary>
    /// The diffuse color property
    /// </summary>
    public static readonly DependencyProperty DiffuseColorProperty =
        DependencyProperty.Register("DiffuseColor", typeof(Color4), typeof(DiffuseMaterial), new PropertyMetadata((Color4)Color.White,
            (d, e) =>
            {
                if (d is Material { Core: DiffuseMaterialCore core })
                {
                    core.DiffuseColor = (Color4)e.NewValue;
                }
            }));

    /// <summary>
    /// Gets or sets the diffuse color for the material.
    /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb147175(v=vs.85).aspx
    /// </summary>
#if WPF
    [TypeConverter(typeof(Color4Converter))]
#endif
    public Color4 DiffuseColor
    {
        get
        {
            return (Color4)this.GetValue(DiffuseColorProperty);
        }
        set
        {
            this.SetValue(DiffuseColorProperty, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty DiffuseMapProperty =
        DependencyProperty.Register("DiffuseMap", typeof(TextureModel), typeof(DiffuseMaterial), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Material { Core: DiffuseMaterialCore core })
                {
                    core.DiffuseMap = e.NewValue as TextureModel;
                }
            }));

    /// <summary>
    /// Gets or sets the diffuse map.
    /// </summary>
    /// <value>
    /// The diffuse map.
    /// </value>
    public TextureModel? DiffuseMap
    {
        get
        {
            return (TextureModel?)this.GetValue(DiffuseMapProperty);
        }
        set
        {
            this.SetValue(DiffuseMapProperty, value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty DiffuseMapSamplerProperty =
        DependencyProperty.Register("DiffuseMapSampler", typeof(SamplerStateDescription), typeof(DiffuseMaterial), new PropertyMetadata(DefaultSamplers.LinearSamplerWrapAni4,
            (d, e) =>
            {
                if (d is Material { Core: DiffuseMaterialCore core })
                {
                    core.DiffuseMapSampler = (SamplerStateDescription)e.NewValue;
                }
            }));

    /// <summary>
    /// 
    /// </summary>
    public SamplerStateDescription DiffuseMapSampler
    {
        get
        {
            return (SamplerStateDescription)this.GetValue(DiffuseMapSamplerProperty);
        }
        set
        {
            this.SetValue(DiffuseMapSamplerProperty, value);
        }
    }
    /// <summary>
    /// The uv transform property
    /// </summary>
    public static readonly DependencyProperty UVTransformProperty =
        DependencyProperty.Register("UVTransform", typeof(UVTransform), typeof(DiffuseMaterial), new PropertyMetadata(UVTransform.Identity, (d, e) =>
        {
            if (d is Material { Core: DiffuseMaterialCore core })
            {
                core.UVTransform = (UVTransform)e.NewValue;
            }
        }));

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
    /// Gets or sets a value indicating whether whether disable lighting. Directly render diffuse color and diffuse map.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable un lit]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableUnLit
    {
        get
        {
            return (bool)GetValue(EnableUnLitProperty);
        }
        set
        {
            SetValue(EnableUnLitProperty, value);
        }
    }

    /// <summary>
    /// The enable un lit property
    /// </summary>
    public static readonly DependencyProperty EnableUnLitProperty =
        DependencyProperty.Register("EnableUnLit", typeof(bool), typeof(DiffuseMaterial), new PropertyMetadata(false,
            (d, e) =>
            {
                if (d is Material { Core: DiffuseMaterialCore core })
                {
                    core.EnableUnLit = (bool)e.NewValue;
                }
            }));

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

    public static readonly DependencyProperty EnableFlatShadingProperty =
        DependencyProperty.Register("EnableFlatShading", typeof(bool), typeof(DiffuseMaterial), new PropertyMetadata(false, (d, e) =>
        {
            if (d is Material { Core: DiffuseMaterialCore core })
            {
                core.EnableFlatShading = (bool)e.NewValue;
            }
        }));

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

    public static readonly DependencyProperty VertexColorBlendingFactorProperty =
        DependencyProperty.Register("VertexColorBlendingFactor", typeof(double), typeof(DiffuseMaterial), new PropertyMetadata(0.0,
        (d, e) =>
        {
            if (d is Material { Core: DiffuseMaterialCore core })
            {
                core.VertexColorBlendingFactor = (float)(double)e.NewValue;
            }
        }));

    public DiffuseMaterial()
    {
    }

    public DiffuseMaterial(DiffuseMaterialCore core) : base(core)
    {
        DiffuseColor = core.DiffuseColor;
        DiffuseMap = core.DiffuseMap;
        UVTransform = core.UVTransform;
        DiffuseMapSampler = core.DiffuseMapSampler;
        EnableUnLit = core.EnableUnLit;
        EnableFlatShading = core.EnableFlatShading;
        VertexColorBlendingFactor = core.VertexColorBlendingFactor;
    }

    protected override MaterialCore OnCreateCore()
    {
        return new DiffuseMaterialCore()
        {
            DiffuseColor = DiffuseColor,
            DiffuseMap = DiffuseMap,
            UVTransform = UVTransform,
            DiffuseMapSampler = DiffuseMapSampler,
            EnableUnLit = EnableUnLit,
            EnableFlatShading = EnableFlatShading,
            VertexColorBlendingFactor = (float)VertexColorBlendingFactor,
        };
    }

    public virtual DiffuseMaterial CloneMaterial()
    {
        return new DiffuseMaterial()
        {
            DiffuseColor = DiffuseColor,
            DiffuseMap = DiffuseMap,
            DiffuseMapSampler = DiffuseMapSampler,
            UVTransform = UVTransform,
            Name = Name,
            EnableUnLit = EnableUnLit,
            EnableFlatShading = EnableFlatShading,
            VertexColorBlendingFactor = VertexColorBlendingFactor,
        };
    }

#if WPF
    protected override Freezable CreateInstanceCore()
    {
        return CloneMaterial();
    }
#endif
}
