using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using System.Windows;
using HelixToolkit.Geometry;

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
    /// Identifies the <see cref="IsTopBottomViewReverseOriented"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsTopBottomViewReverseOrientedProperty =
        DependencyProperty.Register("IsTopBottomViewReverseOriented", typeof(bool), typeof(ViewCubeVisual3D), new PropertyMetadata(false, VisualModelChanged));

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
    ///   Gets or sets a value indicating whether the top and bottom views reverse oriented.
    /// </summary>
    public bool IsTopBottomViewReverseOriented
    {
        get
        {
            return (bool)GetValue(IsTopBottomViewReverseOrientedProperty);
        }

        set
        {
            SetValue(IsTopBottomViewReverseOrientedProperty, value);
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
    private float overhang => 0.001f * (float)Size;
    #endregion Properties
    #region Fields
    /// <summary>
    /// The dictionary [object,(normal vector,up vector)].
    /// </summary>
    private readonly Dictionary<object, (Vector3D faceNormalVector, Vector3D faceUpVector)> faceNormalUpVectors = new();

    private readonly Dictionary<CubeFaces, ModelUIElement3D> cubeFaceModels = new(6);// 6 faces of cuve
    private readonly ModelUIElement3D[] cubeEdgeModels = new ModelUIElement3D[12];//3*4=12 edges of cube;
    private readonly ModelUIElement3D[] cubeCornerModels = new ModelUIElement3D[8];//8 corners of cube
    private readonly PieSliceVisual3D circle = new();


    private readonly SolidColorBrush cornerBrush = Brushes.Silver;
    private readonly SolidColorBrush edgeBrush = Brushes.Silver;
    private readonly SolidColorBrush highlightBrush = Brushes.CornflowerBlue;

    private Vector3D frontVector;
    private Vector3D leftVector;
    private Vector3D upVector;
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
            this.cubeFaceModels[cubeFace] = element;
            this.Children.Add(element);
        }
        // Init 12 edges of cube
        for (int i = 0; i < this.cubeEdgeModels.Length; ++i)
        {
            var element = new ModelUIElement3D();
            element.MouseLeftButtonDown += this.FaceMouseLeftButtonDown;
            element.MouseEnter += EdgesMouseEnters;
            element.MouseLeave += EdgesMouseLeaves;
            this.cubeEdgeModels[i] = element;
            this.Children.Add(element);
        }
        // Init 8 edges of cube
        for (int i = 0; i < this.cubeCornerModels.Length; ++i)
        {
            var element = new ModelUIElement3D();
            element.MouseLeftButtonDown += this.FaceMouseLeftButtonDown;
            element.MouseEnter += CornersMouseEnters;
            element.MouseLeave += CornersMouseLeaves;
            this.cubeCornerModels[i] = element;
            this.Children.Add(element);
        }

        this.Children.Add(circle);

        UpdateVisuals();
        EnableDisableEdgeClicks();
    }

    private void UpdateVisuals()
    {
        // 1.Update local unit vectors
        CalculateLocalUnitVectors();
        // 2.Update faces
        this.faceNormalUpVectors.Clear();
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

        Vector3D vecUp = this.ModelUpDirection;
        // create left vector 90° from up, using right-hand rule
        Vector3D vecLeft1 = new Vector3D(vecUp.Y, vecUp.Z, vecUp.X);
        if (vecLeft1 == vecUp)
        {
            vecLeft1 = new Vector3D(0, 0, 1);
        }
        Vector3D vecFront = Vector3D.CrossProduct(vecLeft1, vecUp);
        Vector3D vecLeft = Vector3D.CrossProduct(vecUp, vecFront);
        vecUp.Normalize();
        vecLeft.Normalize();
        vecFront.Normalize();
        this.frontVector = vecFront;
        this.leftVector = vecLeft;
        this.upVector = vecUp;

    }
    void CreateCubeFaces()
    {
        AddCubeFace(cubeFaceModels[CubeFaces.Front], frontVector, upVector, GetCubeFaceColor(CubeFaces.Front), FrontText);
        AddCubeFace(cubeFaceModels[CubeFaces.Back], -frontVector, upVector, GetCubeFaceColor(CubeFaces.Back), BackText);
        AddCubeFace(cubeFaceModels[CubeFaces.Left], leftVector, upVector, GetCubeFaceColor(CubeFaces.Left), LeftText);
        AddCubeFace(cubeFaceModels[CubeFaces.Right], -leftVector, upVector, GetCubeFaceColor(CubeFaces.Right), RightText);


        Vector3D tempFrontVector = frontVector;
        Vector3D tempLeftVector = leftVector;
        if (IsTopBottomViewReverseOriented)
        {
            tempFrontVector = -frontVector;
            tempLeftVector = -leftVector;
        }
        if (IsTopBottomViewOrientedToFrontBack)
        {
            AddCubeFace(cubeFaceModels[CubeFaces.Top], upVector, tempFrontVector, GetCubeFaceColor(CubeFaces.Top), TopText);
            AddCubeFace(cubeFaceModels[CubeFaces.Bottom], -upVector, -tempFrontVector, GetCubeFaceColor(CubeFaces.Bottom), BottomText);
        }
        else
        {
            AddCubeFace(cubeFaceModels[CubeFaces.Top], upVector, tempLeftVector, GetCubeFaceColor(CubeFaces.Top), TopText);
            AddCubeFace(cubeFaceModels[CubeFaces.Bottom], -upVector, -tempLeftVector, GetCubeFaceColor(CubeFaces.Bottom), BottomText);
        }
    }
    private Brush GetCubeFaceColor(CubeFaces cubeFace)
    {
        double max = Math.Max(Math.Max(this.ModelUpDirection.X, this.ModelUpDirection.Y), this.ModelUpDirection.Z);
        if (max == this.ModelUpDirection.Z)
        {
            return cubeFace switch
            {
                CubeFaces.Front or CubeFaces.Back => Brushes.Red,
                CubeFaces.Left or CubeFaces.Right => Brushes.Green,
                CubeFaces.Top or CubeFaces.Bottom => Brushes.Blue,
                _ => Brushes.White,
            };
        }
        else if (max == this.ModelUpDirection.Y)
        {
            return cubeFace switch
            {
                CubeFaces.Front or CubeFaces.Back => Brushes.Blue,
                CubeFaces.Left or CubeFaces.Right => Brushes.Red,
                CubeFaces.Top or CubeFaces.Bottom => Brushes.Green,
                _ => Brushes.White,
            };
        }
        else // if (max == this.ModelUpDirection.X)
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
        float halfSize = (float)this.Size / 2f;
        float sideWidthHeight = (float)this.Size / 5f;

        float moveDistance = (float)Math.Sqrt(2) * (sideWidthHeight / 2f - this.overhang);
        float squaredLength = (float)this.Size - 2f * (sideWidthHeight - this.overhang);

        Point3D p0 = this.Center - (this.leftVector + this.frontVector + this.upVector) * halfSize;
        Point3D p1 = p0 + this.frontVector * this.Size;
        Point3D p2 = p1 + this.leftVector * this.Size;
        Point3D p3 = p0 + this.leftVector * this.Size;

        Point3D p04 = p0 + this.upVector * halfSize;
        Point3D p15 = p1 + this.upVector * halfSize;
        Point3D p26 = p2 + this.upVector * halfSize;
        Point3D p37 = p3 + this.upVector * halfSize;

        Point3D p01 = p0 + this.frontVector * halfSize;
        Point3D p03 = p0 + this.leftVector * halfSize;
        Point3D p12 = p03 + this.frontVector * this.Size;
        Point3D p23 = p01 + this.leftVector * this.Size;

        Point3D p45 = p01 + this.upVector * this.Size;
        Point3D p56 = p12 + this.upVector * this.Size;
        Point3D p67 = p23 + this.upVector * this.Size;
        Point3D p47 = p03 + this.upVector * this.Size;

        Point3D[] xPoints = new Point3D[] { p01, p23, p67, p45 };
        Point3D[] yPoints = new Point3D[] { p56, p12, p03, p47 };
        Point3D[] zPoints = new Point3D[] { p04, p15, p26, p37 };
        int counter = 0;
        for (int i = 0; i < xPoints.Length; i++)
        {
            Point3D point = xPoints[i];
            Vector3D faceNormal = (Vector3D)point;
            faceNormal.Normalize();
            Point3D p = point - faceNormal * moveDistance;
            AddCubeEdge(this.cubeEdgeModels[counter], p, squaredLength, sideWidthHeight, sideWidthHeight, faceNormal);
            counter++;
        }
        for (int i = 0; i < yPoints.Length; i++)
        {
            Point3D point = yPoints[i];
            Vector3D faceNormal = (Vector3D)point;
            faceNormal.Normalize();
            Point3D p = point - faceNormal * moveDistance;
            AddCubeEdge(this.cubeEdgeModels[counter], p, sideWidthHeight, squaredLength, sideWidthHeight, faceNormal);
            counter++;
        }
        for (int i = 0; i < zPoints.Length; i++)
        {
            Point3D point = zPoints[i];
            Vector3D faceNormal = (Vector3D)point;
            faceNormal.Normalize();
            Point3D p = point - faceNormal * moveDistance;
            AddCubeEdge(this.cubeEdgeModels[counter], p, sideWidthHeight, sideWidthHeight, squaredLength, faceNormal);
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

        float halfSize = (float)this.Size / 2;
        float sideLength = (float)this.Size / 5;
        float moveDistance = (float)Math.Sqrt(3) * (sideLength / 2 - this.overhang);

        Point3D p0 = this.Center - (this.leftVector + this.frontVector + this.upVector) * halfSize;
        Point3D p1 = p0 + this.frontVector * this.Size;
        Point3D p2 = p1 + this.leftVector * this.Size;
        Point3D p3 = p0 + this.leftVector * this.Size;
        Point3D p4 = p0 + this.upVector * this.Size;
        Point3D p5 = p1 + this.upVector * this.Size;
        Point3D p6 = p2 + this.upVector * this.Size;
        Point3D p7 = p3 + this.upVector * this.Size;

        Point3D[] cornerPoints = new Point3D[] { p0, p1, p2, p3, p4, p5, p6, p7 };
        for (int i = 0; i < this.cubeCornerModels.Length; i++)
        {
            Point3D point = cornerPoints[i];
            Vector3D faceNormal = (Vector3D)point;
            faceNormal.Normalize();

            Point3D p = point - faceNormal * moveDistance;

            AddCubeCorner(this.cubeCornerModels[i], p, sideLength, faceNormal);
        }
    }
    void CreateCircle()
    {
        this.circle.BeginEdit();
        this.circle.Center = (Point3D)(this.upVector * (-this.Size / 2));
        this.circle.Normal = this.upVector;
        this.circle.UpVector = this.leftVector; // rotate 90° so that it's at the bottom plane of the cube.
        this.circle.InnerRadius = this.Size;
        this.circle.OuterRadius = this.Size * 1.3;
        this.circle.StartAngle = 0;
        this.circle.EndAngle = 360;
        this.circle.Fill = Brushes.Gray;
        this.circle.EndEdit();
    }

    /// <summary>
    /// Add a cube face.
    /// </summary>
    /// <param name="element"></param>
    /// <param name="faceNormal"></param>
    /// <param name="upVector"></param>
    /// <param name="background"></param>
    /// <param name="text"></param>
    private void AddCubeFace(ModelUIElement3D element, Vector3D faceNormal, Vector3D upVector, Brush background, string text)
    {
        Material material = CreateTextMaterial(background, text);
        float a = (float)this.Size;
        var builder = new MeshBuilder(false, true);
        builder.AddCubeFace(this.Center.ToVector3(), faceNormal.ToVector3(), upVector.ToVector3(), a, a, a);
        MeshGeometry3D geometry = builder.ToMesh().ToWndMeshGeometry3D();
        geometry.Freeze();
        var model = new GeometryModel3D { Geometry = geometry, Material = material };
        element.Model = model;

        faceNormalUpVectors.Add(element, (faceNormal, upVector));
    }
    private void AddCubeEdge(ModelUIElement3D element, Point3D center, float xLength, float yLength, float zLength, Vector3D faceNormal)
    {
        var builder = new MeshBuilder(false, true);
        builder.AddBox(center.ToVector3(), frontVector.ToVector3(), leftVector.ToVector3(), xLength, yLength, zLength);
        MeshGeometry3D geometry = builder.ToMesh().ToWndMeshGeometry3D();
        geometry.Freeze();
        var model = new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(edgeBrush) };
        element.Model = model;

        faceNormalUpVectors.Add(element, (faceNormal, upVector));

    }
    void AddCubeCorner(ModelUIElement3D element, Point3D center, float sideLength, Vector3D faceNormal)
    {
        var builder = new MeshBuilder(false, true);
        builder.AddBox(center.ToVector3(), frontVector.ToVector3(), leftVector.ToVector3(), sideLength, sideLength, sideLength);
        MeshGeometry3D geometry = builder.ToMesh().ToWndMeshGeometry3D();
        geometry.Freeze();
        var model = new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(cornerBrush) };
        element.Model = model;

        faceNormalUpVectors.Add(element, (faceNormal, upVector));

    }
    private static Material CreateTextMaterial(Brush? background, string text, Brush? foreground = null)
    {
        var grid = new Grid()
        {
            Width = 25,
            Height = 25,
            Background = background,
        };
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
        Material material = new DiffuseMaterial(new VisualBrush(grid));
        if (material.CanFreeze)
        {
            material.Freeze();
        }
        return material;
    }
    private void EnableDisableEdgeClicks()
    {
        if (this.EnableEdgeClicks)
        {
            for (int i = 0; i < this.cubeEdgeModels.Length; i++)
            {
                this.cubeEdgeModels[i].Visibility = Visibility.Visible;
            }
            for (int i = 0; i < this.cubeCornerModels.Length; i++)
            {
                this.cubeCornerModels[i].Visibility = Visibility.Visible;
            }
        }
        else
        {
            for (int i = 0; i < this.cubeEdgeModels.Length; i++)
            {
                this.cubeEdgeModels[i].Visibility = Visibility.Hidden;
            }
            for (int i = 0; i < cubeCornerModels.Length; i++)
            {
                this.cubeCornerModels[i].Visibility = Visibility.Hidden;
            }
        }
    }

    private void UpdateCubeFaceMaterial(CubeFaces cubeFace, Brush? background, string text)
    {
        if (this.cubeFaceModels.ContainsKey(cubeFace))
        {
            GeometryModel3D? gm = this.cubeFaceModels[cubeFace].Model as GeometryModel3D;

            if (gm is not null)
            {
                gm.Material = CreateTextMaterial(background, text);
            }
        }
    }

    private void FacesMouseEnters(object? sender, MouseEventArgs e)
    {
        ModelUIElement3D? s = sender as ModelUIElement3D;
        CubeFaces cubeFace = this.cubeFaceModels.FirstOrDefault(x => x.Value == s).Key;
        GeometryModel3D? gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = CreateTextMaterial(this.highlightBrush, GetCubeFaceName(cubeFace), GetCubeFaceColor(cubeFace));
        }
    }

    private void FacesMouseLeaves(object? sender, MouseEventArgs e)
    {
        ModelUIElement3D? s = sender as ModelUIElement3D;
        CubeFaces cubeFace = this.cubeFaceModels.FirstOrDefault(x => x.Value == s).Key;
        GeometryModel3D? gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = CreateTextMaterial(GetCubeFaceColor(cubeFace), GetCubeFaceName(cubeFace));
        }
    }

    private void EdgesMouseEnters(object? sender, MouseEventArgs e)
    {
        ModelUIElement3D? s = sender as ModelUIElement3D;
        GeometryModel3D? gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = MaterialHelper.CreateMaterial(this.highlightBrush);
        }
    }

    private void EdgesMouseLeaves(object? sender, MouseEventArgs e)
    {
        ModelUIElement3D? s = sender as ModelUIElement3D;
        GeometryModel3D? gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = MaterialHelper.CreateMaterial(this.edgeBrush);
        }
    }

    private void CornersMouseEnters(object? sender, MouseEventArgs e)
    {
        ModelUIElement3D? s = sender as ModelUIElement3D;
        GeometryModel3D? gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = MaterialHelper.CreateMaterial(this.highlightBrush);
        }
    }

    private void CornersMouseLeaves(object? sender, MouseEventArgs e)
    {
        ModelUIElement3D? s = sender as ModelUIElement3D;
        GeometryModel3D? gm = s?.Model as GeometryModel3D;

        if (gm is not null)
        {
            gm.Material = MaterialHelper.CreateMaterial(this.cornerBrush);
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

        Vector3D faceNormal = this.faceNormalUpVectors[sender].faceNormalVector;
        Vector3D faceUp = this.faceNormalUpVectors[sender].faceUpVector;

        Vector3D lookDirection = -faceNormal;
        Vector3D upDirection = faceUp;
        lookDirection.Normalize();
        upDirection.Normalize();

        // Double-click reverses the look direction
        if (e.ClickCount == 2)
        {
            lookDirection *= -1;
            if (upDirection != upVector)
            {
                upDirection *= -1;
            }
        }

        if (this.Viewport is not null)
        {
            ProjectionCamera? camera = this.Viewport.Camera as ProjectionCamera;
            if (camera is not null)
            {
                Point3D target = camera.Position + camera.LookDirection;
                double distance = camera.LookDirection.Length;
                lookDirection *= distance;
                Point3D newPosition = target - lookDirection;
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
}
