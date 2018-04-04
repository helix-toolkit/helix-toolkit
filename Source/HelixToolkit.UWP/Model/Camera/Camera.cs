/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelixToolkit.UWP
{
    using global::SharpDX;
    using Cameras;
    using Windows.UI.Xaml;
    using Windows.UI.Composition;
    using Windows.UI.Xaml.Media.Media3D;
    using Windows.UI.Xaml.Media.Animation;
    using System.Diagnostics;

    /// <summary>
    /// Specifies what portion of the 3D scene is rendered by the Viewport3DX element.
    /// </summary>
    public abstract class Camera : DependencyObject
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public abstract Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>
        /// The look direction.
        /// </value>
        public abstract Vector3 LookDirection { get; set; }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        public abstract Vector3 UpDirection { get; set; }

        private CameraCore core;
        public CameraCore CameraInternal
        {
            get
            {
                if (core == null)
                {
                    core = CreatePortableCameraCore();
                }
                return core;
            }
        }

        /// <summary>
        /// Creates the view matrix.
        /// </summary>
        /// <returns>A <see cref="Matrix" />.</returns>
        public Matrix CreateViewMatrix() { return CameraInternal.CreateViewMatrix(); }

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>A <see cref="Matrix" />.</returns>
        public Matrix CreateProjectionMatrix(double aspectRatio) { return CameraInternal.CreateProjectionMatrix((float)aspectRatio); }

        private CompositeTransform3D PositionTransform = new CompositeTransform3D();
        private CompositeTransform3D LookDirectionTransform = new CompositeTransform3D();
        private CompositeTransform3D UpDirectionTransform = new CompositeTransform3D();

        public Camera()
        {
            PositionTransform.RegisterPropertyChangedCallback(CompositeTransform3D.TranslateXProperty, (d, e) => { Position = new Vector3((float)(double)d.GetValue(e), Position.Y, Position.Z); });
            PositionTransform.RegisterPropertyChangedCallback(CompositeTransform3D.TranslateYProperty, (d, e) => { Position = new Vector3(Position.X, (float)(double)d.GetValue(e), Position.Z); });
            PositionTransform.RegisterPropertyChangedCallback(CompositeTransform3D.TranslateZProperty, (d, e) => { Position = new Vector3(Position.X, Position.Y, (float)(double)d.GetValue(e)); });

            LookDirectionTransform.RegisterPropertyChangedCallback(CompositeTransform3D.TranslateXProperty, (d, e) => { LookDirection = new Vector3((float)(double)d.GetValue(e), Position.Y, Position.Z); });
            LookDirectionTransform.RegisterPropertyChangedCallback(CompositeTransform3D.TranslateYProperty, (d, e) => { LookDirection = new Vector3(Position.X, (float)(double)d.GetValue(e), Position.Z); });
            LookDirectionTransform.RegisterPropertyChangedCallback(CompositeTransform3D.TranslateZProperty, (d, e) => { LookDirection = new Vector3(Position.X, Position.Y, (float)(double)d.GetValue(e)); });

            UpDirectionTransform.RegisterPropertyChangedCallback(CompositeTransform3D.TranslateXProperty, (d, e) => { UpDirection = new Vector3((float)(double)d.GetValue(e), Position.Y, Position.Z); });
            UpDirectionTransform.RegisterPropertyChangedCallback(CompositeTransform3D.TranslateYProperty, (d, e) => { UpDirection = new Vector3(Position.X, (float)(double)d.GetValue(e), Position.Z); });
            UpDirectionTransform.RegisterPropertyChangedCallback(CompositeTransform3D.TranslateZProperty, (d, e) => { UpDirection = new Vector3(Position.X, Position.Y, (float)(double)d.GetValue(e)); });
        }

        protected abstract CameraCore CreatePortableCameraCore();

        public void AnimateTo(
            Vector3 newPosition,
            Vector3 newDirection,
            Vector3 newUpDirection,
            double animationTime)
        {
            var projectionCamera = this as ProjectionCamera;
            if (projectionCamera == null || animationTime == 0)
            {
                Position = newPosition;
                LookDirection = newDirection;
                UpDirection = newUpDirection;
                return;
            }

            var px = new DoubleAnimationUsingKeyFrames() { BeginTime = TimeSpan.FromMilliseconds(0), AutoReverse = false };
            px.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromMilliseconds(0), Value = Position.X });
            px.KeyFrames.Add(new EasingDoubleKeyFrame() { KeyTime = TimeSpan.FromMilliseconds(animationTime), Value = newPosition.X, EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut, Power = 1.5 } });

            //var py = new DoubleAnimationUsingKeyFrames()
            //{
            //    From = Position.Y,
            //    To = newPosition.Y,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)),
            //    FillBehavior = FillBehavior.Stop,
            //    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
            //};
            //var pz = new DoubleAnimationUsingKeyFrames()
            //{
            //    From = Position.Z,
            //    To = newPosition.Z,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)),
            //    FillBehavior = FillBehavior.Stop,
            //    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
            //};

            //var lx = new DoubleAnimationUsingKeyFrames()
            //{
            //    From = LookDirection.X,
            //    To = newDirection.X,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)),
            //    FillBehavior = FillBehavior.Stop,
            //    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
            //};
            //var ly = new DoubleAnimationUsingKeyFrames()
            //{
            //    From = LookDirection.Y,
            //    To = newDirection.Y,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)),
            //    FillBehavior = FillBehavior.Stop,
            //    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
            //};
            //var lz = new DoubleAnimationUsingKeyFrames()
            //{
            //    From = LookDirection.Z,
            //    To = newDirection.Z,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)),
            //    FillBehavior = FillBehavior.Stop,
            //    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
            //};

            //var ux = new DoubleAnimationUsingKeyFrames()
            //{
            //    From = UpDirection.X,
            //    To = newUpDirection.X,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)),
            //    FillBehavior = FillBehavior.Stop,
            //    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
            //};
            //var uy = new DoubleAnimationUsingKeyFrames()
            //{
            //    From = UpDirection.Y,
            //    To = newUpDirection.Y,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)),
            //    FillBehavior = FillBehavior.Stop,
            //    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
            //};
            //var uz = new DoubleAnimationUsingKeyFrames()
            //{
            //    From = UpDirection.Z,
            //    To = newUpDirection.Z,
            //    Duration = new Duration(TimeSpan.FromMilliseconds(animationTime)),
            //    FillBehavior = FillBehavior.Stop,
            //    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseOut }
            //};
            Storyboard sb = new Storyboard();
            sb.BeginTime = TimeSpan.FromMilliseconds(0);
            Storyboard.SetTarget(px, PositionTransform);
            //Storyboard.SetTarget(py, PositionTransform);
            //Storyboard.SetTarget(pz, PositionTransform);
            //Storyboard.SetTarget(lx, LookDirectionTransform);
            //Storyboard.SetTarget(ly, LookDirectionTransform);
            //Storyboard.SetTarget(lz, LookDirectionTransform);
            //Storyboard.SetTarget(ux, UpDirectionTransform);
            //Storyboard.SetTarget(uy, UpDirectionTransform);
            //Storyboard.SetTarget(uz, UpDirectionTransform);
            Storyboard.SetTargetProperty(px, "TranslateX");
            //Storyboard.SetTargetProperty(py, nameof(CompositeTransform3D.CenterY));
            //Storyboard.SetTargetProperty(pz, nameof(CompositeTransform3D.CenterZ));
            //Storyboard.SetTargetProperty(lx, nameof(CompositeTransform3D.CenterX));
            //Storyboard.SetTargetProperty(ly, nameof(CompositeTransform3D.CenterY));
            //Storyboard.SetTargetProperty(lz, nameof(CompositeTransform3D.CenterZ));
            //Storyboard.SetTargetProperty(ux, nameof(CompositeTransform3D.CenterX));
            //Storyboard.SetTargetProperty(uy, nameof(CompositeTransform3D.CenterY));
            //Storyboard.SetTargetProperty(uz, nameof(CompositeTransform3D.CenterZ));

            sb.Children.Add(px);
            //sb.Children.Add(py);
            //sb.Children.Add(pz);
            //sb.Children.Add(lx);
            //sb.Children.Add(ly);
            //sb.Children.Add(lz);
            //sb.Children.Add(ux);
            //sb.Children.Add(uy);
            //sb.Children.Add(uz);
            sb.Completed += Sb_Completed;
            sb.Begin();
            //if (animationTime > 0)
            //{
            //    var a1 = new Point3DAnimation(
            //        fromPosition, newPosition, new Duration(TimeSpan.FromMilliseconds(animationTime)))
            //    {
            //        AccelerationRatio = 0.3,
            //        DecelerationRatio = 0.5,
            //        FillBehavior = FillBehavior.Stop
            //    };

            //    a1.Completed += (s, a) => { camera.BeginAnimation(ProjectionCamera.PositionProperty, null); };
            //    camera.BeginAnimation(ProjectionCamera.PositionProperty, a1);

            //    var a2 = new Vector3DAnimation(
            //        fromDirection, newDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
            //    {
            //        AccelerationRatio = 0.3,
            //        DecelerationRatio = 0.5,
            //        FillBehavior = FillBehavior.Stop
            //    };
            //    a2.Completed += (s, a) => { camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, null); };
            //    camera.BeginAnimation(ProjectionCamera.LookDirectionProperty, a2);

            //    var a3 = new Vector3DAnimation(
            //        fromUpDirection, newUpDirection, new Duration(TimeSpan.FromMilliseconds(animationTime)))
            //    {
            //        AccelerationRatio = 0.3,
            //        DecelerationRatio = 0.5,
            //        FillBehavior = FillBehavior.Stop
            //    };
            //    a3.Completed += (s, a) => { camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, null); };
            //    camera.BeginAnimation(ProjectionCamera.UpDirectionProperty, a3);
            //}
        }

        private void Sb_Completed(object sender, object e)
        {
            Debug.WriteLine("Camera Animation Completed.");
        }

        public static implicit operator CameraCore(Camera camera)
        {
            return camera == null ? null : camera.CameraInternal;
        }
    }
}
