// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WiggleView3D.xaml.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A stereoscopic wiggle control.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media.Media3D;
    using System.Windows.Threading;

    /// <summary>
    /// A stereoscopic wiggle control.
    /// </summary>
    public partial class WiggleView3D : StereoControl
    {
        /// <summary>
        /// Identifies the <see cref="WiggleRate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WiggleRateProperty = DependencyProperty.Register(
            "WiggleRate", typeof(double), typeof(WiggleView3D), new UIPropertyMetadata(5.0, WiggleRateChanged));

        /// <summary>
        /// The timer.
        /// </summary>
        private readonly DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// The watch.
        /// </summary>
        private readonly Stopwatch watch = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref = "WiggleView3D" /> class.
        /// </summary>
        public WiggleView3D()
        {
            this.InitializeComponent();

            this.RightCamera = new PerspectiveCamera();
            this.BindViewports(this.View1, null, true, true);

            this.Loaded += this.ControlLoaded;
            this.Unloaded += this.ControlUnloaded;

            this.UpdateTimer();
            this.watch.Start();
            this.renderingEventListener = new RenderingEventListener(this.OnCompositionTargetRendering);
        }

        private readonly RenderingEventListener renderingEventListener;

        private void ControlUnloaded(object sender, RoutedEventArgs e)
        {
            RenderingEventManager.RemoveListener(renderingEventListener);
        }

        private void ControlLoaded(object sender, RoutedEventArgs e)
        {
            RenderingEventManager.AddListener(renderingEventListener);
        }

        /// <summary>
        /// Wiggles per second
        /// </summary>
        public double WiggleRate
        {
            get
            {
                return (double)this.GetValue(WiggleRateProperty);
            }

            set
            {
                this.SetValue(WiggleRateProperty, value);
            }
        }

        /// <summary>
        /// The wiggle rate changed.
        /// </summary>
        /// <param name="d">
        /// The d.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected static void WiggleRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((WiggleView3D)d).UpdateTimer();
        }

        /// <summary>
        /// The composition target_ rendering.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void OnCompositionTargetRendering(object sender, EventArgs e)
        {
            if (this.watch.ElapsedMilliseconds > 1000 / this.WiggleRate)
            {
                this.watch.Reset();
                this.watch.Start();
                this.Wiggle();
            }
        }

        /// <summary>
        /// The update timer.
        /// </summary>
        private void UpdateTimer()
        {
            this.timer.Interval = TimeSpan.FromSeconds(1.0 / this.WiggleRate);
        }

        /// <summary>
        /// Toggle between left and right camera.
        /// </summary>
        private void Wiggle()
        {
            if (this.View1.Camera == this.LeftCamera)
            {
                this.View1.Camera = this.RightCamera;
            }
            else
            {
                this.View1.Camera = this.LeftCamera;
            }
        }

    }
}