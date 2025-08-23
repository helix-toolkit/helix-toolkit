using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;

#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Model;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Model;
#elif AVALONIA
using HelixToolkit.Avalonia.SharpDX.Model;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX;
#elif AVALONIA
namespace HelixToolkit.Avalonia.SharpDX;
#else
#error Unknown framework
#endif

/// <summary>
/// 
/// </summary>
public class CoordinateSystemModel3D : ScreenSpacedElement3D
{
    /// <summary>
    /// <see cref="AxisXColor"/>
    /// </summary>
    public static readonly DependencyProperty AxisXColorProperty =
        HelixProperty.Register<CoordinateSystemModel3D, UIColor>("AxisXColor",
            UIColors.Red,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: CoordinateSystemNode node })
                {
                    node.AxisXColor = ((UIColor)e.NewValue!).ToColor4();
                }
            });

    /// <summary>
    /// <see cref="AxisYColor"/>
    /// </summary>
    public static readonly DependencyProperty AxisYColorProperty =
        HelixProperty.Register<CoordinateSystemModel3D, UIColor>("AxisYColor",
            UIColors.Green,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: CoordinateSystemNode node })
                {
                    node.AxisYColor = ((UIColor)e.NewValue!).ToColor4();
                }
            });

    /// <summary>
    /// <see cref="AxisZColor"/>
    /// </summary>
    public static readonly DependencyProperty AxisZColorProperty =
        HelixProperty.Register<CoordinateSystemModel3D, UIColor>("AxisZColor",
            UIColors.Blue,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: CoordinateSystemNode node })
                {
                    node.AxisZColor = ((UIColor)e.NewValue!).ToColor4();
                }
            });
    /// <summary>
    /// 
    /// </summary>
    public static readonly DependencyProperty LabelColorProperty =
        HelixProperty.Register<CoordinateSystemModel3D, UIColor>("LabelColor",
            UIColors.Gray,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: CoordinateSystemNode node })
                {
                    node.LabelColor = ((UIColor)e.NewValue!).ToColor4();
                }
            });

    /// <summary>
    /// The coordinate system label x property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelXProperty =
        HelixProperty.Register<CoordinateSystemModel3D, string>("CoordinateSystemLabelX",
            "X",
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: CoordinateSystemNode node })
                {
                    node.LabelX = (string)e.NewValue!;
                }
            });

    /// <summary>
    /// The coordinate system label Y property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelYProperty =
        HelixProperty.Register<CoordinateSystemModel3D, string>("CoordinateSystemLabelY",
            "Y",
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: CoordinateSystemNode node })
                {
                    node.LabelY = (string)e.NewValue!;
                }
            });

    /// <summary>
    /// The coordinate system label Z property
    /// </summary>
    public static readonly DependencyProperty CoordinateSystemLabelZProperty =
        HelixProperty.Register<CoordinateSystemModel3D, string>("CoordinateSystemLabelZ",
            "Z",
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: CoordinateSystemNode node })
                {
                    node.LabelZ = (string)e.NewValue!;
                }
            });

    /// <summary>
    /// Axis X Color
    /// </summary>
    public UIColor AxisXColor
    {
        set
        {
            SetValue(AxisXColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(AxisXColorProperty)!;
        }
    }

    /// <summary>
    /// Axis Y Color
    /// </summary>
    public UIColor AxisYColor
    {
        set
        {
            SetValue(AxisYColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(AxisYColorProperty)!;
        }
    }

    /// <summary>
    /// Axis Z Color
    /// </summary>
    public UIColor AxisZColor
    {
        set
        {
            SetValue(AxisZColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(AxisZColorProperty)!;
        }
    }

    /// <summary>
    /// Label Color
    /// </summary>
    public UIColor LabelColor
    {
        set
        {
            SetValue(LabelColorProperty, value);
        }
        get
        {
            return (UIColor)GetValue(LabelColorProperty)!;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public string CoordinateSystemLabelX
    {
        set
        {
            SetValue(CoordinateSystemLabelXProperty, value);
        }
        get
        {
            return (string)GetValue(CoordinateSystemLabelXProperty)!;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public string CoordinateSystemLabelY
    {
        set
        {
            SetValue(CoordinateSystemLabelYProperty, value);
        }
        get
        {
            return (string)GetValue(CoordinateSystemLabelYProperty)!;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public string CoordinateSystemLabelZ
    {
        set
        {
            SetValue(CoordinateSystemLabelZProperty, value);
        }
        get
        {
            return (string)GetValue(CoordinateSystemLabelZProperty)!;
        }
    }

    protected override SceneNode OnCreateSceneNode()
    {
        return new CoordinateSystemNode();
    }

    public override bool HitTest(HitTestContext? context, ref List<HitTestResult> hits)
    {
        return false;
    }
}
