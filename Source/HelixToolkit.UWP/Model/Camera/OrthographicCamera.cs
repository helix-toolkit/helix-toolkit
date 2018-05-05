/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using Windows.UI.Xaml;

namespace HelixToolkit.UWP
{
    using Cameras;
    using System;

    /// <summary>
    /// Represents an orthographic projection camera.
    /// </summary>
    public class OrthographicCamera : ProjectionCamera
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
            return new OrthographicCameraCore()
            {
                CreateLeftHandSystem = this.CreateLeftHandSystem,
                FarPlaneDistance = (float)this.FarPlaneDistance,
                LookDirection = this.LookDirection,
                NearPlaneDistance = (float)this.NearPlaneDistance,
                Position = this.Position,
                UpDirection = this.UpDirection,
                Width = (float)this.Width
            };
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
