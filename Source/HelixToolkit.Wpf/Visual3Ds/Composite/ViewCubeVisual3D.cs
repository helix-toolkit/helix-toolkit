using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;

namespace HelixToolkit.Wpf;

/// <summary>
/// A visual element that shows a view cube.
/// </summary>
public class ViewCubeVisual3D : ModelVisual3D
{
    #region Dependency Properties
    /// <summary>
    /// Identifies the <see cref="FrontText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty FrontTextProperty = DependencyProperty.Register(
        "FrontText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("F", (d, e) =>
        {
            var brush = (d as ViewCubeVisual3D)?.GetCubeFaceColor(CubeFaces.Front);
            (d as ViewCubeVisual3D)?.UpdateCubeFaceMaterial(CubeFaces.Front, brush, e.NewValue == null ? string.Empty : (string)e.NewValue);
        }));

    /// <summary>
    /// Identifies the <see cref="BackText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BackTextProperty = DependencyProperty.Register(
        "BackText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("B", (d, e) =>
        {
            var brush = (d as ViewCubeVisual3D)?.GetCubeFaceColor(CubeFaces.Back);
            (d as ViewCubeVisual3D)?.UpdateCubeFaceMaterial(CubeFaces.Back, brush, e.NewValue == null ? string.Empty : (string)e.NewValue);
        }));
    /// <summary>
    /// Identifies the <see cref="LeftText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty LeftTextProperty = DependencyProperty.Register(
        "LeftText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("L", (d, e) =>
        {
            var brush = (d as ViewCubeVisual3D)?.GetCubeFaceColor(CubeFaces.Left);
            (d as ViewCubeVisual3D)?.UpdateCubeFaceMaterial(CubeFaces.Left, brush, e.NewValue == null ? string.Empty : (string)e.NewValue);
        }));

    /// <summary>
    /// Identifies the <see cref="RightText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty RightTextProperty = DependencyProperty.Register(
        "RightText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("R", (d, e) =>
        {
            var brush = (d as ViewCubeVisual3D)?.GetCubeFaceColor(CubeFaces.Right);
            (d as ViewCubeVisual3D)?.UpdateCubeFaceMaterial(CubeFaces.Right, brush, e.NewValue == null ? string.Empty : (string)e.NewValue);
        }));

    /// <summary>
    /// Identifies the <see cref="TopText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty TopTextProperty = DependencyProperty.Register(
        "TopText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("U", (d, e) =>
        {
            var brush = (d as ViewCubeVisual3D)?.GetCubeFaceColor(CubeFaces.Top);
            (d as ViewCubeVisual3D)?.UpdateCubeFaceMaterial(CubeFaces.Top, brush, e.NewValue == null ? string.Empty : (string)e.NewValue);
        }));
    /// <summary>
    /// Identifies the <see cref="BottomText"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty BottomTextProperty = DependencyProperty.Register(
        "BottomText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("D", (d, e) =>
        {
            var brush = (d as ViewCubeVisual3D)?.GetCubeFaceColor(CubeFaces.Bottom);
            (d as ViewCubeVisual3D)?.UpdateCubeFaceMaterial(CubeFaces.Bottom, brush, e.NewValue == null ? string.Empty : (string)e.NewValue);
        }));

    /// <summary>
    /// Identifies the <see cref="Center"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
        "Center", typeof(Point3D), typeof(ViewCubeVisual3D), new UIPropertyMetadata(new Point3D(0, 0, 0)));
    /// <summary>
    /// Identifies the <see cref="Size"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
        "Size", typeof(double), typeof(ViewCubeVisual3D), new UIPropertyMetadata(5.0, VisualModelChanged));

    /// <summary>
    /// Identifies the <see cref="IsEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
        "IsEnabled", typeof(bool), typeof(ViewCubeVisual3D), new UIPropertyMetadata(true));

    /// <summary>
    /// Identifies the <see cref="IsTopBottomViewOrientedToFrontBack"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsTopBottomViewOrientedToFrontBackProperty =
        DependencyProperty.Register("IsTopBottomViewOrientedToFrontBack", typeof(bool), typeof(ViewCubeVisual3D), new PropertyMetadata(false, VisualModelChanged));

    /// <summary>
    /// Identifies the <see cref="ModelUpDirection"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ModelUpDirectionProperty =
        DependencyProperty.Register(
            "ModelUpDirection",
            typeof(Vector3D),
            typeof(ViewCubeVisual3D),
            new UIPropertyMetadata(new Vector3D(0, 0, 1), VisualModelChanged));

    /// <summary>
    /// Identifies the <see cref="Viewport"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(
        "Viewport", typeof(Viewport3D), typeof(ViewCubeVisual3D), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="EnableEdgeClicks"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty EnableEdgeClicksProperty =
        DependencyProperty.Register("EnableEdgeClicks", typeof(bool), typeof(ViewCubeVisual3D), new PropertyMetadata(false, (d, e) =>
        {
            (d as ViewCubeVisual3D)?.EnableDisableEdgeClicks();
            // (d as ViewCubeVisual3D1)?.UpdateVisuals();
        }));
    #endregion Dependency Properties
    #region Properties
    /// <summary>
    ///   Gets or sets the back text.
    /// </summary>
    /// <value>The back text.</value>
    public string BackText
    {
        get
        {
            return (string)this.GetValue(BackTextProperty);
        }

        set
        {
            this.SetValue(BackTextProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets the bottom text.
    /// </summary>
    /// <value>The bottom text.</value>
    public string BottomText
    {
        get
        {
            return (string)this.GetValue(BottomTextProperty);
        }

        set
        {
            this.SetValue(BottomTextProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets the center.
    /// </summary>
    /// <value>The center.</value>
    public Point3D Center
    {
        get
        {
            return (Point3D)this.GetValue(CenterProperty);
        }

        set
        {
            this.SetValue(CenterProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets the front text.
    /// </summary>
    /// <value>The front text.</value>
    public string FrontText
    {
        get
        {
            return (string)this.GetValue(FrontTextProperty);
        }

        set
        {
            this.SetValue(FrontTextProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets the left text.
    /// </summary>
    /// <value>The left text.</value>
    public string LeftText
    {
        get
        {
            return (string)this.GetValue(LeftTextProperty);
        }

        set
        {
            this.SetValue(LeftTextProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets the model up direction.
    /// </summary>
    /// <value>The model up direction.</value>
    public Vector3D ModelUpDirection
    {
        get
        {
            return (Vector3D)this.GetValue(ModelUpDirectionProperty);
        }

        set
        {
            this.SetValue(ModelUpDirectionProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets the right text.
    /// </summary>
    /// <value>The right text.</value>
    public string RightText
    {
        get
        {
            return (string)this.GetValue(RightTextProperty);
        }

        set
        {
            this.SetValue(RightTextProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets the size.
    /// </summary>
    /// <value>The size.</value>
    public double Size
    {
        get
        {
            return (double)this.GetValue(SizeProperty);
        }

        set
        {
            this.SetValue(SizeProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets the top text.
    /// </summary>
    /// <value>The top text.</value>
    public string TopText
    {
        get
        {
            return (string)this.GetValue(TopTextProperty);
        }

        set
        {
            this.SetValue(TopTextProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the view cube is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get
        {
            return (bool)this.GetValue(IsEnabledProperty);
        }

        set
        {
            this.SetValue(IsEnabledProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets a value indicating whether the top and bottom views are oriented to front and back.
    /// </summary>
    public bool IsTopBottomViewOrientedToFrontBack
    {
        get
        {
            return (bool)GetValue(IsTopBottomViewOrientedToFrontBackProperty);
        }

        set
        {
            SetValue(IsTopBottomViewOrientedToFrontBackProperty, value);
        }
    }

    /// <summary>
    ///   Gets or sets the viewport that is being controlled by the view cube.
    /// </summary>
    /// <value>The viewport.</value>
    [Browsable(false)]
    public Viewport3D Viewport
    {
        get
        {
            return (Viewport3D)this.GetValue(ViewportProperty);
        }

        set
        {
            this.SetValue(ViewportProperty, value);
        }
    }

    /// <summary>
    /// Set or Get if view cube edge clickable.
    /// </summary>
    public bool EnableEdgeClicks
    {
        get { return (bool)GetValue(EnableEdgeClicksProperty); }
        set { SetValue(EnableEdgeClicksProperty, value); }
    }
    private double Overhang => 0.001 * Size;
    #endregion Properties
    #region Fields
    /// <summary>
    /// The dictionary [object,(normal vector,up vector)].
    /// </summary>
    private readonly Dictionary<object, NormalAndUpVector> _faceUpNormalVectors = new();
    // use value tuple instead of NormalAndUpVector when upgrade .net 4.7
    // private readonly Dictionary<object, (Vector3D faceNormal, Vector3D faceUpVector)> _faceUpNormalVectors = new();

    private readonly Dictionary<CubeFaces, ModelUIElement3D> _cubeFaceModels = new(6);// 6 faces of cuve
    private readonly ModelUIElement3D[] _cubeEdgeModels = new ModelUIElement3D[12];//3*4=12 edges of cube;
    private readonly ModelUIElement3D[] _cubeCornerModels = new ModelUIElement3D[8];//8 corners of cube
    private readonly PieSliceVisual3D _circle = new();


    private readonly SolidColorBrush _cornerBrush = Brushes.Silver;
    private readonly SolidColorBrush _edgeBrush = Brushes.Silver;
    private readonly SolidColorBrush _highlightBrush = Brushes.CornflowerBlue;

    private Vector3D _frontVector;
    private Vector3D _leftVector;
    private Vector3D _upVector;
    #endregion Fields
    #region Constructors
    /// <summary>
    ///   Initializes a new instance of the <see cref = "ViewCubeVisual3D" /> class.
    /// </summary>
    public ViewCubeVisual3D()
    {
        this.InitialModels();
    }
    #endregion Constructors

    #region Events
    /// <summary>
    /// Occurs when a face has been clicked on.
    /// </summary>
    public event EventHandler<ClickedEventArgs>? Clicked;

    #endregion Events


    /// <summary>
    /// Raises the Clicked event.
    /// </summary>
    /// <param name="lookDirection">The look direction.</param>
    /// <param name="upDirection">Up direction.</param>
    protected virtual void OnClicked(Vector3D lookDirection, Vector3D upDirection)
    {
        this.Clicked?.Invoke(this, new ClickedEventArgs { LookDirection = lookDirection, UpDirection = upDirection });
    }

    /// <summary>
    /// The VisualModel property changed.
    /// </summary>
    /// <param name="d">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private static void VisualModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ViewCubeVisual3D)d).UpdateVisuals();
    }
    void InitialModels()
    {
        // Init Element
        // Init 6 faces of cube
        Array cubefaces = Enum.GetValues(typeof(CubeFaces));
        foreach (CubeFaces cubeFace in cubefaces)
        {
            var element = new ModelUIElement3D();
            element.MouseLeftButtonDown += this.FaceMouseLeftButtonDown;
            element.MouseEnter += FacesMouseEnters;
            element.MouseLeave += FacesMouseLeaves;
            _cubeFaceModels[cubeFace] = element;
            Children.Add(element);
        }
        // Init 12 edges of cube
        for (int i = 0; i < _cubeEdgeModels.Length; ++i)
        {
            var element = new ModelUIElement3D();
            element.MouseLeftButtonDown += this.FaceMouseLeftButtonDown;
            element.MouseEnter += EdgesMouseEnters;
            element.MouseLeave += EdgesMouseLeaves;
            _cubeEdgeModels[i] = element;
            Children.Add(element);
        }
        // Init 8 edges of cube
        for (int i = 0; i < _cubeCornerModels.Length; ++i)
        {
            var element = new ModelUIElement3D();
            element.MouseLeftButtonDown += this.FaceMouseLeftButtonDown;
            element.MouseEnter += CornersMouseEnters;
            element.MouseLeave += CornersMouseLeaves;
            _cubeCornerModels[i] = element;
            Children.Add(element);
        }

        this.Children.Add(_circle);

        UpdateVisuals();
        EnableDisableEdgeClicks();
    }

    private void UpdateVisuals()
    {
        // 1.Update local unit vectors
        CalculateLocalUnitVectors();
        // 2.Update faces
        _faceUpNormalVectors.Clear();
        CreateCubeFaces();
        CreateCubeEdges();
        CreateCubeCorners();
        CreateCircle();

    }

    /// <summary>
    /// Calculate vector normalize unit of up, front, left vector
    /// </summary>
    void CalculateLocalUnitVectors()
    {
        /* Coordinate system
         * 
         *     | Z (Up)
         *     |
         *     |
         *    O|_________ Y (Left)
         *    /
         *   /
         *  / X (Front)
         *
         */

        var vecUp = this.ModelUpDirection;
        // create left vector 90° from up, using right-hand rule
        var vecLeft1 = new Vector3D(vecUp.Y, vecUp.Z, vecUp.X);
        if (vecLeft1 == vecUp)
        {
            vecLeft1 = new Vector3D(0, 0, 1);
        }
        var vecFront = Vector3D.CrossProduct(vecLeft1, vecUp);
        var vecLeft = Vector3D.CrossProduct(vecUp, vecFront);
        vecUp.Normalize();
        vecLeft.Normalize();
        vecFront.Normalize();
        this._frontVector = vecFront;
        this._leftVector = vecLeft;
        this._upVector = vecUp;

    }
    void CreateCubeFaces()
    {
        AddCubeFace(_cubeFaceModels[CubeFaces.Front], _frontVector, _upVector, GetCubeFaceColor(CubeFaces.Front), FrontText);
        AddCubeFace(_cubeFaceModels[CubeFaces.Back], -_frontVector, _upVector, GetCubeFaceColor(CubeFaces.Back), BackText);
        AddCubeFace(_cubeFaceModels[CubeFaces.Left], _leftVector, _upVector, GetCubeFaceColor(CubeFaces.Left), LeftText);
        AddCubeFace(_cubeFaceModels[CubeFaces.Right], -_leftVector, _upVector, GetCubeFaceColor(CubeFaces.Right), RightText);
        if (IsTopBottomViewOrientedToFrontBack)
        {
            AddCubeFace(_cubeFaceModels[CubeFaces.Top], _upVector, _frontVector, GetCubeFaceColor(CubeFaces.Top), TopText);
            AddCubeFace(_cubeFaceModels[CubeFaces.Bottom], -_upVector, -_frontVector, GetCubeFaceColor(CubeFaces.Bottom), BottomText);
        }
        else
        {
            AddCubeFace(_cubeFaceModels[CubeFaces.Top], _upVector, _leftVector, GetCubeFaceColor(CubeFaces.Top), TopText);
            AddCubeFace(_cubeFaceModels[CubeFaces.Bottom], -_upVector, -_leftVector, GetCubeFaceColor(CubeFaces.Bottom), BottomText);
        }
    }
    private Brush GetCubeFaceColor(CubeFaces cubeFace)
    {
        double max = Math.Max(Math.Max(ModelUpDirection.X, ModelUpDirection.Y), ModelUpDirection.Z);
        if (max == ModelUpDirection.Z)
        {
            return cubeFace switch
            {
                CubeFaces.Front or CubeFaces.Back => Brushes.Red,
                CubeFaces.Left or CubeFaces.Right => Brushes.Green,
                CubeFaces.Top or CubeFaces.Bottom => Brushes.Blue,
                _ => Brushes.White,
            };
        }
        else if (max == ModelUpDirection.Y)
        {
            return cubeFace switch
            {
                CubeFaces.Front or CubeFaces.Back => Brushes.Blue,
                CubeFaces.Left or CubeFaces.Right => Brushes.Red,
                CubeFaces.Top or CubeFaces.Bottom => Brushes.Green,
                _ => Brushes.White,
            };
        }
        else // if (max == ModelUpDirection.X)
        {
            return cubeFace switch
            {
                CubeFaces.Front or CubeFaces.Back => Brushes.Green,
                CubeFaces.Left or CubeFaces.Right => Brushes.Blue,
                CubeFaces.Top or CubeFaces.Bottom => Brushes.Red,
                _ => Brushes.White,
            };
        }

    }
    string GetCubeFaceName(CubeFaces cubeFace)
    {
        return cubeFace switch
        {
            CubeFaces.Front => FrontText,
            CubeFaces.Back => BackText,
            CubeFaces.Left => LeftText,
            CubeFaces.Right => RightText,
            CubeFaces.Top => TopText,
            CubeFaces.Bottom => BottomText,
            _ => string.Empty,
        };
    }
    void CreateCubeEdges()
    {
        /*
         *               Z | Up   
         *                 |
         *         p4______|__p47______p7
         *         /|      |         /|
         *        / |      |        / |
         *    p45/  |      |    p67/  |
         *      /  p04     |      /   |
         *  p5 |--------p56------|p6 p37
         *     |    |      |     |    |
         *     |    |    O +-----|----------- Y Left
         *     |  p0|_____/_p03__|____|p3
         *    p15  /     /      p26   /
         *     |  /     /        |   /
         *     | /p01  /         |  /p23
         *     |/     /          | /
         *  p1 |_____/___p12_____|/p2
         *          /
         *       X / Front
         *
         */

        if (this.Size == 0) return;
        double halfSize = Size / 2;
        double sideWidthHeight = Size / 5;

        var moveDistance = Math.Sqrt(2) * (sideWidthHeight / 2 - Overhang);
        double squaredLength = Size - 2 * (sideWidthHeight - Overhang);


        var p0 = Center - (_leftVector + _frontVector + _upVector) * halfSize;
        var p1 = p0 + _frontVector * Size;
        var p2 = p1 + _leftVector * Size;
        var p3 = p0 + _leftVector * Size;

        var p04 = p0 + _upVector * halfSize;
        var p15 = p1 + _upVector * halfSize;
        var p26 = p2 + _upVector * halfSize;
        var p37 = p3 + _upVector * halfSize;

        var p01 = p0 + _frontVector * halfSize;
        var p03 = p0 + _leftVector * halfSize;
        var p12 = p03 + _frontVector * Size;
        var p23 = p01 + _leftVector * Size;

        var p45 = p01 + _upVector * Size;
        var p56 = p12 + _upVector * Size;
        var p67 = p23 + _upVector * Size;
        var p47 = p03 + _upVector * Size;

        var xPoints = new Point3D[] { p01, p23, p67, p45 };
        var yPoints = new Point3D[] { p56, p12, p03, p47 };
        var zPoints = new Point3D[] { p04, p15, p26, p37 };
        int counter = 0;
        for (int i = 0; i < xPoints.Length; i++)
        {
            Point3D point = xPoints[i];
            Vector3D faceNormal = (Vector3D)point;
            faceNormal.Normalize();
            Point3D p = point - faceNormal * moveDistance;
            AddCubeEdge(_cubeEdgeModels[counter], p, squaredLength, sideWidthHeight, sideWidthHeight, faceNormal);
            counter++;
        }
        for (int i = 0; i < yPoints.Length; i++)
        {
            Point3D point = yPoints[i];
            Vector3D faceNormal = (Vector3D)point;
            faceNormal.Normalize();
            Point3D p = point - faceNormal * moveDistance;
            AddCubeEdge(_cubeEdgeModels[counter], p, sideWidthHeight, squaredLength, sideWidthHeight, faceNormal);
            counter++;
        }
        for (int i = 0; i < zPoints.Length; i++)
        {
            Point3D point = zPoints[i];
            Vector3D faceNormal = (Vector3D)point;
            faceNormal.Normalize();
            Point3D p = point - faceNormal * moveDistance;
            AddCubeEdge(_cubeEdgeModels[counter], p, sideWidthHeight, sideWidthHeight, squaredLength, faceNormal);
            counter++;
        }
    }

    void CreateCubeCorners()
    {
        /*
         *               Z | Up   
         *                 |
         *         p4______|__________p7
         *         /|      |         /|
         *        / |      |        / |
         *       /  |      |       /  |
         *      /   |      |      /   |
         *  p5 |-----------------|p6  |
         *     |    |      |     |    |
         *     |    |    O +-----|----------- Y Left
         *     |  p0|_____/______|____|p3
         *     |   /     /       |    /
         *     |  /     /        |   /
         *     | /     /         |  /
         *     |/     /          | /
         *  p1 |_____/___________|/p2
         *          /
         *       X / Front
         *
         */

        if (this.Size == 0) return;

        double halfSize = Size / 2;
        double sideLength = Size / 5;
        var moveDistance = Math.Sqrt(3) * (sideLength / 2 - Overhang);

        var p0 = Center - (_leftVector + _frontVector + _upVector) * halfSize;
        var p1 = p0 + _frontVector * Size;
        var p2 = p1 + _leftVector * Size;
        var p3 = p0 + _leftVector * Size;
        var p4 = p0 + _upVector * Size;
        var p5 = p1 + _upVector * Size;
        var p6 = p2 + _upVector * Size;
        var p7 = p3 + _upVector * Size;

        var cornerPoints = new Point3D[] { p0, p1, p2, p3, p4, p5, p6, p7 };
        for (int i = 0; i < _cubeCornerModels.Length; i++)
        {
            Point3D point = cornerPoints[i];
            Vector3D faceNormal = (Vector3D)point;
            faceNormal.Normalize();

            Point3D p = point - faceNormal * moveDistance;

            AddCubeCorner(_cubeCornerModels[i], p, sideLength, faceNormal);
        }
    }
    void CreateCircle()
    {
        _circle.BeginEdit();
        _circle.Center = (Point3D)(this._upVector * (-this.Size / 2));
        _circle.Normal = this._upVector;
        _circle.UpVector = this._leftVector; // rotate 90° so that it's at the bottom plane of the cube.
        _circle.InnerRadius = this.Size;
        _circle.OuterRadius = this.Size * 1.3;
        _circle.StartAngle = 0;
        _circle.EndAngle = 360;
        _circle.Fill = Brushes.Gray;
        _circle.EndEdit();
    }

    /// <summary>
    /// Add a cube face.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="normal"></param>
    /// <param name="up"></param>
    /// <param name="background"></param>
    /// <param name="text"></param>
    private void AddCubeFace(ModelUIElement3D element, Vector3D normal, Vector3D up, Brush background, string text)
    {
        Material material = CreateTextMaterial(background, text);
        double a = this.Size;
        var builder = new MeshBuilder(false, true);
        builder.AddCubeFace(this.Center.ToVector(), normal.ToVector(), up.ToVector(), (float)a, (float)a, (float)a);
        MeshGeometry3D geometry = builder.ToMesh().ToMeshGeometry3D();
        geometry.Freeze();
        var model = new GeometryModel3D { Geometry = geometry, Material = material };
        element.Model = model;

        _faceUpNormalVectors.Add(element, new NormalAndUpVector(normal, up));
        // _faceUpNormalVectors.Add(element, (normal, up));  // use value tuple it instead of NormalAndUpVector when upgrade .net 4.7
    }
    private void AddCubeEdge(ModelUIElement3D element, Point3D center, double x, double y, double z, Vector3D faceNormal)
    {
        var builder = new MeshBuilder(false, true);
        builder.AddBox(center.ToVector(), _frontVector.ToVector(), _leftVector.ToVector(), (float)x, (float)y, (float)z);
        var geometry = builder.ToMesh().ToMeshGeometry3D();
        geometry.Freeze();
        var model = new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(_edgeBrush) };
        element.Model = model;

        _faceUpNormalVectors.Add(element, new NormalAndUpVector(faceNormal, _upVector));
        //_faceUpNormalVectors.Add(element, (faceNormal, _upVector)); // use value tuple it instead of NormalAndUpVector when upgrade .net 4.7

    }
    void AddCubeCorner(ModelUIElement3D element, Point3D center, double sideLength, Vector3D faceNormal)
    {

        var builder = new MeshBuilder(false, true);
        builder.AddBox(center.ToVector(), _frontVector.ToVector(), _leftVector.ToVector(), (float)sideLength, (float)sideLength, (float)sideLength);
        var geometry = builder.ToMesh().ToMeshGeometry3D();
        geometry.Freeze();
        var model = new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(_cornerBrush) };
        element.Model = model;

        _faceUpNormalVectors.Add(element, new NormalAndUpVector(faceNormal, _upVector));
        //_faceUpNormalVectors.Add(element, (faceNormal, _upVector)); // use value tuple it instead of NormalAndUpVector when upgrade .net 4.7

    }
    private Material CreateTextMaterial(Brush? background, string text, Brush? foreground = null)
    {
        var grid = new Grid { Width = 25, Height = 25, Background = background };
        if (foreground is null) foreground = Brushes.White;
        grid.Children.Add(
            new TextBlock
            {
                Text = text,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Foreground = foreground,
            });
        grid.Arrange(new Rect(new Point(0, 0), new Size(25, 25)));

        var bmp = new RenderTargetBitmap((int)grid.Width, (int)grid.Height, 96, 96, PixelFormats.Default);
        bmp.Render(grid);
        bmp.Freeze();
        var material = MaterialHelper.CreateMaterial(new ImageBrush(bmp));
        material.Freeze();
        return material;
    }
    private void EnableDisableEdgeClicks()
    {
        if (EnableEdgeClicks)
        {
            for (int i = 0; i < _cubeEdgeModels.Length; i++)
            {
                _cubeEdgeModels[i].Visibility = Visibility.Visible;
            }
            for (int i = 0; i < _cubeCornerModels.Length; i++)
            {
                _cubeCornerModels[i].Visibility = Visibility.Visible;
            }
        }
        else
        {
            for (int i = 0; i < _cubeEdgeModels.Length; i++)
            {
                _cubeEdgeModels[i].Visibility = Visibility.Hidden;
            }
            for (int i = 0; i < _cubeCornerModels.Length; i++)
            {
                _cubeCornerModels[i].Visibility = Visibility.Hidden;
            }
        }
    }

    private void UpdateCubeFaceMaterial(CubeFaces cubeFace, Brush? background, string text)
    {
        if (_cubeFaceModels.ContainsKey(cubeFace))
        {
            var gm = _cubeFaceModels[cubeFace].Model as GeometryModel3D;

            if (gm is not null)
            {
                gm.Material = CreateTextMaterial(background, text);
            }
        }
    }

    private void FacesMouseEnters(object? sender, MouseEventArgs e)
    {
        var s = sender as ModelUIElement3D;
        CubeFaces cubeFace = _cubeFaceModels.FirstOrDefault(x => x.Value == s).Key;
        var gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = CreateTextMaterial(_highlightBrush, GetCubeFaceName(cubeFace), GetCubeFaceColor(cubeFace));
        }
    }

    private void FacesMouseLeaves(object? sender, MouseEventArgs e)
    {
        var s = sender as ModelUIElement3D;
        CubeFaces cubeFace = _cubeFaceModels.FirstOrDefault(x => x.Value == s).Key;
        var gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = CreateTextMaterial(GetCubeFaceColor(cubeFace), GetCubeFaceName(cubeFace));
        }
    }

    private void EdgesMouseEnters(object? sender, MouseEventArgs e)
    {
        var s = sender as ModelUIElement3D;
        var gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = MaterialHelper.CreateMaterial(_highlightBrush);
        }
    }

    private void EdgesMouseLeaves(object? sender, MouseEventArgs e)
    {
        var s = sender as ModelUIElement3D;
        var gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = MaterialHelper.CreateMaterial(_edgeBrush);
        }
    }

    private void CornersMouseEnters(object? sender, MouseEventArgs e)
    {
        var s = sender as ModelUIElement3D;
        var gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = MaterialHelper.CreateMaterial(_highlightBrush);
        }
    }

    private void CornersMouseLeaves(object? sender, MouseEventArgs e)
    {
        var s = sender as ModelUIElement3D;
        var gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = MaterialHelper.CreateMaterial(_cornerBrush);
        }
    }

    /// <summary>
    /// Handles left clicks on the view cube.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void FaceMouseLeftButtonDown(object? sender, MouseButtonEventArgs e)
    {
        if (!this.IsEnabled || sender is null)
        {
            return;
        }

        var faceNormal = _faceUpNormalVectors[sender].faceNormal;
        var faceUp = _faceUpNormalVectors[sender].faceUpVector;

        var lookDirection = -faceNormal;
        var upDirection = faceUp;
        lookDirection.Normalize();
        upDirection.Normalize();

        // Double-click reverses the look direction
        if (e.ClickCount == 2)
        {
            lookDirection *= -1;
            if (upDirection != _upVector)
            {
                upDirection *= -1;
            }
        }

        if (this.Viewport != null)
        {
            var camera = this.Viewport.Camera as ProjectionCamera;
            if (camera != null)
            {
                var target = camera.Position + camera.LookDirection;
                double distance = camera.LookDirection.Length;
                lookDirection *= distance;
                var newPosition = target - lookDirection;
                CameraHelper.AnimateTo(camera, newPosition, lookDirection, upDirection, 500);
            }
        }

        e.Handled = true;
        this.OnClicked(lookDirection, upDirection);

    }
    /// <summary>
    /// Provides event data for the Clicked event.
    /// </summary>
    public sealed class ClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>The look direction.</value>
        public Vector3D LookDirection { get; set; }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>Up direction.</value>
        public Vector3D UpDirection { get; set; }
    }

    enum CubeFaces
    {
        Front = 0,
        Back = 1,
        Left = 2,
        Right = 3,
        Top = 4,
        Bottom = 5,
    }
    private readonly struct NormalAndUpVector
    {
        public readonly Vector3D faceNormal;
        public readonly Vector3D faceUpVector;
        public NormalAndUpVector(Vector3D faceNormal, Vector3D faceUpVector)
        {
            this.faceNormal = faceNormal;
            this.faceUpVector = faceUpVector;
        }
    }
}
