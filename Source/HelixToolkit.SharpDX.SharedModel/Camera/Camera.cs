/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Diagnostics;

#if NETFX_CORE
using  Windows.UI.Xaml;
using Vector3D = SharpDX.Vector3;
using Point3D = SharpDX.Vector3;
namespace HelixToolkit.UWP

#elif WINUI
using Microsoft.UI.Xaml;
using Vector3D = SharpDX.Vector3;
using Point3D = SharpDX.Vector3;
using HelixToolkit.SharpDX.Core.Cameras;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using System.Windows.Media.Media3D;
#if COREWPF
using HelixToolkit.SharpDX.Core.Cameras;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Cameras;
#endif
    /// <summary>
    /// 
    /// </summary>
    public interface ICameraModel
    {
        /// <summary>
        /// 
        /// </summary>
        bool CreateLeftHandSystem
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        Point3D Position
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        Vector3D LookDirection
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        Vector3D UpDirection
        {
            set; get;
        }
        /// <summary>
        /// 
        /// </summary>
        CameraCore CameraInternal
        {
            get;
        }
        void AnimateTo(
            Point3D newPosition,
            Vector3D newDirection,
            Vector3D newUpDirection,
            double animationTime);
        void StopAnimation();
        bool OnTimeStep();
    }
    /// <summary>
    /// Specifies what portion of the 3D scene is rendered by the Viewport3DX element.
    /// </summary>
    public abstract class Camera :
#if !NETFX_CORE && !WINUI
        System.Windows.Media.Animation.Animatable, ICameraModel
#else
        DependencyObject, ICameraModel
#endif
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public abstract Point3D Position
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>
        /// The look direction.
        /// </value>
        public abstract Vector3D LookDirection
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        public abstract Vector3D UpDirection
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets a value indicating whether [create left hand system].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [create left hand system]; otherwise, <c>false</c>.
        /// </value>
        public abstract bool CreateLeftHandSystem
        {
            set; get;
        }

        private CameraCore core;
        /// <summary>
        /// Gets the camera internal.
        /// </summary>
        /// <value>
        /// The camera internal.
        /// </value>
        public CameraCore CameraInternal
        {
            get
            {
                if (core == null)
                {
                    core = CreatePortableCameraCore();
                    OnCoreCreated(core);
                }
                return core;
            }
        }

        /// <summary>
        /// Creates the view matrix.
        /// </summary>
        /// <returns>A <see cref="Matrix" />.</returns>
        public Matrix CreateViewMatrix()
        {
            return CameraInternal.CreateViewMatrix();
        }

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>A <see cref="Matrix" />.</returns>
        public Matrix CreateProjectionMatrix(double aspectRatio)
        {
            return CameraInternal.CreateProjectionMatrix((float)aspectRatio);
        }

        private Vector3 targetPosition;
        private Vector3 targetLookDirection;
        private Vector3 targetUpDirection;
        private Vector3 oldPosition;
        private Vector3 oldLookDir;
        private Vector3 oldUpDir;
        private double aniTime = 0;
        private double accumTime = 0;
        private long prevTicks = 0;
        /// <summary>
        /// Creates the portable camera core.
        /// </summary>
        /// <returns></returns>
        protected abstract CameraCore CreatePortableCameraCore();
        /// <summary>
        /// Called when [core created].
        /// </summary>
        protected virtual void OnCoreCreated(CameraCore core)
        {
#if NETFX_CORE || WINUI
            core.LookDirection = this.LookDirection;
            core.Position = this.Position;
            core.UpDirection = this.UpDirection;
#else
            core.LookDirection = this.LookDirection.ToVector3();
            core.Position = this.Position.ToVector3();
            core.UpDirection = this.UpDirection.ToVector3();
#endif
            core.CreateLeftHandSystem = this.CreateLeftHandSystem;
        }
        /// <summary>
        /// Animates to.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        /// <param name="newDirection">The new direction.</param>
        /// <param name="newUpDirection">The new up direction.</param>
        /// <param name="animationTime">The animation time.</param>
        public void AnimateTo(
            Point3D newPosition,
            Vector3D newDirection,
            Vector3D newUpDirection,
            double animationTime)
        {
            if (animationTime == 0)
            {
                Position = newPosition;
                LookDirection = newDirection;
                UpDirection = newUpDirection;
                aniTime = 0;
            }
            else
            {
#if NETFX_CORE|| WINUI
                targetPosition = newPosition;
                targetLookDirection = newDirection;
                targetUpDirection = newUpDirection;
#else
                targetPosition = newPosition.ToVector3();
                targetLookDirection = newDirection.ToVector3();
                targetUpDirection = newUpDirection.ToVector3();
#endif
                oldPosition = CameraInternal.Position;
                oldLookDir = CameraInternal.LookDirection;
                oldUpDir = CameraInternal.UpDirection;
                aniTime = animationTime;
                accumTime = 1;
                prevTicks = Stopwatch.GetTimestamp();
                OnUpdateAnimation(0);
            }
        }
        /// <summary>
        /// Called when [time step] to update camera animation.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnTimeStep()
        {
            var ticks = Stopwatch.GetTimestamp();
            var ellapsed = (float)(ticks - prevTicks) / Stopwatch.Frequency * 1000;
            prevTicks = ticks;
            return OnUpdateAnimation(ellapsed);
        }

        protected virtual bool OnUpdateAnimation(float ellapsed)
        {
            if (aniTime == 0)
            {
                return false;
            }
            accumTime += ellapsed;
            if (accumTime > aniTime)
            {
#if NETFX_CORE|| WINUI
                Position = targetPosition;
                LookDirection = targetLookDirection;
                UpDirection = targetUpDirection;
#else
                Position = targetPosition.ToPoint3D();
                LookDirection = targetLookDirection.ToVector3D();
                UpDirection = targetUpDirection.ToVector3D();
#endif
                aniTime = 0;
                return false;
            }
            else
            {
                var l = (float)(accumTime / aniTime);
                var nextPos = Vector3.Lerp(oldPosition, targetPosition, l);
                var nextLook = Vector3.Lerp(oldLookDir, targetLookDirection, l);
                var nextUp = Vector3.Lerp(oldUpDir, targetUpDirection, l);
#if NETFX_CORE|| WINUI
                Position = nextPos;
                LookDirection = nextLook;
                UpDirection = nextUp;
#else
                Position = nextPos.ToPoint3D();
                LookDirection = nextLook.ToVector3D();
                UpDirection = nextUp.ToVector3D();
#endif
                return true;
            }
        }

        public void StopAnimation()
        {
            aniTime = 0;
        }

        public static implicit operator CameraCore(Camera camera)
        {
            return camera?.CameraInternal;
        }
    }
}
