using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Model.Scene;
using SharpDX.Direct3D11;
using SharpDX;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Model;
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

/// <summary>
/// Provides a base class for a scene model which contains geometry
/// </summary>
public abstract class GeometryModel3D : Element3D, IHitable, IThrowingShadow, IApplyPostEffect
{
    #region DependencyProperties        
    /// <summary>
    /// The geometry property
    /// </summary>
    public static readonly DependencyProperty GeometryProperty =
        DependencyProperty.Register("Geometry", typeof(Geometry3D), typeof(GeometryModel3D), new PropertyMetadata(null,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: GeometryNode core })
                {
                    core.Geometry = e.NewValue as Geometry3D;
                }
            }));

    public static readonly DependencyProperty IsThrowingShadowProperty =
            DependencyProperty.Register("IsThrowingShadow", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(false, (d, e) =>
            {
                if (d is Element3D { SceneNode: IThrowingShadow t })
                {
                    t.IsThrowingShadow = (bool)e.NewValue;
                }
            }));

    /// <summary>
    /// The depth bias property
    /// </summary>
    public static readonly DependencyProperty DepthBiasProperty =
        DependencyProperty.Register("DepthBias", typeof(int), typeof(GeometryModel3D), new PropertyMetadata(0, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: GeometryNode core })
            {
                core.DepthBias = (int)e.NewValue;
            }
        }));
    /// <summary>
    /// The slope scaled depth bias property
    /// </summary>
    public static readonly DependencyProperty SlopeScaledDepthBiasProperty =
        DependencyProperty.Register("SlopeScaledDepthBias", typeof(double), typeof(GeometryModel3D), new PropertyMetadata(0.0, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: GeometryNode core })
            {
                core.SlopeScaledDepthBias = (float)(double)e.NewValue;
            }
        }));
    /// <summary>
    /// The is selected property
    /// </summary>
    public static readonly DependencyProperty IsSelectedProperty =
        DependencyProperty.Register("IsSelected", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(false));
    /// <summary>
    /// The is multisample enabled property
    /// </summary>
    public static readonly DependencyProperty IsMultisampleEnabledProperty =
        DependencyProperty.Register("IsMultisampleEnabled", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: GeometryNode core })
            {
                core.IsMSAAEnabled = (bool)e.NewValue;
            }
        }));
    /// <summary>
    /// The fill mode property
    /// </summary>
    public static readonly DependencyProperty FillModeProperty = DependencyProperty.Register("FillMode", typeof(FillMode), typeof(GeometryModel3D),
        new PropertyMetadata(FillMode.Solid, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: GeometryNode core })
            {
                core.FillMode = (FillMode)e.NewValue;
            }
        }));
    /// <summary>
    /// The is scissor enabled property
    /// </summary>
    public static readonly DependencyProperty IsScissorEnabledProperty =
        DependencyProperty.Register("IsScissorEnabled", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: GeometryNode core })
            {
                core.IsScissorEnabled = (bool)e.NewValue;
            }
        }));
    /// <summary>
    /// The enable view frustum check property
    /// </summary>
    public static readonly DependencyProperty EnableViewFrustumCheckProperty =
        DependencyProperty.Register("EnableViewFrustumCheck", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: GeometryNode core })
                {
                    core.EnableViewFrustumCheck = (bool)e.NewValue;
                }
            }));
    /// <summary>
    /// The is depth clip enabled property
    /// </summary>
    public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(GeometryModel3D),
        new PropertyMetadata(true, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: GeometryNode core })
            {
                core.IsDepthClipEnabled = (bool)e.NewValue;
            }
        }));


    /// <summary>
    /// The post effects property
    /// </summary>
    public static readonly DependencyProperty PostEffectsProperty =
        DependencyProperty.Register("PostEffects", typeof(string), typeof(GeometryModel3D), new PropertyMetadata(string.Empty, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: GeometryNode core })
            {
                core.PostEffects = (string)e.NewValue;
            }
        }));

    /// <summary>
    /// The always hittable property
    /// </summary>
    public static readonly DependencyProperty AlwaysHittableProperty =
        DependencyProperty.Register("AlwaysHittable", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(false, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: GeometryNode core })
            {
                core.AlwaysHittable = (bool)e.NewValue;
            }
        }));



    /// <summary>
    /// Gets or sets the geometry.
    /// </summary>
    /// <value>
    /// The geometry.
    /// </value>
    public Geometry3D? Geometry
    {
        get
        {
            return (Geometry3D?)this.GetValue(GeometryProperty);
        }
        set
        {
            this.SetValue(GeometryProperty, value);
        }
    }
    /// <summary>
    /// <see cref="IThrowingShadow.IsThrowingShadow"/>
    /// </summary>
    public bool IsThrowingShadow
    {
        set
        {
            SetValue(IsThrowingShadowProperty, value);
        }
        get
        {
            return (bool)GetValue(IsThrowingShadowProperty);
        }
    }
    /// <summary>
    /// List of instance matrix.
    /// </summary>
    public static readonly DependencyProperty InstancesProperty =
        DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(GeometryModel3D), new PropertyMetadata(null, (d, e) =>
        {
            if (d is Element3DCore { SceneNode: GeometryNode core })
            {
                core.Instances = e.NewValue as IList<Matrix>;
            }
        }));

    /// <summary>
    /// List of instance matrix. 
    /// </summary>
    public IList<Matrix> Instances
    {
        get
        {
            return (IList<Matrix>)this.GetValue(InstancesProperty);
        }
        set
        {
            this.SetValue(InstancesProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the depth bias.
    /// </summary>
    /// <value>
    /// The depth bias.
    /// </value>
    public int DepthBias
    {
        get
        {
            return (int)this.GetValue(DepthBiasProperty);
        }
        set
        {
            this.SetValue(DepthBiasProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets the slope scaled depth bias.
    /// </summary>
    /// <value>
    /// The slope scaled depth bias.
    /// </value>
    public double SlopeScaledDepthBias
    {
        get
        {
            return (double)this.GetValue(SlopeScaledDepthBiasProperty);
        }
        set
        {
            this.SetValue(SlopeScaledDepthBiasProperty, value);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is selected.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
    /// </value>
    public bool IsSelected
    {
        get
        {
            return (bool)this.GetValue(IsSelectedProperty);
        }
        set
        {
            this.SetValue(IsSelectedProperty, value);
        }
    }

    /// <summary>
    /// Only works under FillMode = Wireframe. MSAA is determined by viewport MSAA settings for FillMode = Solid
    /// </summary>
    public bool IsMultisampleEnabled
    {
        set
        {
            SetValue(IsMultisampleEnabledProperty, value);
        }
        get
        {
            return (bool)GetValue(IsMultisampleEnabledProperty);
        }
    }
    /// <summary>
    /// Gets or sets the fill mode.
    /// </summary>
    /// <value>
    /// The fill mode.
    /// </value>
    public FillMode FillMode
    {
        set
        {
            SetValue(FillModeProperty, value);
        }
        get
        {
            return (FillMode)GetValue(FillModeProperty);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is scissor enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is scissor enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsScissorEnabled
    {
        set
        {
            SetValue(IsScissorEnabledProperty, value);
        }
        get
        {
            return (bool)GetValue(IsScissorEnabledProperty);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this instance is depth clip enabled.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is depth clip enabled; otherwise, <c>false</c>.
    /// </value>
    public bool IsDepthClipEnabled
    {
        set
        {
            SetValue(IsDepthClipEnabledProperty, value);
        }
        get
        {
            return (bool)GetValue(IsDepthClipEnabledProperty);
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [enable view frustum check].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable view frustum check]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableViewFrustumCheck
    {
        set
        {
            SetValue(EnableViewFrustumCheckProperty, value);
        }
        get
        {
            return (bool)GetValue(EnableViewFrustumCheckProperty);
        }
    }

    public string PostEffects
    {
        get
        {
            return (string)GetValue(PostEffectsProperty);
        }
        set
        {
            SetValue(PostEffectsProperty, value);
        }
    }


    /// <summary>
    /// Gets or sets a value indicating whether [always hittable] even it is not rendering.
    /// </summary>
    /// <value>
    ///   <c>true</c> if [always hittable]; otherwise, <c>false</c>.
    /// </value>
    public bool AlwaysHittable
    {
        get
        {
            return (bool)GetValue(AlwaysHittableProperty);
        }
        set
        {
            SetValue(AlwaysHittableProperty, value);
        }
    }
    #endregion

    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        if (node is GeometryNode n)
        {
            n.DepthBias = this.DepthBias;
            n.IsDepthClipEnabled = this.IsDepthClipEnabled;
            n.SlopeScaledDepthBias = (float)this.SlopeScaledDepthBias;
            n.IsMSAAEnabled = this.IsMultisampleEnabled;
            n.FillMode = this.FillMode;
            n.IsScissorEnabled = this.IsScissorEnabled;
            n.EnableViewFrustumCheck = this.EnableViewFrustumCheck;
            n.PostEffects = this.PostEffects;
            n.AlwaysHittable = this.AlwaysHittable;
        }
        base.AssignDefaultValuesToSceneNode(node);
    }
}
