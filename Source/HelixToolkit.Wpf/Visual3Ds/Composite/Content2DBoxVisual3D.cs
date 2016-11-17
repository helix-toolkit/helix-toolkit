using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf
{

  [Flags]
  public enum FaceFlags
  {
    Front = 1,
    Back = 2,
    Right = 4,
    Left = 8,
    Top = 16,
    Bottom = 32,
    X = Front | Back,
    Y = Right | Left,
    Z = Top | Bottom,
    All = Front | Back | Right | Left | Top | Bottom
  }

  public class Content2DBoxVisual3D : ModelVisual3D
  {
    #region 属性
    #region 中心
    public static readonly DependencyProperty CenterProperty =
        DependencyProperty.Register("Center", typeof(Point3D), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(new Point3D(0, 0, 0), VisualModelChanged));
    public Point3D Center
    {
      get
      {
        return (Point3D)GetValue(CenterProperty);
      }

      set
      {
        SetValue(CenterProperty, value);
      }
    }
    #endregion 中心

    #region 尺寸

    public static readonly DependencyProperty HeightProperty = DependencyProperty.Register(
        "Height", typeof(double), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(300.0, VisualModelChanged));
    public double Height
    {
      get
      {
        return (double)GetValue(HeightProperty);
      }

      set
      {
        SetValue(HeightProperty, value);
      }
    }

    public static readonly DependencyProperty LengthProperty = DependencyProperty.Register(
        "Length", typeof(double), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(400.0, VisualModelChanged));
    public double Length
    {
      get
      {
        return (double)GetValue(LengthProperty);
      }

      set
      {
        SetValue(LengthProperty, value);
      }
    }

    public static readonly DependencyProperty WidthProperty =
        DependencyProperty.Register(
        "Width", typeof(double), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(500.0, VisualModelChanged));
    public double Width
    {
      get
      {
        return (double)GetValue(WidthProperty);
      }

      set
      {
        SetValue(WidthProperty, value);
      }
    }
    #endregion 尺寸

    #region 方向
    public static readonly DependencyProperty ModelUpDirectionProperty =
        DependencyProperty.Register("ModelUpDirection", typeof(Vector3D), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(new Vector3D(0, 0, 1), VisualModelChanged));
    public Vector3D ModelUpDirection
    {
      get
      {
        return (Vector3D)GetValue(ModelUpDirectionProperty);
      }

      set
      {
        SetValue(ModelUpDirectionProperty, value);
      }
    }
    #endregion 方向

    #region 面选择
    public static readonly DependencyProperty FaceSelectProperty =
        DependencyProperty.Register("FaceSelect", typeof(FaceFlags), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(FaceFlags.All, VisualModelChanged));
    public FaceFlags FaceSelect
    {
      get
      {
        return (FaceFlags)GetValue(FaceSelectProperty);
      }

      set
      {
        SetValue(FaceSelectProperty, value);
      }
    }
    #endregion 面选择

    #region 内容
    public static readonly DependencyProperty FrontVisualProperty =
        DependencyProperty.Register("FrontVisual", typeof(Visual), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(null, VisualModelChanged));

    public Visual FrontVisual
    {
      get
      {
        return (Visual)GetValue(FrontVisualProperty);
      }

      set
      {
        SetValue(FrontVisualProperty, value);
      }
    }

    public static readonly DependencyProperty BackVisualProperty =
        DependencyProperty.Register("BackVisual", typeof(Visual), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(null, VisualModelChanged));

    public Visual BackVisual
    {
      get
      {
        return (Visual)GetValue(BackVisualProperty);
      }

      set
      {
        SetValue(BackVisualProperty, value);
      }
    }

    public static readonly DependencyProperty RightVisualProperty =
        DependencyProperty.Register("RightVisual", typeof(Visual), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(null, VisualModelChanged));

    public Visual RightVisual
    {
      get
      {
        return (Visual)GetValue(RightVisualProperty);
      }

      set
      {
        SetValue(RightVisualProperty, value);
      }
    }

    public static readonly DependencyProperty LeftVisualProperty =
        DependencyProperty.Register("LeftVisual", typeof(Visual), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(null, VisualModelChanged));

    public Visual LeftVisual
    {
      get
      {
        return (Visual)GetValue(LeftVisualProperty);
      }

      set
      {
        SetValue(LeftVisualProperty, value);
      }
    }

    public static readonly DependencyProperty TopVisualProperty =
        DependencyProperty.Register("TopVisual", typeof(Visual), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(null, VisualModelChanged));

    public Visual TopVisual
    {
      get
      {
        return (Visual)GetValue(TopVisualProperty);
      }

      set
      {
        SetValue(TopVisualProperty, value);
      }
    }

    public static readonly DependencyProperty BottomVisualProperty =
        DependencyProperty.Register("BottomVisual", typeof(Visual), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(null, VisualModelChanged));

    public Visual BottomVisual
    {
      get
      {
        return (Visual)GetValue(BottomVisualProperty);
      }

      set
      {
        SetValue(BottomVisualProperty, value);
      }
    }
    #endregion 内容

    #region 材质
    public static DiffuseMaterial DefaultFrontMaterial =
        new DiffuseMaterial(Brushes.Red);
    public static readonly DependencyProperty FrontMaterialProperty =
        DependencyProperty.Register("FrontMaterial", typeof(Material), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(DefaultFrontMaterial, VisualModelChanged));

    public Material FrontMaterial
    {
      get
      {
        return (Material)GetValue(FrontMaterialProperty);
      }

      set
      {
        SetValue(FrontMaterialProperty, value);
      }
    }

    public static DiffuseMaterial DefaultBackMaterial =
        new DiffuseMaterial(Brushes.Green);
    public static readonly DependencyProperty BackMaterialProperty =
        DependencyProperty.Register("BackMaterial", typeof(Material), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(DefaultBackMaterial, VisualModelChanged));

    public Material BackMaterial
    {
      get
      {
        return (Material)GetValue(BackMaterialProperty);
      }

      set
      {
        SetValue(BackMaterialProperty, value);
      }
    }

    public static DiffuseMaterial DefaultRightMaterial =
        new DiffuseMaterial(Brushes.Blue);
    public static readonly DependencyProperty RightMaterialProperty =
        DependencyProperty.Register("RightMaterial", typeof(Material), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(DefaultRightMaterial, VisualModelChanged));

    public Material RightMaterial
    {
      get
      {
        return (Material)GetValue(RightMaterialProperty);
      }

      set
      {
        SetValue(RightMaterialProperty, value);
      }
    }

    public static DiffuseMaterial DefaultLeftMaterial =
        new DiffuseMaterial(Brushes.Yellow);
    public static readonly DependencyProperty LeftMaterialProperty =
        DependencyProperty.Register("LeftMaterial", typeof(Material), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(DefaultLeftMaterial, VisualModelChanged));

    public Material LeftMaterial
    {
      get
      {
        return (Material)GetValue(LeftMaterialProperty);
      }

      set
      {
        SetValue(LeftMaterialProperty, value);
      }
    }

    public static DiffuseMaterial DefaultTopMaterial =
        new DiffuseMaterial(Brushes.Violet);
    public static readonly DependencyProperty TopMaterialProperty =
        DependencyProperty.Register("TopMaterial", typeof(Material), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(DefaultTopMaterial, VisualModelChanged));

    public Material TopMaterial
    {
      get
      {
        return (Material)GetValue(TopMaterialProperty);
      }

      set
      {
        SetValue(TopMaterialProperty, value);
      }
    }

    public static DiffuseMaterial DefaultBottomMaterial =
        new DiffuseMaterial(Brushes.YellowGreen);
    public static readonly DependencyProperty BottomMaterialProperty =
        DependencyProperty.Register("BottomMaterial", typeof(Material), typeof(Content2DBoxVisual3D),
        new UIPropertyMetadata(DefaultBottomMaterial, VisualModelChanged));

    public Material BottomMaterial
    {
      get
      {
        return (Material)GetValue(BottomMaterialProperty);
      }

      set
      {
        SetValue(BottomMaterialProperty, value);
      }
    }
    #endregion 材质
    #endregion 属性

    #region 私有变量
    private readonly Dictionary<object, Vector3D> faceNormals = new Dictionary<object, Vector3D>();
    private readonly Dictionary<object, Vector3D> faceUpVectors = new Dictionary<object, Vector3D>();
    #endregion 私有变量

    public Content2DBoxVisual3D()
    {
      UpdateVisuals();
    }

    private void AddFace(Vector3D normal, Vector3D up, double dist, double width, double height, int faceselect)
    {
      var builder = new MeshBuilder(false, true);
      builder.AddCubeFace(Center, normal, up, dist, width, height);
      var geometry = builder.ToMesh();
      geometry.Freeze();

      Visual v = new Button();
      Material m = new DiffuseMaterial(Brushes.YellowGreen);
      switch (faceselect)
      {
        case (int)FaceFlags.Front:
          v = FrontVisual;
          m = FrontMaterial;
          break;
        case (int)FaceFlags.Back:
          v = BackVisual;
          m = BackMaterial;
          break;
        case (int)FaceFlags.Right:
          v = RightVisual;
          m = RightMaterial;
          break;
        case (int)FaceFlags.Left:
          v = LeftVisual;
          m = LeftMaterial;
          break;
        case (int)FaceFlags.Top:
          v = TopVisual;
          m = TopMaterial;
          break;
        case (int)FaceFlags.Bottom:
          v = BottomVisual;
          m = BottomMaterial;
          break;
      }

      Material tFrontMaterial = m.Clone();
      Viewport2DVisual3D.SetIsVisualHostMaterial(tFrontMaterial, true);
      if (v != null)
      {
        ContentPresenter cp = new ContentPresenter();
        cp.Content = v;

        var element =
            new Viewport2DVisual3D
            {
              Visual = cp,
              Geometry = geometry,
              Material = tFrontMaterial
            };

        faceNormals.Add(element, normal);
        faceUpVectors.Add(element, up);

        Children.Add(element);
      }
    }

    public void UpdateVisuals()
    {
      //Children.Clear();

      var up = ModelUpDirection;
      var right = new Vector3D(0, 1, 0);
      if (up.Z != 1)
      {
        right = new Vector3D(0, 0, 1);
      }
      var front = Vector3D.CrossProduct(right, up);

      if ((FaceSelect & FaceFlags.Front) != 0)
      {
        AddFace(front, up, Length, Width, Height, (int)FaceFlags.Front);
      }

      if ((FaceSelect & FaceFlags.Back) != 0)
      {
        AddFace(-front, up, Length, Width, Height, (int)FaceFlags.Back);
      }

      if ((FaceSelect & FaceFlags.Right) != 0)
      {
        AddFace(right, up, Width, Length, Height, (int)FaceFlags.Right);
      }

      if ((FaceSelect & FaceFlags.Left) != 0)
      {
        AddFace(-right, up, Width, Length, Height, (int)FaceFlags.Left);
      }

      if ((FaceSelect & FaceFlags.Top) != 0)
      {
        AddFace(up, right, Height, Length, Width, (int)FaceFlags.Top);
      }

      if ((FaceSelect & FaceFlags.Bottom) != 0)
      {
        AddFace(-up, -right, Height, Length, Width, (int)FaceFlags.Bottom);
      }
    }

    private static void ViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Content2DBoxVisual3D)d).OnViewportChanged();
    }

    private void OnViewportChanged()
    {
      UpdateVisuals();
    }
    public static void VisualModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      ((Content2DBoxVisual3D)d).UpdateVisuals();
    }
  }
}