// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseGestureHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   An abstract base class for the mouse gesture handlers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Numerics;

    /// <summary>
    /// An abstract base class for the mouse gesture handlers.
    /// </summary>
    internal abstract class MouseGestureHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseGestureHandler"/> class.
        /// </summary>
        /// <param name="controller">
        /// The camera controller.
        /// </param>
        protected MouseGestureHandler(CameraController controller)
        {
            Controller = controller;
        }

        /// <summary>
        /// Gets the origin.
        /// </summary>
        public Vector3 Origin
        {
            get
            {
                if (this.MouseDownNearestPoint3D != null)
                {
                    return this.MouseDownNearestPoint3D.Value;
                }

                if (this.MouseDownPoint3D != null)
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
        protected Camera Camera
        {
            get
            {
                return this.Controller.ActualCamera;
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
        protected Point LastPoint { get; set; }

        /// <summary>
        /// Gets or sets the last point (in 3D world coordinates).
        /// </summary>
        protected Vector3? LastPoint3D { get; set; }
        /// <summary>
        /// Use to invert the left handed system
        /// </summary>
        /// <value>
        /// The inv.
        /// </value>
        protected int Inv { private set; get; } = 1;

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
        /// Gets or sets the mouse down point at the nearest hit element (3D world coordinates).
        /// </summary>
        protected Vector3? MouseDownNearestPoint3D { get; set; }

        /// <summary>
        /// Gets or sets the mouse down point (2D screen coordinates).
        /// </summary>
        protected Point MouseDownPoint { get; set; }

        /// <summary>
        /// Gets or sets the mouse down point (3D world coordinates).
        /// </summary>
        protected Vector3? MouseDownPoint3D { get; set; }

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
        public Viewport3DX Viewport
        {
            get { return Controller.Viewport; }
        }

        public CameraController Controller { get; private set; }

        /// <summary>
        /// Gets the zoom sensitivity.
        /// </summary>
        /// <value>The zoom sensitivity.</value>
        protected double ZoomSensitivity
        {
            get
            {
                return this.Controller.ZoomSensitivity;
            }
        }

        /// <summary>
        /// Gets or sets the old cursor.
        /// </summary>
        private Cursor OldCursor { get; set; }

        /// <summary>
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="Point"/> instance containing the event data.
        /// </param>
        public virtual void Completed(Point e)
        {
            var elapsed = (double)(Stopwatch.GetTimestamp() - startTick) / Stopwatch.Frequency * 1000; //this.ManipulationWatch.ElapsedMilliseconds;
            if (elapsed > 0 && elapsed < this.Controller.SpinReleaseTime)
            {
                this.OnInertiaStarting(elapsed);
            }
            startTick = Stopwatch.GetTimestamp();
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">
        /// The <see cref="Point"/> instance containing the event data.
        /// </param>
        public virtual void Delta(Point e)
        {
        }

        /// <summary>
        /// Executes the mouse gesture command.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (!this.CanExecute())
            {
                return;
            }

            this.Viewport.MouseMove -= this.OnMouseMove;
            this.Viewport.MouseUp -= this.OnMouseUp;
            this.Viewport.MouseUp += this.OnMouseUp;
            this.Viewport.Focus();
            this.Viewport.CaptureMouse();
            this.OnMouseDown(sender, null);
            this.Viewport.MouseMove += this.OnMouseMove;
        }

        private long startTick;
        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">
        /// The <see cref="Point"/> instance containing the event data.
        /// </param>
        public virtual void Started(Point e)
        {
            this.SetMouseDownPoint(e);
            this.LastPoint = this.MouseDownPoint;
            this.LastPoint3D = this.MouseDownPoint3D;
            //this.ManipulationWatch.Restart();
            startTick = Stopwatch.GetTimestamp();
            Inv = Camera.CreateLeftHandSystem ? -1 : 1;
            Controller.StopAnimations();
            Controller.PushCameraSetting();
        }

        /// <summary>
        /// Un-projects a point from the screen (2D) to a point on plane (3D)
        /// </summary>
        /// <param name="p">
        /// The 2D point.
        /// </param>
        /// <param name="position">
        /// plane position
        /// </param>
        /// <param name="normal">
        /// plane normal
        /// </param>
        /// <returns>
        /// A 3D point.
        /// </returns>
        public Vector3? UnProject(Point p, Vector3 position, Vector3 normal)
        {
            var ray = this.GetRay(p);
            if (ray == null)
            {
                return null;
            }
            var plane = Mathematics.PlaneHelper.GetPlane(position, normal);
            if(ray.Intersects(ref plane, out Vector3 point))
            {
                return point;
            }
            else { return null; }
            //return ray.PlaneIntersection(position, normal);
        }

        /// <summary>
        /// Un-projects a point from the screen (2D) to a point on the plane trough the camera target point.
        /// </summary>
        /// <param name="p">
        /// The 2D point.
        /// </param>
        /// <returns>
        /// A 3D point.
        /// </returns>
        public Vector3? UnProject(Point p)
        {
            return this.UnProject(p, this.Camera.CameraInternal.Target, this.Camera.CameraInternal.LookDirection);
        }

        /// <summary>
        /// Occurs when the command associated with this handler initiates a check to determine whether the command can be executed on the command target.
        /// </summary>
        /// <returns>
        /// True if the execution can continue.
        /// </returns>
        protected virtual bool CanExecute()
        {
            return true;
        }

        /// <summary>
        /// Gets the cursor for the gesture.
        /// </summary>
        /// <returns>
        /// A cursor.
        /// </returns>
        protected abstract Cursor GetCursor();

        /// <summary>
        /// Get the ray into the view volume given by the position in 2D (screen coordinates)
        /// </summary>
        /// <param name="position">
        /// A 2D point.
        /// </param>
        /// <returns>
        /// A ray
        /// </returns>
        protected Mathematics.Ray GetRay(Point position)
        {
            return this.Viewport.UnProject(position);
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
        /// Called when the mouse button is pressed down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnMouseDown(object sender, MouseEventArgs e)
        {
            this.Started(Mouse.GetPosition(this.Viewport));

            this.OldCursor = this.Viewport.Cursor;
            this.Viewport.Cursor = this.GetCursor();
        }

        /// <summary>
        /// The on mouse move.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
            this.Delta(Mouse.GetPosition(this.Viewport));
        }

        /// <summary>
        /// The on mouse up.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Viewport.MouseMove -= this.OnMouseMove;
            this.Viewport.MouseUp -= this.OnMouseUp;
            this.Viewport.ReleaseMouseCapture();
            this.Viewport.Cursor = this.OldCursor;
            this.Completed(Mouse.GetPosition(this.Viewport));
        }

        /// <summary>
        /// Calculate the screen position of a 3D point.
        /// </summary>
        /// <param name="p">
        /// The 3D point.
        /// </param>
        /// <returns>
        /// The 2D point.
        /// </returns>
        protected Point Project(Vector3 p)
        {
            return this.Viewport.Project(p);
        }

        /// <summary>
        /// Sets mouse down point.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        private void SetMouseDownPoint(Point position)
        {
            this.MouseDownPoint = position;

            if (!this.Viewport.FixedRotationPointEnabled && this.Viewport.FindNearest(this.MouseDownPoint, out Vector3 nearestPoint, out Vector3 normal, out Element3D visual))
            {
                this.MouseDownNearestPoint3D = nearestPoint;
            }
            else
            {
                this.MouseDownNearestPoint3D = null;
            }

            this.MouseDownPoint3D = this.UnProject(this.MouseDownPoint);
        }
    }
}