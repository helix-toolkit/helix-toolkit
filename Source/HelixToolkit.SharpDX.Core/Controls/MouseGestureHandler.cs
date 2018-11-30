/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Diagnostics;
using System;
namespace HelixToolkit.SharpDX.Core.Controls
{
    using Cameras;
    public abstract class MouseGestureHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseGestureHandler"/> class.
        /// </summary>
        /// <param name="cameraController">
        /// The viewport.
        /// </param>
        protected MouseGestureHandler(CameraController cameraController)
        {
            this.Controller = cameraController;
        }

        /// <summary>
        /// Gets the origin.
        /// </summary>
        public Vector3 Origin
        {
            get
            {
                if (Controller.RotateAroundMouseDownPoint && this.MouseDownNearestPoint3D.HasValue)
                {
                    return this.MouseDownNearestPoint3D.Value;
                }
                else if (this.MouseDownPoint3D.HasValue)
                {
                    return this.MouseDownPoint3D.Value;
                }

                return new Vector3();
            }
        }

        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>The camera.</value>
        protected ProjectionCameraCore Camera
        {
            get
            {
                return this.Controller.ActualCamera as ProjectionCameraCore;
            }
        }

        /// <summary>
        /// Gets the camera mode.
        /// </summary>
        /// <value>The camera mode.</value>
        protected CameraMode CameraMode
        {
            get
            {
                return this.Controller.CameraMode;
            }
        }

        /// <summary>
        /// Gets or sets the last point (in 2D screen coordinates).
        /// </summary>
        protected Vector2 LastPoint;

        /// <summary>
        /// Gets or sets the last point (in 3D world coordinates).
        /// </summary>
        protected Vector3? LastPoint3D;
        /// <summary>
        /// Use to invert the left handed system
        /// </summary>
        /// <value>
        /// The inv.
        /// </value>
        protected int inv = 1;
        /// <summary>
        /// Gets the model up direction.
        /// </summary>
        /// <value>The model up direction.</value>
        protected Vector3 ModelUpDirection
        {
            get
            {
                return this.Controller.ModelUpDirection;
            }
        }

        /// <summary>
        /// Gets or sets the mouse down Vector2 at the nearest hit element (3D world coordinates).
        /// </summary>
        protected Vector3? MouseDownNearestPoint3D;

        /// <summary>
        /// Gets or sets the mouse down Vector2 (2D screen coordinates).
        /// </summary>
        protected Vector2 MouseDownPoint;

        /// <summary>
        /// Gets or sets the mouse down Vector2 (3D world coordinates).
        /// </summary>
        protected Vector3? MouseDownPoint3D;

        /// <summary>
        /// Gets the rotation sensitivity.
        /// </summary>
        /// <value>The rotation sensitivity.</value>
        protected double RotationSensitivity
        {
            get
            {
                return this.Controller.RotationSensitivity;
            }
        }

        /// <summary>
        /// Gets the viewport.
        /// </summary>
        /// <value>The viewport.</value>
        protected CameraController Controller { get; private set; }

        /// <summary>
        /// Gets the zoom sensitivity.
        /// </summary>
        /// <value>The zoom sensitivity.</value>
        protected float ZoomSensitivity
        {
            get
            {
                return this.Controller.ZoomSensitivity;
            }
        }

        private long startTick;

        public event EventHandler MouseCaptureRequested;
        public event EventHandler MouseReleaseRequested;


        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">
        /// The <see cref="Vector2"/> instance containing the event data.
        /// </param>
        public virtual void Delta(Vector2 e)
        {
        }


        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">
        /// The <see cref="Vector2"/> instance containing the event data.
        /// </param>
        protected virtual void Started(Vector2 e)
        {
            this.SetMouseDownPoint(e);
            this.LastPoint = this.MouseDownPoint;
            this.LastPoint3D = this.MouseDownPoint3D;
            startTick = Stopwatch.GetTimestamp();
            inv = Camera.CreateLeftHandSystem ? -1 : 1;
            Controller.StopAnimations();
            Controller.Viewport.InvalidateRender();
        }

        /// <summary>
        /// Un-projects a Vector2 from the screen (2D) to a Vector2 on plane (3D)
        /// </summary>
        /// <param name="p">
        /// The 2D Vector2.
        /// </param>
        /// <param name="position">
        /// plane position
        /// </param>
        /// <param name="normal">
        /// plane normal
        /// </param>
        /// <returns>
        /// A 3D Vector2.
        /// </returns>
        public Vector3? UnProject(Vector2 p, Vector3 position, Vector3 normal)
        {
            var ray = this.GetRay(p);
            var plane = new Plane(position, normal);
            if(Collision.RayIntersectsPlane(ref ray, ref plane, out Vector3 point))
            {
                return point;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Un-projects a Vector2 from the screen (2D) to a Vector2 on the plane trough the camera target Vector2.
        /// </summary>
        /// <param name="p">
        /// The 2D Vector2.
        /// </param>
        /// <returns>
        /// A 3D Vector2.
        /// </returns>
        public Vector3? UnProject(Vector2 p)
        {
            return this.UnProject(p, this.Camera.Target, this.Camera.LookDirection);
        }

        /// <summary>
        /// Get the ray into the view volume given by the position in 2D (screen coordinates)
        /// </summary>
        /// <param name="position">
        /// A 2D Vector2.
        /// </param>
        /// <returns>
        /// A ray
        /// </returns>
        protected Ray GetRay(Vector2 position)
        {
            return this.Controller.Viewport.UnProject(position);
        }

        /// <summary>
        /// Called when inertia is starting.
        /// </summary>
        /// <param name="elapsedTime">
        /// The elapsed time (milliseconds).
        /// </param>
        protected virtual void OnInertiaStarting(double elapsedTime)
        {
        }

        /// <summary>
        /// Mouses down.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns></returns>
        public virtual bool Start(Vector2 e)
        {
            if (CanStart())
            {
                MouseCaptureRequested?.Invoke(this, EventArgs.Empty);
                this.Started(e);
                return true;
            }
            else { return false; }
        }

        protected virtual bool CanStart()
        {
            return true;
        }

        /// <summary>
        /// Mouses the move.
        /// </summary>
        /// <param name="e">The e.</param>
        public virtual void MouseMove(Vector2 e)
        {
            Delta(e);
            Controller.Viewport.InvalidateRender();
        }

        /// <summary>
        /// Mouses up.
        /// </summary>
        /// <param name="e">The e.</param>
        public virtual void End(Vector2 e)
        {
            MouseReleaseRequested?.Invoke(this, EventArgs.Empty);
            Completed(e);
        }

        protected virtual void Completed(Vector2 e)
        {
            var elapsed = (double)(Stopwatch.GetTimestamp() - startTick) / Stopwatch.Frequency * 1000; //this.ManipulationWatch.ElapsedMilliseconds;
            if (elapsed > 0 && elapsed < this.Controller.SpinReleaseTime)
            {
                this.OnInertiaStarting(elapsed);
            }
            startTick = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Calculate the screen position of a 3D Vector2.
        /// </summary>
        /// <param name="p">
        /// The 3D Vector2.
        /// </param>
        /// <returns>
        /// The 2D Vector2.
        /// </returns>
        protected Vector2 Project(Vector3 p)
        {
            return this.Controller.Viewport.Project(p);
        }

        /// <summary>
        /// Sets mouse down Vector2.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        private void SetMouseDownPoint(Vector2 position)
        {
            this.MouseDownPoint = position;

            if (!this.Controller.FixedRotationPointEnabled
                && this.Controller.Viewport.FindNearest(position, out var nearestPoint, out var normal, out var visual))
            {
                this.MouseDownNearestPoint3D = nearestPoint;
            }
            else
            {
                this.MouseDownNearestPoint3D = null;
            }

            this.MouseDownPoint3D = this.UnProject(position);
        }
    }
}
