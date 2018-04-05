/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

namespace HelixToolkit.UWP
{
    using Cameras;
    using global::SharpDX;
    using System.Diagnostics;
    using Windows.UI.Xaml;

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
        /// <summary>
        /// Gets the camera internal.
        /// </summary>
        /// <value>
        /// The camera internal.
        /// </value>
        internal CameraCore CameraInternal
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
        /// Animates to.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        /// <param name="newDirection">The new direction.</param>
        /// <param name="newUpDirection">The new up direction.</param>
        /// <param name="animationTime">The animation time.</param>
        public void AnimateTo(
            Vector3 newPosition,
            Vector3 newDirection,
            Vector3 newUpDirection,
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
                targetPosition = newPosition;
                targetLookDirection = newDirection;
                targetUpDirection = newUpDirection;
                oldPosition = Position;
                oldLookDir = LookDirection;
                oldUpDir = UpDirection;
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
                Position = targetPosition;
                LookDirection = targetLookDirection;
                UpDirection = targetUpDirection;
                aniTime = 0;
                return false;
            }
            else
            {
                var l = (float)(accumTime / aniTime);
                var next = Vector3.Lerp(oldPosition, targetPosition, l);
                Position = next;

                next = Vector3.Lerp(oldLookDir, targetLookDirection, l);
                LookDirection = next;

                next = Vector3.Lerp(oldUpDir, targetUpDirection, l);
                UpDirection = next;
                return true;
            }
        }

        public static implicit operator CameraCore(Camera camera)
        {
            return camera == null ? null : camera.CameraInternal;
        }
    }
}
