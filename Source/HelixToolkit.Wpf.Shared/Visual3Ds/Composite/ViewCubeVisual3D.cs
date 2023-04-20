// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewCubeVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a view cube.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Media.Media3D;

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
                var brush = (d as ViewCubeVisual3D).GetCubefaceColor(0);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(0, brush, e.NewValue == null ? "" : (string)e.NewValue);
            }));

        /// <summary>
        /// Identifies the <see cref="BackText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackTextProperty = DependencyProperty.Register(
            "BackText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("B", (d, e) =>
            {
                var brush = (d as ViewCubeVisual3D).GetCubefaceColor(1);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(1, brush, e.NewValue == null ? "" : (string)e.NewValue);
            }));
        /// <summary>
        /// Identifies the <see cref="LeftText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftTextProperty = DependencyProperty.Register(
            "LeftText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("L", (d, e) =>
            {
                var brush = (d as ViewCubeVisual3D).GetCubefaceColor(2);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(2, brush, e.NewValue == null ? "" : (string)e.NewValue);
            }));

        /// <summary>
        /// Identifies the <see cref="RightText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightTextProperty = DependencyProperty.Register(
            "RightText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("R", (d, e) =>
            {
                var brush = (d as ViewCubeVisual3D).GetCubefaceColor(3);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(3, brush, e.NewValue == null ? "" : (string)e.NewValue);
            }));

        /// <summary>
        /// Identifies the <see cref="TopText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopTextProperty = DependencyProperty.Register(
            "TopText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("U", (d, e) =>
            {
                var brush = (d as ViewCubeVisual3D).GetCubefaceColor(4);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(4, brush, e.NewValue == null ? "" : (string)e.NewValue);
            }));
        /// <summary>
        /// Identifies the <see cref="BottomText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomTextProperty = DependencyProperty.Register(
            "BottomText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("D", (d, e) =>
            {
                var brush = (d as ViewCubeVisual3D).GetCubefaceColor(5);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(5, brush, e.NewValue == null ? "" : (string)e.NewValue);
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
        /// Set or Get if view cube edge clickable.
        /// </summary>
        public bool EnableEdgeClicks
        {
            get { return (bool)GetValue(EnableEdgeClicksProperty); }
            set { SetValue(EnableEdgeClicksProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="EnableEdgeClicks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty EnableEdgeClicksProperty =
            DependencyProperty.Register("EnableEdgeClicks", typeof(bool), typeof(ViewCubeVisual3D), new PropertyMetadata(false, (d, e) =>
            {
                (d as ViewCubeVisual3D).EnableDisableEdgeClicks();
                // (d as ViewCubeVisual3D1).UpdateVisuals();
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
        private double Overhang => 0.001 * Size;
        #endregion Properties

        /// <summary>
        /// The dictionary [object,(normal vector,up vector)].
        /// </summary>
        private readonly Dictionary<object, NormalAndUpVector> _faceUpNormalVectors = new Dictionary<object, NormalAndUpVector>();
        // use value tuple instead of NormalAndUpVector when upgrade .net 4.7
        // private readonly Dictionary<object, (Vector3D faceNormal, Vector3D faceUpVector)> _faceUpNormalVectors = new Dictionary<object, (Vector3D, Vector3D)>();

        private readonly ModelUIElement3D[] _cubeFaceModels = new ModelUIElement3D[6];// 6 faces of cuve
        private readonly ModelUIElement3D[] _cubeEdgeModels = new ModelUIElement3D[12];//3*4=12 edges of cube;
        private readonly ModelUIElement3D[] _cubeCornerModels = new ModelUIElement3D[8];//8 corners of cube
        private readonly PieSliceVisual3D _circle = new PieSliceVisual3D();


        private readonly SolidColorBrush _cornerBrush = Brushes.Silver;
        private readonly SolidColorBrush _edgeBrush = Brushes.Silver;
        private readonly SolidColorBrush _highlightBrush = Brushes.CornflowerBlue;

        private Vector3D _frontVector;
        private Vector3D _leftVector;
        private Vector3D _upVector;

        /// <summary>
        /// Map number cube face to text name of cube.
        /// </summary>
        private readonly Dictionary<int, string> _mapFaceNames;

        #region Constructors
        /// <summary>
        ///   Initializes a new instance of the <see cref = "ViewCubeVisual3D" /> class.
        /// </summary>
        public ViewCubeVisual3D()
        {
            _mapFaceNames = new Dictionary<int, string>()
            {
                [0] = FrontText,// map with text property
                [1] = BackText,
                [2] = LeftText,
                [3] = RightText,
                [4] = TopText,
                [5] = BottomText,
            };
            this.InitialModels();
        }
        #endregion Constructors

        #region Events
        /// <summary>
        /// Occurs when a face has been clicked on.
        /// </summary>
        public event EventHandler<ClickedEventArgs> Clicked;

        #endregion Events


        /// <summary>
        /// Raises the Clicked event.
        /// </summary>
        /// <param name="lookDirection">The look direction.</param>
        /// <param name="upDirection">Up direction.</param>
        protected virtual void OnClicked(Vector3D lookDirection, Vector3D upDirection)
        {
            var clicked = this.Clicked;
            if (clicked != null)
            {
                clicked(this, new ClickedEventArgs { LookDirection = lookDirection, UpDirection = upDirection });
            }
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
            for (int i = 0; i < _cubeFaceModels.Length; ++i)
            {
                var element = new ModelUIElement3D();
                element.MouseLeftButtonDown += this.FaceMouseLeftButtonDown;
                element.MouseEnter += FacesMouseEnters;
                element.MouseLeave += FacesMouseLeaves;
                _cubeFaceModels[i] = element;
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
            AddCubeFace(_cubeFaceModels[0], _frontVector, _upVector, GetCubefaceColor(0), FrontText);
            AddCubeFace(_cubeFaceModels[1], -_frontVector, _upVector, GetCubefaceColor(1), BackText);
            AddCubeFace(_cubeFaceModels[2], _leftVector, _upVector, GetCubefaceColor(2), LeftText);
            AddCubeFace(_cubeFaceModels[3], -_leftVector, _upVector, GetCubefaceColor(3), RightText);
            if (IsTopBottomViewOrientedToFrontBack)
            {
                AddCubeFace(_cubeFaceModels[4], _upVector, _frontVector, GetCubefaceColor(4), TopText);
                AddCubeFace(_cubeFaceModels[5], -_upVector, -_frontVector, GetCubefaceColor(5), BottomText);
            }
            else
            {
                AddCubeFace(_cubeFaceModels[4], _upVector, _leftVector, GetCubefaceColor(4), TopText);
                AddCubeFace(_cubeFaceModels[5], -_upVector, -_leftVector, GetCubefaceColor(5), BottomText);
            }
        }
        private Brush GetCubefaceColor(int index)
        {
            double max = Math.Max(Math.Max(ModelUpDirection.X, ModelUpDirection.Y), ModelUpDirection.Z);
            if (max == ModelUpDirection.Z)
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Brushes.Red;
                    case 2:
                    case 3:
                        return Brushes.Green;
                    case 4:
                    case 5:
                        return Brushes.Blue;
                    default:
                        return Brushes.White;
                }
            }
            else if (max == ModelUpDirection.Y)
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Brushes.Blue;
                    case 2:
                    case 3:
                        return Brushes.Red;
                    case 4:
                    case 5:
                        return Brushes.Green;
                    default:
                        return Brushes.White;
                }
            }
            else // if (max == ModelUpDirection.X)
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Brushes.Green;
                    case 2:
                    case 3:
                        return Brushes.Blue;
                    case 4:
                    case 5:
                        return Brushes.Red;
                    default:
                        return Brushes.White;
                }
            }
           
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
             *     |    |    O +--- -|---------- Y Left
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
             *     |    |    O +--- -|---------- Y Left
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
            MeshBuilder builder = new MeshBuilder(false, true);
            builder.AddCubeFace(this.Center, normal, up, a, a, a);
            MeshGeometry3D geometry = builder.ToMesh();
            geometry.Freeze();
            GeometryModel3D model = new GeometryModel3D { Geometry = geometry, Material = material };
            element.Model = model;

            _faceUpNormalVectors.Add(element, new NormalAndUpVector(normal, up));
            // _faceUpNormalVectors.Add(element, (normal, up));  // use value tuple it instead of NormalAndUpVector when upgrade .net 4.7
        }
        private void AddCubeEdge(ModelUIElement3D element, Point3D center, double x, double y, double z, Vector3D faceNormal)
        {
            MeshBuilder builder = new MeshBuilder(false, true);
            builder.AddBox(center, _frontVector, _leftVector, x, y, z);
            MeshGeometry3D geometry = builder.ToMesh();
            geometry.Freeze();
            GeometryModel3D model = new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(_edgeBrush) };
            element.Model = model;

            _faceUpNormalVectors.Add(element, new NormalAndUpVector(faceNormal, _upVector));
            //_faceUpNormalVectors.Add(element, (faceNormal, _upVector)); // use value tuple it instead of NormalAndUpVector when upgrade .net 4.7

        }
        void AddCubeCorner(ModelUIElement3D element, Point3D center, double sideLength, Vector3D faceNormal)
        {

            var builder = new MeshBuilder(false, true);
            builder.AddBox(center, _frontVector, _leftVector, sideLength, sideLength, sideLength);
            var geometry = builder.ToMesh();
            geometry.Freeze();
            var model = new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(_cornerBrush) };
            element.Model = model;

            _faceUpNormalVectors.Add(element, new NormalAndUpVector(faceNormal, _upVector));
            //_faceUpNormalVectors.Add(element, (faceNormal, _upVector)); // use value tuple it instead of NormalAndUpVector when upgrade .net 4.7

        }
        private Material CreateTextMaterial(Brush background, string text, Brush foreground = null)
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

        private void UpdateCubefaceMaterial(int index, Brush background, string text)
        {
            if (_cubeFaceModels.Length > 0 && index < _cubeFaceModels.Length)
            {
                (_cubeFaceModels[index].Model as GeometryModel3D).Material = CreateTextMaterial(background, text);
                _mapFaceNames[index] = text;
            }
            else
            {
                UpdateVisuals();
            }
        }
        private void FacesMouseEnters(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            var faceIndex = Array.IndexOf(_cubeFaceModels, s);
            (s.Model as GeometryModel3D).Material = CreateTextMaterial(_highlightBrush, _mapFaceNames[faceIndex], GetCubefaceColor(faceIndex));
        }
        private void FacesMouseLeaves(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            var faceIndex = Array.IndexOf(_cubeFaceModels, s);
            (s.Model as GeometryModel3D).Material = CreateTextMaterial(GetCubefaceColor(faceIndex), _mapFaceNames[faceIndex]);
        }



        private void EdgesMouseEnters(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            (s.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(_highlightBrush);
        }
        private void EdgesMouseLeaves(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            (s.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(_edgeBrush);

        }

        private void CornersMouseEnters(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            (s.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(_highlightBrush);
        }
        private void CornersMouseLeaves(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            (s.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(_cornerBrush);
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
        private void FaceMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!this.IsEnabled)
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
        public class ClickedEventArgs : EventArgs
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
        struct NormalAndUpVector
        {
            public readonly Vector3D faceNormal;
            public readonly Vector3D faceUpVector;
            public NormalAndUpVector(Vector3D _faceNormal, Vector3D _faceUpVector)
            {
                faceNormal = _faceNormal;
                faceUpVector = _faceUpVector;
            }
        }
    }
}