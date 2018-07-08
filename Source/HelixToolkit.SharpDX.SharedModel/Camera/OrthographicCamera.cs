/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Cameras;
    public interface IOrthographicCameraModel : IProjectionCameraModel
    {
        double Width { set; get; }
        void AnimateWidth(double newWidth, double animationTime);
    }
    /// <summary>
    /// Represents an orthographic projection camera.
    /// </summary>
    public class OrthographicCamera : ProjectionCamera, IOrthographicCameraModel
    {
        /// <summary>
        /// The width property
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.Register(
            "Width", typeof(double), typeof(OrthographicCamera), new PropertyMetadata(10.0, (d, e) =>
            {
                ((d as Camera).CameraInternal as OrthographicCameraCore).Width = (float)(double)e.NewValue;
            }));

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>
        /// The width.
        /// </value>
        public double Width
        {
            get { return (double)this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }

        private double oldWidth;
        private double newWidth;
        private double accumTime;
        private double aniTime;

        public OrthographicCamera()
        {
            // default values for near-far must be different for ortho:
            NearPlaneDistance = -10.0;
            FarPlaneDistance = 100.0;
        }

        protected override CameraCore CreatePortableCameraCore()
        {
            return new OrthographicCameraCore();
        }

        protected override void OnCoreCreated(CameraCore core)
        {
            base.OnCoreCreated(core);
            (core as OrthographicCameraCore).FarPlaneDistance = (float)this.FarPlaneDistance;
            (core as OrthographicCameraCore).NearPlaneDistance = (float)this.NearPlaneDistance;
            (core as OrthographicCameraCore).Width = (float)this.Width;
        }

        public void AnimateWidth(double newWidth, double animationTime)
        {
            if (animationTime == 0)
            {
                Width = newWidth;
                animationTime = 0;
            }
            else
            {
                oldWidth = Width;
                this.newWidth = newWidth;
                accumTime = 1;
                aniTime = animationTime;
                OnUpdateAnimation(0);
            }
        }

        protected override bool OnUpdateAnimation(float ellapsed)
        {
            bool res = base.OnUpdateAnimation(ellapsed);
            if (aniTime == 0)
            {
                return res;
            }
            accumTime += ellapsed;
            if(accumTime > aniTime)
            {
                Width = newWidth;
                aniTime = 0;
                return res;
            }
            else
            {
                Width = oldWidth + (newWidth - oldWidth) / (accumTime / aniTime);
                return true;
            }
        }
    }
}
