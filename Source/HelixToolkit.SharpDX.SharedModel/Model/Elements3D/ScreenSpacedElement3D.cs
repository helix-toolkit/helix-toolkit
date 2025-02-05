using HelixToolkit.SharpDX;
using HelixToolkit.SharpDX.Model.Scene;
#if false
#elif WINUI
using HelixToolkit.WinUI.SharpDX.Elements2D;
using HelixToolkit.WinUI.SharpDX.Model;
using Microsoft.UI.Xaml.Data;
#elif WPF
using HelixToolkit.Wpf.SharpDX.Elements2D;
using HelixToolkit.Wpf.SharpDX.Model;
using System.Windows;
using System.Windows.Data;
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
/// ScreenSpacedElement3D uses a fixed camera to render model (Mainly used for view box and coordinate system rendering) onto screen which is separated from viewport camera.
/// <para>
/// Default fix camera is perspective camera with FOV 45 degree and camera distance = 20. Look direction is always looking at (0,0,0).
/// </para>
/// <para>
/// User must properly scale the model to fit into the camera frustum. The usual maximum size is from (5,5,5) to (-5,-5,-5) bounding box.
/// </para>
/// <para>
/// User can use <see cref="ScreenSpacedNode.SizeScale"/> to scale the size of the rendering.
/// </para>
/// </summary>
public abstract class ScreenSpacedElement3D : GroupModel3D
{
    /// <summary>
    /// <see cref="RelativeScreenLocationX"/>
    /// </summary>
    public static readonly DependencyProperty RelativeScreenLocationXProperty = DependencyProperty.Register("RelativeScreenLocationX", typeof(double), typeof(ScreenSpacedElement3D),
        new PropertyMetadata(-0.8,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ScreenSpacedNode node })
                {
                    node.RelativeScreenLocationX = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    /// <see cref="RelativeScreenLocationY"/>
    /// </summary>
    public static readonly DependencyProperty RelativeScreenLocationYProperty = DependencyProperty.Register("RelativeScreenLocationY", typeof(double), typeof(ScreenSpacedElement3D),
        new PropertyMetadata(-0.8,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ScreenSpacedNode node })
                {
                    node.RelativeScreenLocationY = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    /// <see cref="SizeScale"/>
    /// </summary>
    public static readonly DependencyProperty SizeScaleProperty = DependencyProperty.Register("SizeScale", typeof(double), typeof(ScreenSpacedElement3D),
        new PropertyMetadata(1.0,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ScreenSpacedNode node })
                {
                    node.SizeScale = (float)(double)e.NewValue;
                }
            }));

    /// <summary>
    /// The enable mover property
    /// </summary>
    public static readonly DependencyProperty EnableMoverProperty =
        DependencyProperty.Register("EnableMover", typeof(bool), typeof(ScreenSpacedElement3D), new PropertyMetadata(true));

    /// <summary>
    /// The mode property
    /// </summary>
    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register("Mode", typeof(ScreenSpacedMode), typeof(ScreenSpacedElement3D), new PropertyMetadata(ScreenSpacedMode.RelativeScreenSpaced,
            (d, e) =>
            {
                if (d is Element3DCore { SceneNode: ScreenSpacedNode node })
                {
                    node.Mode = (ScreenSpacedMode)e.NewValue;
                }
            }));

    // Using a DependencyProperty as the backing store for CameraType.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty CameraTypeProperty =
        DependencyProperty.Register("CameraType", typeof(ScreenSpacedCameraType), typeof(ScreenSpacedElement3D), new PropertyMetadata(ScreenSpacedCameraType.Auto,
            (d, e) =>
            {
                if (d is Element3DCore element && element.SceneNode is ScreenSpacedNode node)
                {
                    node.CameraType = (ScreenSpacedCameraType)e.NewValue;
                }
            }));

    /// <summary>
    /// The absolute position3 d property
    /// </summary>
    public static readonly DependencyProperty AbsolutePosition3DProperty =
        DependencyProperty.Register("AbsolutePosition3D", typeof(Point3D), typeof(ScreenSpacedElement3D), new PropertyMetadata(new Point3D(), (d, e) =>
        {
            if (d is Element3DCore { SceneNode: ScreenSpacedNode node })
            {
                node.AbsolutePosition3D = ((Point3D)e.NewValue).ToVector3();
            }
        }));

    public static readonly DependencyProperty FarPlaneDistanceProperty =
        DependencyProperty.Register("FarPlaneDistance", typeof(double), typeof(ScreenSpacedElement3D), new PropertyMetadata(1e3, (d, e) =>
        {
            if (d is Element3DCore element && element.SceneNode is ScreenSpacedNode node)
            {
                node.FarPlane = (float)e.NewValue;
            }
        }));

    public static readonly DependencyProperty NearPlaneDistanceProperty =
        DependencyProperty.Register("NearPlaneDistance", typeof(double), typeof(ScreenSpacedElement3D), new PropertyMetadata(1e-2, (d, e) =>
        {
            if (d is Element3DCore element && element.SceneNode is ScreenSpacedNode node)
            {
                node.NearPlane = (float)e.NewValue;
            }
        }));

    /// <summary>
    /// Gets or sets a value indicating whether [enable mover].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [enable mover]; otherwise, <c>false</c>.
    /// </value>
    public bool EnableMover
    {
        get
        {
            return (bool)GetValue(EnableMoverProperty);
        }
        set
        {
            SetValue(EnableMoverProperty, value);
        }
    }

    /// <summary>
    /// Relative Location X on screen. Range from -1~1
    /// </summary>
    public double RelativeScreenLocationX
    {
        set
        {
            SetValue(RelativeScreenLocationXProperty, value);
        }
        get
        {
            return (double)GetValue(RelativeScreenLocationXProperty);
        }
    }

    /// <summary>
    /// Relative Location Y on screen. Range from -1~1
    /// </summary>
    public double RelativeScreenLocationY
    {
        set
        {
            SetValue(RelativeScreenLocationYProperty, value);
        }
        get
        {
            return (double)GetValue(RelativeScreenLocationYProperty);
        }
    }

    /// <summary>
    /// Size scaling
    /// </summary>
    public double SizeScale
    {
        set
        {
            SetValue(SizeScaleProperty, value);
        }
        get
        {
            return (double)GetValue(SizeScaleProperty);
        }
    }

    /// <summary>
    /// Gets or sets the mode.
    /// </summary>
    /// <value>
    /// The mode.
    /// </value>
    public ScreenSpacedMode Mode
    {
        get
        {
            return (ScreenSpacedMode)GetValue(ModeProperty);
        }
        set
        {
            SetValue(ModeProperty, value);
        }
    }

    /// <summary>
    /// Only being used when <see cref="Mode"/> is RelativeScreenSpaced
    /// </summary>
    /// <value>
    /// The type of the camera.
    /// </value>
    public ScreenSpacedCameraType CameraType
    {
        get
        {
            return (ScreenSpacedCameraType)GetValue(CameraTypeProperty);
        }
        set
        {
            SetValue(CameraTypeProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the absolute position in 3d. Use by <see cref="Mode"/> = <see cref="ScreenSpacedMode.AbsolutePosition3D"/>
    /// </summary>
    /// <value>
    /// The absolute position3 d.
    /// </value>
    public Point3D AbsolutePosition3D
    {
        get
        {
            return (Point3D)GetValue(AbsolutePosition3DProperty);
        }
        set
        {
            SetValue(AbsolutePosition3DProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the far plane distance.
    /// </summary>
    /// <value>
    /// The far plane distance.
    /// </value>
    public double FarPlaneDistance
    {
        get
        {
            return (double)GetValue(FarPlaneDistanceProperty);
        }
        set
        {
            SetValue(FarPlaneDistanceProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the near plane distance.
    /// </summary>
    /// <value>
    /// The near plane distance.
    /// </value>
    public double NearPlaneDistance
    {
        get
        {
            return (double)GetValue(NearPlaneDistanceProperty);
        }
        set
        {
            SetValue(NearPlaneDistanceProperty, value);
        }
    }

    protected override void AssignDefaultValuesToSceneNode(SceneNode node)
    {
        if (node is ScreenSpacedNode n)
        {
            n.RelativeScreenLocationX = (float)this.RelativeScreenLocationX;
            n.RelativeScreenLocationY = (float)this.RelativeScreenLocationY;
            n.SizeScale = (float)this.SizeScale;
            n.AbsolutePosition3D = AbsolutePosition3D.ToVector3();
            n.Mode = Mode;
        }
        base.AssignDefaultValuesToSceneNode(node);
        InitializeMover();
    }

    #region 2D stuffs
    public RelativePositionCanvas2D MoverCanvas
    {
        private set; get;
    } = new RelativePositionCanvas2D()
    {
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch
    };

    private ScreenSpacePositionMoverBase? mover;

    private bool isMoverInitialized = false;

    private void InitializeMover()
    {
        if (isMoverInitialized)
        {
            return;
        }
        mover = OnCreateMover();
        MoverCanvas.Children.Add(mover);
        SetBinding(nameof(RelativeScreenLocationX), mover, RelativePositionCanvas2D.RelativeXProperty, this, BindingMode.TwoWay);
        SetBinding(nameof(RelativeScreenLocationY), mover, RelativePositionCanvas2D.RelativeYProperty, this, BindingMode.TwoWay);
        SetBinding(nameof(IsRendering), mover, Element2D.VisibilityProperty, this, BindingMode.OneWay, new BoolToVisibilityConverter());
        SetBinding(nameof(EnableMover), mover, ScreenSpacePositionMoverBase.EnableMoverProperty, this, BindingMode.OneWay);
        mover.OnMoveClicked += Mover_OnMoveClicked;
        isMoverInitialized = true;
    }

    protected virtual ScreenSpacePositionMoverBase OnCreateMover()
    {
        return new ScreenSpacePositionMover();
    }

    private void Mover_OnMoveClicked(object? sender, ScreenSpaceMoveDirArgs e)
    {
        switch (e.Direction)
        {
            case ScreenSpaceMoveDirection.LeftTop:
                this.RelativeScreenLocationX = -Math.Abs(RelativeScreenLocationX);
                this.RelativeScreenLocationY = Math.Abs(RelativeScreenLocationY);
                break;
            case ScreenSpaceMoveDirection.LeftBottom:
                this.RelativeScreenLocationX = -Math.Abs(RelativeScreenLocationX);
                this.RelativeScreenLocationY = -Math.Abs(RelativeScreenLocationY);
                break;
            case ScreenSpaceMoveDirection.RightTop:
                this.RelativeScreenLocationX = Math.Abs(RelativeScreenLocationX);
                this.RelativeScreenLocationY = Math.Abs(RelativeScreenLocationY);
                break;
            case ScreenSpaceMoveDirection.RightBottom:
                this.RelativeScreenLocationX = Math.Abs(RelativeScreenLocationX);
                this.RelativeScreenLocationY = -Math.Abs(RelativeScreenLocationY);
                break;
            default:
                break;
        }
    }

    private static void SetBinding(string path, DependencyObject dobj, DependencyProperty property, object viewModel, BindingMode mode = BindingMode.TwoWay, IValueConverter? converter = null)
    {
        var binding = new Binding
        {
            Path = new PropertyPath(path),
            Source = viewModel,
            Mode = mode
        };

        if (converter != null)
        {
            binding.Converter = converter;
        }
        BindingOperations.SetBinding(dobj, property, binding);
    }
    #endregion
}
