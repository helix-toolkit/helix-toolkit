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
        /// <summary>
        /// Identifies the <see cref="BackText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackTextProperty = DependencyProperty.Register(
            "BackText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("B", (d, e) =>
            {
                var b = (d as ViewCubeVisual3D).GetCubefaceColor(1);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(1, b, e.NewValue == null ? "" : (string)e.NewValue);
            }));

        /// <summary>
        /// Identifies the <see cref="BottomText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomTextProperty = DependencyProperty.Register(
            "BottomText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("D", (d, e) =>
            {
                var b = (d as ViewCubeVisual3D).GetCubefaceColor(5);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(5, b, e.NewValue == null ? "" : (string)e.NewValue);
            }));

        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Point3D), typeof(ViewCubeVisual3D), new UIPropertyMetadata(new Point3D(0, 0, 0)));

        /// <summary>
        /// Identifies the <see cref="FrontText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontTextProperty = DependencyProperty.Register(
            "FrontText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("F", (d, e) =>
            {
                var b = (d as ViewCubeVisual3D).GetCubefaceColor(0);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(0, b, e.NewValue == null ? "" : (string)e.NewValue);
            }));

        /// <summary>
        /// Identifies the <see cref="LeftText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftTextProperty = DependencyProperty.Register(
            "LeftText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("L", (d, e) =>
            {
                var b = (d as ViewCubeVisual3D).GetCubefaceColor(2);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(2, b, e.NewValue == null ? "" : (string)e.NewValue);
            }));

        /// <summary>
        /// Identifies the <see cref="LeftText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.Register(
            "IsEnabled", typeof(bool), typeof(ViewCubeVisual3D), new UIPropertyMetadata(true));

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
        /// Identifies the <see cref="RightText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightTextProperty = DependencyProperty.Register(
            "RightText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("R", (d, e) =>
            {
                var b = (d as ViewCubeVisual3D).GetCubefaceColor(3);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(3, b, e.NewValue == null ? "" : (string)e.NewValue);
            }));

        /// <summary>
        /// Identifies the <see cref="Size"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size", typeof(double), typeof(ViewCubeVisual3D), new UIPropertyMetadata(5.0));

        /// <summary>
        /// Identifies the <see cref="TopText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopTextProperty = DependencyProperty.Register(
            "TopText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("U", (d, e) =>
            {
                var b = (d as ViewCubeVisual3D).GetCubefaceColor(4);
                (d as ViewCubeVisual3D).UpdateCubefaceMaterial(4, b, e.NewValue == null ? "" : (string)e.NewValue);
            }));

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
            }));

        /// <summary>
        /// The normal vectors.
        /// </summary>
        private readonly Dictionary<object, Vector3D> faceNormals = new Dictionary<object, Vector3D>();

        /// <summary>
        /// The up vectors.
        /// </summary>
        private readonly Dictionary<object, Vector3D> faceUpVectors = new Dictionary<object, Vector3D>();

        private readonly IList<ModelUIElement3D> CubeFaceModels = new List<ModelUIElement3D>(6);
        private readonly IList<ModelUIElement3D> EdgeModels = new List<ModelUIElement3D>(4 * 3);
        private readonly IList<ModelUIElement3D> CornerModels = new List<ModelUIElement3D>(8);
        private static readonly Point3D[] xAligned = { new Point3D(0, -1, -1), new Point3D(0, 1, -1), new Point3D(0, -1, 1), new Point3D(0, 1, 1) }; //x
        private static readonly Point3D[] yAligned = { new Point3D(-1, 0, -1), new Point3D(1, 0, -1), new Point3D(-1, 0, 1), new Point3D(1, 0, 1) };//y
        private static readonly Point3D[] zAligned = { new Point3D(-1, -1, 0), new Point3D(-1, 1, 0), new Point3D(1, -1, 0), new Point3D(1, 1, 0) };//z

        private static readonly Point3D[] cornerPoints =   {
                new Point3D(-1,-1,-1 ), new Point3D(1, -1, -1), new Point3D(1, 1, -1), new Point3D(-1, 1, -1),
                new Point3D(-1,-1,1 ),new Point3D(1,-1,1 ),new Point3D(1,1,1 ),new Point3D(-1,1,1 )};

        private readonly PieSliceVisual3D circle = new PieSliceVisual3D();

        private readonly Brush CornerBrush = Brushes.Gold;
        private readonly Brush EdgeBrush = Brushes.Silver;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ViewCubeVisual3D" /> class.
        /// </summary>
        public ViewCubeVisual3D()
        {
            this.InitialModels();
        }

        /// <summary>
        /// Occurs when a face has been clicked on.
        /// </summary>
        public event EventHandler<ClickedEventArgs> Clicked;

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

        private void InitialModels()
        {
            for (int i = 0; i < 6; ++i)
            {
                var element = new ModelUIElement3D();
                CubeFaceModels.Add(element);
                Children.Add(CubeFaceModels[i]);
                element.MouseLeftButtonDown += this.FaceMouseLeftButtonDown;
            }
            this.Children.Add(circle);

            for (int i = 0; i < xAligned.Length + yAligned.Length + zAligned.Length; ++i)
            {
                var element = new ModelUIElement3D();
                EdgeModels.Add(element);
                element.MouseLeftButtonDown += FaceMouseLeftButtonDown;
                element.MouseEnter += EdggesMouseEnters;
                element.MouseLeave += EdgesMouseLeaves;
            }

            for (int i = 0; i < cornerPoints.Length; ++i)
            {
                var element = new ModelUIElement3D();
                CornerModels.Add(element);
                element.MouseLeftButtonDown += FaceMouseLeftButtonDown;
                element.MouseEnter += EdggesMouseEnters;
                element.MouseLeave += EdgesMouseLeaves;
            }

            UpdateVisuals();
        }

        /// <summary>
        /// Updates the visuals.
        /// </summary>
        private void UpdateVisuals()
        {
            var vecUp = this.ModelUpDirection;
            // create left vector 90° from up
            var vecLeft = new Vector3D(vecUp.Y, vecUp.Z, vecUp.X);

            var vecFront = Vector3D.CrossProduct(vecLeft, vecUp);

            faceNormals.Clear();
            faceUpVectors.Clear();
            AddCubeFace(CubeFaceModels[0], vecFront, vecUp, GetCubefaceColor(0), this.FrontText);
            AddCubeFace(CubeFaceModels[1], -vecFront, vecUp, GetCubefaceColor(1), this.BackText);
            AddCubeFace(CubeFaceModels[2], vecLeft, vecUp, GetCubefaceColor(2), this.LeftText);
            AddCubeFace(CubeFaceModels[3], -vecLeft, vecUp, GetCubefaceColor(3), this.RightText);
            AddCubeFace(CubeFaceModels[4], vecUp, vecLeft, GetCubefaceColor(4), this.TopText);
            AddCubeFace(CubeFaceModels[5], -vecUp, -vecLeft, GetCubefaceColor(5), this.BottomText);

            //var circle = new PieSliceVisual3D();
            circle.BeginEdit();
            circle.Center = (this.ModelUpDirection * (-this.Size / 2)).ToPoint3D();
            circle.Normal = this.ModelUpDirection;
            circle.UpVector = vecLeft; // rotate 90° so that it's at the bottom plane of the cube.
            circle.InnerRadius = this.Size;
            circle.OuterRadius = this.Size * 1.3;
            circle.StartAngle = 0;
            circle.EndAngle = 360;
            circle.Fill = Brushes.Gray;
            circle.EndEdit();

            AddCorners();
            AddEdges();
            EnableDisableEdgeClicks();
        }

        private Brush GetCubefaceColor(int index)
        {
            switch (index)
            {
                case 0:
                case 1:
                    return Brushes.Red;
                case 2:
                case 3:
                    if (ModelUpDirection.Z < 1)
                    {
                        return Brushes.Blue;
                    }
                    else
                    {
                        return Brushes.Green;
                    }
                case 4:
                case 5:
                    if (ModelUpDirection.Z < 1)
                    {
                        return Brushes.Green;
                    }
                    else
                    {
                        return Brushes.Blue;
                    }
                default:
                    return Brushes.White;
            }
        }

        private void EnableDisableEdgeClicks()
        {
            foreach (var item in EdgeModels)
            {
                Children.Remove(item);
            }
            foreach (var item in CornerModels)
            {
                Children.Remove(item);
            }
            if (EnableEdgeClicks)
            {
                foreach (var item in EdgeModels)
                {
                    (item.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(EdgeBrush);
                    Children.Add(item);
                }
                foreach (var item in CornerModels)
                {
                    (item.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(CornerBrush);
                    Children.Add(item);
                }
            }
        }

        private void UpdateCubefaceMaterial(int index, Brush b, string text)
        {
            if (CubeFaceModels.Count > 0 && index < CubeFaceModels.Count)
            {
                (CubeFaceModels[index].Model as GeometryModel3D).Material = CreateTextMaterial(b, text);
            }
            else
            {
                UpdateVisuals();
            }
        }

        private void AddEdges()
        {
            var halfSize = Size / 2;
            var sideLength = halfSize / 2;

            int counter = 0;
            foreach (var p in xAligned)
            {
                Point3D center = p.Multiply(halfSize);
                AddEdge(EdgeModels[counter++], center, 1.5 * halfSize, sideLength, sideLength, p.ToVector3D());
            }


            foreach (var p in yAligned)
            {
                Point3D center = p.Multiply(halfSize);
                AddEdge(EdgeModels[counter++], center, sideLength, 1.5 * halfSize, sideLength, p.ToVector3D());
            }


            foreach (var p in zAligned)
            {
                Point3D center = p.Multiply(halfSize);
                AddEdge(EdgeModels[counter++], center, sideLength, sideLength, 1.5 * halfSize, p.ToVector3D());
            }
        }

        private void AddEdge(ModelUIElement3D element, Point3D center, double x, double y, double z, Vector3D faceNormal)
        {
            var builder = new MeshBuilder(false, true);

            builder.AddBox(center, x, y, z);

            var geometry = builder.ToMesh();
            geometry.Freeze();

            var model = new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(EdgeBrush) };
            element.Model = model;

            faceNormals.Add(element, faceNormal);
            faceUpVectors.Add(element, ModelUpDirection);
        }

        private void AddCorners()
        {
            var a = Size / 2;
            var sideLength = a / 2;
            int counter = 0;
            foreach (var p in cornerPoints)
            {
                var builder = new MeshBuilder(false, true);

                Point3D center = p.Multiply(a);
                builder.AddBox(center, sideLength, sideLength, sideLength);
                var geometry = builder.ToMesh();
                geometry.Freeze();

                var model = new GeometryModel3D { Geometry = geometry, Material = MaterialHelper.CreateMaterial(CornerBrush) };
                var element = CornerModels[counter++];
                element.Model = model;
                faceNormals.Add(element, p.ToVector3D());
                faceUpVectors.Add(element, ModelUpDirection);
            }
        }

        private void EdgesMouseLeaves(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            (s.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(Colors.Silver);
        }

        private void EdggesMouseEnters(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            (s.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(Colors.Goldenrod);
        }

        private void CornersMouseLeave(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            (s.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(Colors.Gold);
        }

        private void CornersMouseEnters(object sender, MouseEventArgs e)
        {
            ModelUIElement3D s = sender as ModelUIElement3D;
            (s.Model as GeometryModel3D).Material = MaterialHelper.CreateMaterial(Colors.Goldenrod);
        }

        /// <summary>
        /// Adds a cube face.
        /// </summary>
        /// <param name="element">
        /// </param>
        /// <param name="normal">
        /// The normal.
        /// </param>
        /// <param name="up">
        /// The up vector.
        /// </param>
        /// <param name="b">
        /// The brush.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        private void AddCubeFace(ModelUIElement3D element, Vector3D normal, Vector3D up, Brush b, string text)
        {
            var material = CreateTextMaterial(b, text);

            double a = this.Size;

            var builder = new MeshBuilder(false, true);
            builder.AddCubeFace(this.Center, normal, up, a, a, a);
            var geometry = builder.ToMesh();
            geometry.Freeze();

            var model = new GeometryModel3D { Geometry = geometry, Material = material };

            element.Model = model;

            this.faceNormals.Add(element, normal);
            this.faceUpVectors.Add(element, up);
        }

        private Material CreateTextMaterial(Brush b, string text)
        {
            var grid = new Grid { Width = 20, Height = 20, Background = b };
            grid.Children.Add(
                new TextBlock
                {
                    Text = text,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 15,
                    Foreground = Brushes.White
                });
            grid.Arrange(new Rect(new Point(0, 0), new Size(20, 20)));

            var bmp = new RenderTargetBitmap((int)grid.Width, (int)grid.Height, 96, 96, PixelFormats.Default);
            bmp.Render(grid);
            bmp.Freeze();
            var m = MaterialHelper.CreateMaterial(new ImageBrush(bmp));
            m.Freeze();
            return m;
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

            var faceNormal = this.faceNormals[sender];
            var faceUp = this.faceUpVectors[sender];

            var lookDirection = -faceNormal;
            var upDirection = faceUp;
            lookDirection.Normalize();
            upDirection.Normalize();

            // Double-click reverses the look direction
            if (e.ClickCount == 2)
            {
                lookDirection *= -1;
                if (upDirection != this.ModelUpDirection)
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
    }
}
