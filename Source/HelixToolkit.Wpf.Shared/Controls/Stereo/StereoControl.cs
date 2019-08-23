// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StereoControl.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Base class for controls that use stereo cameras
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.IO;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// Base class for controls that use stereo cameras
    /// </summary>
    [ContentProperty("Content")]
    public class StereoControl : ContentControl
    {
        // todo: keyboard shortcut 'x' to change cross/parallel viewing
        /// <summary>
        /// Identifies the <see cref="Camera"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
            "Camera", typeof(PerspectiveCamera), typeof(StereoControl), new UIPropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CameraRotationMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CameraRotationModeProperty =
            DependencyProperty.Register(
                "CameraRotationMode",
                typeof(CameraRotationMode),
                typeof(StereoControl),
                new UIPropertyMetadata(CameraRotationMode.Turntable));

        /// <summary>
        /// Identifies the <see cref="CopyDirectionVector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CopyDirectionVectorProperty =
            DependencyProperty.Register(
                "CopyDirectionVector",
                typeof(bool),
                typeof(StereoControl),
                new UIPropertyMetadata(true, StereoViewChanged));

        /// <summary>
        /// Identifies the <see cref="CopyUpVector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CopyUpVectorProperty = DependencyProperty.Register(
            "CopyUpVector", typeof(bool), typeof(StereoControl), new UIPropertyMetadata(false, StereoViewChanged));

        /// <summary>
        /// Identifies the <see cref="CrossViewing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CrossViewingProperty = DependencyProperty.Register(
            "CrossViewing", typeof(bool), typeof(StereoControl), new UIPropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="StereoBase"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StereoBaseProperty = DependencyProperty.Register(
            "StereoBase", typeof(double), typeof(StereoControl), new UIPropertyMetadata(0.12, StereoViewChanged));

        /// <summary>
        /// Initializes static members of the <see cref="StereoControl"/> class.
        /// </summary>
        static StereoControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(StereoControl), new FrameworkPropertyMetadata(typeof(StereoControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StereoControl"/> class.
        /// </summary>
        public StereoControl()
        {
            this.Camera = CameraHelper.CreateDefaultCamera();
            this.Camera.Changed += this.CameraChanged;
            this.Children = new ObservableCollection<Visual3D>();
        }

        /*        void StereoControl_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.X)
            {
                CrossViewing = !CrossViewing;
            }
        }
        */
        /// <summary>
        /// Gets or sets the camera.
        /// </summary>
        /// <value>The camera.</value>
        public PerspectiveCamera Camera
        {
            get
            {
                return (PerspectiveCamera)this.GetValue(CameraProperty);
            }

            set
            {
                this.SetValue(CameraProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the camera rotation mode.
        /// </summary>
        /// <value>The camera rotation mode.</value>
        public CameraRotationMode CameraRotationMode
        {
            get
            {
                return (CameraRotationMode)this.GetValue(CameraRotationModeProperty);
            }

            set
            {
                this.SetValue(CameraRotationModeProperty, value);
            }
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ObservableCollection<Visual3D> Children { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether [copy direction vector].
        /// </summary>
        /// <value><c>true</c> if [copy direction vector]; otherwise, <c>false</c>.</value>
        public bool CopyDirectionVector
        {
            get
            {
                return (bool)this.GetValue(CopyDirectionVectorProperty);
            }

            set
            {
                this.SetValue(CopyDirectionVectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [copy up vector].
        /// </summary>
        /// <value><c>true</c> if [copy up vector]; otherwise, <c>false</c>.</value>
        public bool CopyUpVector
        {
            get
            {
                return (bool)this.GetValue(CopyUpVectorProperty);
            }

            set
            {
                this.SetValue(CopyUpVectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cameras are set up for cross viewing.
        /// </summary>
        /// <value><c>true</c> if [cross viewing]; otherwise, <c>false</c>.</value>
        public bool CrossViewing
        {
            get
            {
                return (bool)this.GetValue(CrossViewingProperty);
            }

            set
            {
                this.SetValue(CrossViewingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the left camera.
        /// </summary>
        /// <value>The left camera.</value>
        public PerspectiveCamera LeftCamera { get; set; }

        /// <summary>
        /// Gets or sets the left viewport.
        /// </summary>
        /// <value>The left viewport.</value>
        public Viewport3D LeftViewport { get; set; }

        /// <summary>
        /// Gets or sets the right camera.
        /// </summary>
        /// <value>The right camera.</value>
        public PerspectiveCamera RightCamera { get; set; }

        /// <summary>
        /// Gets or sets the right viewport.
        /// </summary>
        /// <value>The right viewport.</value>
        public Viewport3D RightViewport { get; set; }

        /// <summary>
        /// Gets or sets the stereo base.
        /// </summary>
        /// <value>The stereo base.</value>
        public double StereoBase
        {
            get
            {
                return (double)this.GetValue(StereoBaseProperty);
            }

            set
            {
                this.SetValue(StereoBaseProperty, value);
            }
        }

        /// <summary>
        /// Binds the viewports.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        public void BindViewports(Viewport3D left, Viewport3D right)
        {
            this.BindViewports(left, right, true, true);
        }

        /// <summary>
        /// Binds the viewports.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <param name="createLights">
        /// if set to <c>true</c> [create lights].
        /// </param>
        /// <param name="createCamera">
        /// if set to <c>true</c> [create camera].
        /// </param>
        public void BindViewports(Viewport3D left, Viewport3D right, bool createLights, bool createCamera)
        {
            this.LeftViewport = left;
            this.RightViewport = right;

            this.Children.CollectionChanged += this.ChildrenCollectionChanged;

            if (createLights)
            {
                this.Children.Add(new DefaultLights());
            }

            if (createCamera)
            {
                if (this.LeftViewport.Camera == null)
                {
                    this.LeftViewport.Camera = CameraHelper.CreateDefaultCamera();
                }
                else
                {
                    CameraHelper.Reset(this.LeftViewport.Camera as PerspectiveCamera);
                }

                if (this.RightViewport != null && this.RightViewport.Camera == null)
                {
                    this.RightViewport.Camera = new PerspectiveCamera();
                }
            }

            this.LeftCamera = this.LeftViewport.Camera as PerspectiveCamera;
            if (this.RightViewport != null)
            {
                this.RightCamera = this.RightViewport.Camera as PerspectiveCamera;
            }

            this.UpdateCameras();
        }

        /// <summary>
        /// Clears the children collection.
        /// </summary>
        public void Clear()
        {
            this.Children.Clear();
            this.SynchronizeStereoModel();
        }

        /// <summary>
        /// Exports the views to kerkythea.
        /// </summary>
        /// <param name="leftFileName">
        /// Name of the left file.
        /// </param>
        /// <param name="rightFileName">
        /// Name of the right file.
        /// </param>
        public void ExportKerkythea(string leftFileName, string rightFileName)
        {
            var scb = this.Background as SolidColorBrush;

            var leftExporter = new KerkytheaExporter();
            if (scb != null)
            {
                leftExporter.BackgroundColor = scb.Color;
            }

            leftExporter.Reflections = true;
            leftExporter.Shadows = true;
            leftExporter.SoftShadows = true;
            leftExporter.Width = (int)this.LeftViewport.ActualWidth;
            leftExporter.Height = (int)this.LeftViewport.ActualHeight;
            using (var stream = File.Create(leftFileName))
            {
                leftExporter.Export(this.LeftViewport, stream);
            }

            var rightExporter = new KerkytheaExporter();
            if (scb != null)
            {
                rightExporter.BackgroundColor = scb.Color;
            }

            rightExporter.Reflections = true;
            rightExporter.Shadows = true;
            rightExporter.SoftShadows = true;
            rightExporter.Width = (int)this.RightViewport.ActualWidth;
            rightExporter.Height = (int)this.RightViewport.ActualHeight;
            using (var stream = File.Create(rightFileName))
            {
                rightExporter.Export(this.RightViewport, stream);
            }
        }

        /// <summary>
        /// Synchronizes the stereo model.
        /// </summary>
        public void SynchronizeStereoModel()
        {
            this.LeftViewport.Children.Clear();
            if (this.RightViewport != null)
            {
                this.RightViewport.Children.Clear();
            }

            foreach (var v in this.Children)
            {
                this.LeftViewport.Children.Add(v);
                if (this.RightViewport != null)
                {
                    Visual3D clone = StereoHelper.CreateClone(v);
                    if (clone != null)
                    {
                        this.RightViewport.Children.Add(clone);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the cameras.
        /// </summary>
        public void UpdateCameras()
        {
            StereoHelper.UpdateStereoCameras(
                this.Camera,
                this.LeftCamera,
                this.RightCamera,
                this.StereoBase,
                this.CrossViewing,
                this.CopyUpVector,
                this.CopyDirectionVector);
        }

        /// <summary>
        /// The stereo view changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void StereoViewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var v = (StereoControl)d;
            v.UpdateCameras();
        }

        /// <summary>
        /// Handle the camera changed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void CameraChanged(object sender, EventArgs e)
        {
            this.UpdateCameras();
        }

        /// <summary>
        /// Handle changes in the children collection.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void ChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // todo: update left and right collections here
        }

    }
}