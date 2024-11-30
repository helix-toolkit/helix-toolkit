﻿using HelixToolkit.SharpDX;
using SharpDX;
using Windows.UI.Core;

namespace HelixToolkit.WinUI.SharpDX;

/// <summary>
/// Handles zooming.
/// </summary>
internal class ZoomHandler : MouseGestureHandler
{
    /// <summary>
    /// The change field of view.
    /// </summary>
    private readonly bool changeFieldOfView;

    /// <summary>
    /// The zoom point.
    /// </summary>
    private Point zoomPoint;

    /// <summary>
    /// The 3D zoom point.
    /// </summary>
    private Point3D zoomPoint3D;

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
    /// Occurs when the manipulation is completed.
    /// </summary>
    /// <param name="e">The <see cref="Point"/> instance containing the event data.</param>
    public override void Completed(Point e)
    {
        base.Completed(e);
        //this.Viewport.HideTargetAdorner();
    }

    /// <summary>
    /// Occurs when the position is changed during a manipulation.
    /// </summary>
    /// <param name="e">The <see cref="Point"/> instance containing the event data.</param>
    public override void Delta(Vector2 e)
    {
        var delta = e - this.LastPoint;
        this.LastPoint = e;
        this.Zoom(delta.Y * 0.01, this.zoomPoint3D);
    }

    /// <summary>s
    /// Occurs when the manipulation is started.
    /// </summary>
    /// <param name="e">The <see cref="Point"/> instance containing the event data.</param>
    public override void Started(Point e)
    {
        base.Started(e);
        this.zoomPoint = new Point(this.Controller.Viewport.ActualWidth / 2, this.Controller.Viewport.ActualHeight / 2);
        this.zoomPoint3D = this.Camera?.CameraInternal.Target ?? new Vector3();

        if (this.Controller.ZoomAroundMouseDownPoint && this.MouseDownNearestPoint3D != null)
        {
            this.zoomPoint = this.MouseDownPoint.ToPoint();
            this.zoomPoint3D = this.MouseDownNearestPoint3D.Value;
        }

        if (!this.changeFieldOfView)
        {
            // this.Viewport.ShowTargetAdorner(this.zoomPoint);
        }
    }

    /// <summary>
    /// Zooms the view.
    /// </summary>
    /// <param name="delta">
    /// The delta.
    /// </param>
    public void Zoom(double delta)
    {
        if (this.Camera is null)
        {
            return;
        }

        this.Zoom(delta, this.Camera.CameraInternal.Target);
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
    public void Zoom(double delta, Point3D zoomAround, bool isTouch = false, bool stopOther = true)
    {
        if (!this.Controller.IsZoomEnabled)
        {
            return;
        }
        if (stopOther)
        {
            this.Controller.StopSpin();
            this.Controller.StopPanning();
        }
        if (this.Camera is PerspectiveCamera)
        {
            if (!isTouch)
            {
                if (delta < -0.5)
                {
                    delta = -0.5;
                }

                delta *= this.ZoomSensitivity;
            }


            if (this.CameraMode == CameraMode.FixedPosition || this.changeFieldOfView)
            {
                this.Controller.Viewport.ZoomByChangingFieldOfView(delta);
            }
            else
            {
                switch (this.CameraMode)
                {
                    case CameraMode.Inspect:
                        this.ChangeCameraDistance(ref delta, zoomAround);
                        break;
                    case CameraMode.WalkAround:
                        this.Camera.Position -= this.Camera.CameraInternal.LookDirection * (float)delta;
                        break;
                }
            }

            return;
        }
        else if (this.Camera is OrthographicCamera)
        {
            switch (this.CameraMode)
            {
                case CameraMode.WalkAround:
                    this.Camera.Position -= this.Camera.CameraInternal.LookDirection * (float)delta;
                    break;
                default:
                    this.ZoomByChangingCameraWidth(delta, zoomAround);
                    break;
            }
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
            this.Controller.StopSpin();
            this.Controller.StopPanning();
        }

        if (this.Camera is null)
        {
            return;
        }

        var z = Vector3.Normalize(this.Camera.CameraInternal.LookDirection);
        var x = Vector3.Cross(this.Camera.CameraInternal.LookDirection, this.Camera.CameraInternal.UpDirection);
        var y = Vector3.Normalize(Vector3.Cross(x, z));
        x = Vector3.Cross(z, y);

        // delta *= this.ZoomSensitivity;
        switch (this.CameraMode)
        {
            case CameraMode.Inspect:
            case CameraMode.WalkAround:
                this.Camera.Position += (x * delta.X) + (y * delta.Y) + (z * delta.Z);
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
    public void ZoomByChangingCameraWidth(double delta, Point3D zoomAround, bool isTouch = false)
    {
        if (!isTouch)
        {
            if (delta < -0.5)
            {
                delta = -0.5;
            }
        }

        if (ChangeCameraDistance(ref delta, zoomAround))
        {
            // Modify the camera width
            if (this.Camera is OrthographicCamera ocamera)
            {
                ocamera.Width *= Math.Pow(2.5, delta);
            }
        }
    }

    /// <summary>
    /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
    /// </summary>
    /// <returns>
    /// True if the execution can continue.
    /// </returns>
    protected override bool CanExecute()
    {
        if (this.changeFieldOfView)
        {
            return this.Controller.IsChangeFieldOfViewEnabled && this.Camera is PerspectiveCamera;
        }

        return this.Controller.IsZoomEnabled;
    }

    /// <summary>
    /// Gets the cursor for the gesture.
    /// </summary>
    /// <returns>
    /// A cursor.
    /// </returns>
    protected override CoreCursorType GetCursor()
    {
        return this.Controller.ZoomCursor;
    }

    /// <summary>
    /// Changes the camera distance.
    /// </summary>
    /// <param name="delta">The delta.</param>
    /// <param name="zoomAround">The zoom around point.</param>
    private bool ChangeCameraDistance(ref double delta, Point3D zoomAround)
    {
        if (this.Camera is null)
        {
            return false;
        }

        // Handle the 'zoomAround' point
        var target = this.Camera.CameraInternal.Position + this.Camera.CameraInternal.LookDirection;
        var relativeTarget = zoomAround - target;
        var relativePosition = zoomAround - this.Camera.CameraInternal.Position;
        if (relativePosition.LengthSquared() < 1e-5)
        {
            if (delta > 0) //If Zoom out from very close distance, increase the initial relativePosition
            {
                relativePosition = Vector3.Normalize(relativePosition) / 10;
                zoomAround = relativePosition + this.Camera.CameraInternal.Position;
                relativeTarget = zoomAround - target;
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
        var oldDistance = (this.Camera.CameraInternal.Position - zoomAround).Length();

        if (newDistance > this.Controller.ZoomDistanceLimitFar && (oldDistance < this.Controller.ZoomDistanceLimitFar || newDistance > oldDistance))
        {
            var ratio = (newDistance - this.Controller.ZoomDistanceLimitFar) / newDistance;
            f *= 1 - ratio;
            newRelativePosition = relativePosition * (float)f;
            newRelativeTarget = relativeTarget * (float)f;

            newTarget = zoomAround - newRelativeTarget;
            newPosition = zoomAround - newRelativePosition;
            delta = Math.Log(f) / Math.Log(2.5);
        }

        if (newDistance < this.Controller.ZoomDistanceLimitNear && (oldDistance > this.Controller.ZoomDistanceLimitNear || newDistance < oldDistance))
        {
            var ratio = (this.Controller.ZoomDistanceLimitNear - newDistance) / newDistance;
            f *= (1 + ratio);
            newRelativePosition = relativePosition * (float)f;
            newRelativeTarget = relativeTarget * (float)f;

            newTarget = zoomAround - newRelativeTarget;
            newPosition = zoomAround - newRelativePosition;
            delta = Math.Log(f) / Math.Log(2.5);
        }

        var newLookDirection = newTarget - newPosition;
        if (newLookDirection.LengthSquared() < 1e-5)
        {
            return false;
        }
        this.Camera.LookDirection = newLookDirection;
        this.Camera.Position = newPosition;
        return true;
    }
}
