// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewCubeVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
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
            "BackText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("B", VisualModelChanged));

        /// <summary>
        /// Identifies the <see cref="BottomText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomTextProperty = DependencyProperty.Register(
            "BottomText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("D", VisualModelChanged));

        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty = DependencyProperty.Register(
            "Center", typeof(Point3D), typeof(ViewCubeVisual3D), new UIPropertyMetadata(new Point3D(0, 0, 0)));

        /// <summary>
        /// Identifies the <see cref="FrontText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FrontTextProperty = DependencyProperty.Register(
            "FrontText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("F", VisualModelChanged));

        /// <summary>
        /// Identifies the <see cref="LeftText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftTextProperty = DependencyProperty.Register(
            "LeftText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("L", VisualModelChanged));

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
            "RightText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("R", VisualModelChanged));

        /// <summary>
        /// Identifies the <see cref="Size"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
            "Size", typeof(double), typeof(ViewCubeVisual3D), new UIPropertyMetadata(5.0));

        /// <summary>
        /// Identifies the <see cref="TopText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TopTextProperty = DependencyProperty.Register(
            "TopText", typeof(string), typeof(ViewCubeVisual3D), new UIPropertyMetadata("U", VisualModelChanged));

        /// <summary>
        /// Identifies the <see cref="Viewport"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewportProperty = DependencyProperty.Register(
            "Viewport", typeof(Viewport3D), typeof(ViewCubeVisual3D), new PropertyMetadata(null));

        /// <summary>
        /// The normal vectors.
        /// </summary>
        private readonly Dictionary<object, Vector3D> faceNormals = new Dictionary<object, Vector3D>();

        /// <summary>
        /// The up vectors.
        /// </summary>
        private readonly Dictionary<object, Vector3D> faceUpVectors = new Dictionary<object, Vector3D>();

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ViewCubeVisual3D" /> class.
        /// </summary>
        public ViewCubeVisual3D()
        {
            this.UpdateVisuals();
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

        /// <summary>
        /// Updates the visuals.
        /// </summary>
        private void UpdateVisuals()
        {
            this.Children.Clear();
            var frontColor = Brushes.Red;
            var rightColor = Brushes.Green;
            var upColor = Brushes.Blue;
            var up = this.ModelUpDirection;
            var right = new Vector3D(0, 1, 0);
            if (up.Z < 1)
            {
                right = new Vector3D(0, 0, 1);
                rightColor = Brushes.Blue;
                upColor = Brushes.Green;
            }

            var front = Vector3D.CrossProduct(right, up);

            this.AddCubeFace(front, up, frontColor, this.FrontText);
            this.AddCubeFace(-front, up, frontColor, this.BackText);
            this.AddCubeFace(right, up, rightColor, this.RightText);
            this.AddCubeFace(-right, up, rightColor, this.LeftText);
            this.AddCubeFace(up, right, upColor, this.TopText);
            this.AddCubeFace(-up, -right, upColor, this.BottomText);

            var circle = new PieSliceVisual3D();
            circle.BeginEdit();
            circle.Center = (this.ModelUpDirection * (-this.Size / 2)).ToPoint3D();
            circle.Normal = this.ModelUpDirection;
            circle.UpVector = this.ModelUpDirection.Equals(new Vector3D(0, 0, 1))
                                  ? new Vector3D(0, 1, 0)
                                  : new Vector3D(0, 0, 1);
            circle.InnerRadius = this.Size;
            circle.OuterRadius = this.Size * 1.3;
            circle.StartAngle = 0;
            circle.EndAngle = 360;
            circle.Fill = Brushes.Gray;
            circle.EndEdit();
            this.Children.Add(circle);
        }

        /// <summary>
        /// Adds a cube face.
        /// </summary>
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
        private void AddCubeFace(Vector3D normal, Vector3D up, Brush b, string text)
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

            var material = MaterialHelper.CreateMaterial(new ImageBrush(bmp));

            double a = this.Size;

            var builder = new MeshBuilder(false, true);
            builder.AddCubeFace(this.Center, normal, up, a, a, a);
            var geometry = builder.ToMesh();
            geometry.Freeze();

            var model = new GeometryModel3D { Geometry = geometry, Material = material };
            var element = new ModelUIElement3D { Model = model };
            element.MouseLeftButtonDown += this.FaceMouseLeftButtonDown;

            this.faceNormals.Add(element, normal);
            this.faceUpVectors.Add(element, up);

            this.Children.Add(element);
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