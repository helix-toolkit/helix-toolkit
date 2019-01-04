using System;
using SharpDX;


namespace HelixToolkit.SharpDX.Core.Controls
{
    using Cameras;

    public sealed class ZoomHandler : MouseGestureHandler
    {
        /// <summary>
        /// The change field of view.
        /// </summary>
        private readonly bool changeFieldOfView;

        /// <summary>
        /// The zoom point.
        /// </summary>
        private Vector2 zoomPoint;

        /// <summary>
        /// The 3D zoom point.
        /// </summary>
        private Vector3 zoomPoint3D;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZoomHandler"/> class.
        /// </summary>
        /// <param name="viewport">
        /// The viewport.
        /// </param>
        /// <param name="changeFieldOfView">
        /// if set to <c>true</c> [change field of view].
        /// </param>
        public ZoomHandler(CameraController viewport, bool changeFieldOfView = false)
            : base(viewport)
        {
            this.changeFieldOfView = changeFieldOfView;
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="Vector2"/> instance containing the event data.</param>
        public override void Delta(Vector2 e)
        {
            var delta = e - LastPoint;
            LastPoint = e;
            Zoom(delta.Y * 0.01f, zoomPoint3D);
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="Vector2"/> instance containing the event data.</param>
        protected override void Started(Vector2 e)
        {
            base.Started(e);
            zoomPoint = new Vector2(Controller.Width / 2f, Controller.Height / 2f);
            zoomPoint3D = Camera.Target;

            if (Controller.ZoomAroundMouseDownPoint && MouseDownNearestPoint3D != null)
            {
                zoomPoint = MouseDownPoint;
                zoomPoint3D = MouseDownNearestPoint3D.Value;
            }
        }

        /// <summary>
        /// Zooms the view.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        public void Zoom(float delta)
        {
            Zoom(delta, Camera.Target);
        }

        /// <summary>
        /// Zooms the view around the specified point.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        /// <param name="zoomAround">
        /// The zoom around.
        /// </param>
        /// <param name="isTouch"></param>
        /// <param name="stopOther">Stop other manipulation</param>
        public void Zoom(float delta, Vector3 zoomAround, bool isTouch = false, bool stopOther = true)
        {
            if (!Controller.IsZoomEnabled)
            {
                return;
            }
            if (stopOther)
            {
                Controller.StopSpin();
                Controller.StopPanning();
            }
            if (Camera is PerspectiveCameraCore)
            {
                if (!isTouch)
                {
                    if (delta < -0.5f)
                    {
                        delta = -0.5f;
                    }

                    delta *= ZoomSensitivity;
                }


                if (CameraMode == CameraMode.FixedPosition || changeFieldOfView)
                {
                    Controller.ZoomByChangingFieldOfView(delta);
                }
                else
                {
                    switch (CameraMode)
                    {
                        case CameraMode.Inspect:
                            ChangeCameraDistance(ref delta, zoomAround);
                            break;
                        case CameraMode.WalkAround:
                            Camera.Position -= Camera.LookDirection * (float)delta;
                            break;
                    }
                }

                return;
            }
            else if (Camera is OrthographicCameraCore)
            {
                ZoomByChangingCameraWidth(delta, zoomAround);
            }
        }

        /// <summary>
        /// Changes the camera position by the specified vector.
        /// </summary>
        /// <param name="delta">The translation vector in camera space (z in look direction, y in up direction, and x perpendicular to the two others)</param>
        /// <param name="stopOther">Stop other manipulation</param>
        public void MoveCameraPosition(Vector3 delta, bool stopOther = true)
        {
            if (stopOther)
            {
                Controller.StopSpin();
                Controller.StopPanning();
            }
            var z = Vector3.Normalize(Camera.LookDirection);
            var x = Vector3.Cross(z, Vector3.Normalize(Camera.UpDirection));
            var y = Vector3.Cross(x, z);
            x = Vector3.Cross(z, y);

            switch (CameraMode)
            {
                case CameraMode.Inspect:
                case CameraMode.WalkAround:
                    Camera.Position += (x * delta.X) + (y * delta.Y) + (z * delta.Z);
                    break;
            }
        }

        /// <summary>
        /// The change camera width.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        /// <param name="zoomAround">
        /// The zoom around.
        /// </param>
        /// <param name="isTouch"></param>
        public void ZoomByChangingCameraWidth(float delta, Vector3 zoomAround, bool isTouch = false)
        {
            if (!isTouch)
            {
                if (delta < -0.5f)
                {
                    delta = -0.5f;
                }
            }

            switch (CameraMode)
            {
                case CameraMode.WalkAround:
                case CameraMode.Inspect:
                case CameraMode.FixedPosition:
                    if (ChangeCameraDistance(ref delta, zoomAround))
                    {
                        // Modify the camera width
                        if (Camera is OrthographicCameraCore ocamera)
                        {
                            ocamera.Width *= (float)Math.Pow(2.5f, delta);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
        /// </summary>
        /// <returns>
        /// True if the execution can continue.
        /// </returns>
        protected override bool CanStart()
        {
            if (changeFieldOfView)
            {
                return Controller.IsChangeFieldOfViewEnabled && Camera is PerspectiveCameraCore;
            }

            return Controller.IsZoomEnabled;
        }

        /// <summary>
        /// Changes the camera distance.
        /// </summary>
        /// <param name="delta">The delta.</param>
        /// <param name="zoomAround">The zoom around point.</param>
        private bool ChangeCameraDistance(ref float delta, Vector3 zoomAround)
        {
            // Handle the 'zoomAround' point
            var target = Camera.Position + Camera.LookDirection;
            var relativeTarget = zoomAround - target;
            var relativePosition = zoomAround - Camera.Position;
            if (relativePosition.LengthSquared() < 1e-5)
            {
                if (delta > 0) //If Zoom out from very close distance, increase the initial relativePosition
                {
                    relativePosition.Normalize();
                    relativePosition /= 10;
                }
                else//If Zoom in too close, stop it.
                {
                    return false;
                }
            }
            var f = Math.Pow(2.5, delta);
            var newRelativePosition = relativePosition * (float)f;
            var newRelativeTarget = relativeTarget * (float)f;

            var newTarget = zoomAround - newRelativeTarget;
            var newPosition = zoomAround - newRelativePosition;

            var newDistance = (newPosition - zoomAround).Length();
            var oldDistance = (Camera.Position - zoomAround).Length();

            if (newDistance > Controller.ZoomDistanceLimitFar && (oldDistance < Controller.ZoomDistanceLimitFar || newDistance > oldDistance))
            {
                var ratio = (newDistance - Controller.ZoomDistanceLimitFar) / newDistance;
                f *= 1 - ratio;
                newRelativePosition = relativePosition * (float)f;
                newRelativeTarget = relativeTarget * (float)f;

                newTarget = zoomAround - newRelativeTarget;
                newPosition = zoomAround - newRelativePosition;
                delta = (float)(Math.Log(f) / Math.Log(2.5));
            }

            if (newDistance < Controller.ZoomDistanceLimitNear && (oldDistance > Controller.ZoomDistanceLimitNear || newDistance < oldDistance))
            {
                var ratio = (Controller.ZoomDistanceLimitNear - newDistance) / newDistance;
                f *= (1 + ratio);
                newRelativePosition = relativePosition * (float)f;
                newRelativeTarget = relativeTarget * (float)f;

                newTarget = zoomAround - newRelativeTarget;
                newPosition = zoomAround - newRelativePosition;
                delta = (float)(Math.Log(f) / Math.Log(2.5));
            }

            var newLookDirection = newTarget - newPosition;
            Camera.LookDirection = newLookDirection;
            Camera.Position = newPosition;
            return true;
        }
    }
}
