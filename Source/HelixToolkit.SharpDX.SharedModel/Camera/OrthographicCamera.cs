/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using  Windows.UI.Xaml;
namespace HelixToolkit.UWP
#elif WINUI
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Cameras;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Cameras;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Cameras;
#endif
    public interface IOrthographicCameraModel : IProjectionCameraModel
    {
        double Width
        {
            set; get;
        }
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
            get
            {
                return (double)this.GetValue(WidthProperty);
            }
            set
            {
                this.SetValue(WidthProperty, value);
            }
        }

        private double oldWidth;
        private double targetWidth;
        private double accumTime;
        private double aniTime;

        public OrthographicCamera()
        {
            // default values for near-far must be different for ortho:
            NearPlaneDistance = 0.001;
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
                UpdateCameraPositionByWidth(newWidth);
                Width = newWidth;
            }
            else
            {
                oldWidth = Width;
                this.targetWidth = newWidth;
                accumTime = 1;
                aniTime = animationTime;
                OnUpdateAnimation(0);
            }
        }

        protected override bool OnUpdateAnimation(float ellapsed)
        {
            var res = base.OnUpdateAnimation(ellapsed);
            if (aniTime == 0)
            {
                return res;
            }
            accumTime += ellapsed;
            if (accumTime > aniTime)
            {
                UpdateCameraPositionByWidth(targetWidth);
                Width = targetWidth;
                aniTime = 0;
                return res;
            }
            else
            {
                var newWidth = oldWidth + (targetWidth - oldWidth) * (accumTime / aniTime);
                UpdateCameraPositionByWidth(newWidth);
                Width = newWidth;
                return true;
            }
        }

        private void UpdateCameraPositionByWidth(double newWidth)
        {
            var ratio = newWidth / Width;
#if !NETFX_CORE && !WINUI
            var dir = LookDirection.ToVector3();
            var target = Target.ToVector3();
#else
            var dir = LookDirection;
            var target = Target;
#endif
            var dist = dir.Length();
            var newDist = dist * ratio;
            dir.Normalize();
            var position = (target - dir * (float)newDist);
            var lookDir = dir * (float)newDist;
#if !NETFX_CORE && !WINUI
            Position = position.ToPoint3D();
            LookDirection = lookDir.ToVector3D();
#else
            Position = position;
            LookDirection = lookDir;
#endif
        }

#if !NETFX_CORE && !WINUI
        protected override Freezable CreateInstanceCore()
        {
            return new OrthographicCamera();
        }
#endif
    }
}
