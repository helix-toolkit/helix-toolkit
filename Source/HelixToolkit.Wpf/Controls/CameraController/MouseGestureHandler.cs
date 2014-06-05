// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseGestureHandler.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// An abstract base class for the mouse gesture handlers.
    /// </summary>
    internal abstract class MouseGestureHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseGestureHandler"/> class.
        /// </summary>
        /// <param name="controller">
        /// The controller.
        /// </param>
        protected MouseGestureHandler(CameraController controller)
        {
            this.Controller = controller;
            this.ManipulationWatch = new Stopwatch();
        }

        /// <summary>
        /// Gets the origin.
        /// </summary>
        public Point3D Origin
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

                return new Point3D();
            }
        }

        /// <summary>
        /// Gets the camera.
        /// </summary>
        /// <value>The camera.</value>
        protected ProjectionCamera Camera
        {
            get
            {
                return this.Viewport.Camera as ProjectionCamera;
            }
        }

        /// <summary>
        /// Gets or sets the camera look direction.
        /// </summary>
        /// <value>The camera look direction.</value>
        protected Vector3D CameraLookDirection
        {
            get
            {
                return this.Controller.CameraLookDirection;
            }

            set
            {
                this.Controller.CameraLookDirection = value;
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
        /// Gets or sets the camera position.
        /// </summary>
        /// <value>The camera position.</value>
        protected Point3D CameraPosition
        {
            get
            {
                return this.Controller.CameraPosition;
            }

            set
            {
                this.Controller.CameraPosition = value;
            }
        }

        /// <summary>
        /// Gets the camera target.
        /// </summary>
        /// <value>The camera target.</value>
        protected Point3D CameraTarget
        {
            get
            {
                return this.CameraPosition + this.CameraLookDirection;
            }
        }

        /// <summary>
        /// Gets or sets the camera up direction.
        /// </summary>
        /// <value>The camera up direction.</value>
        protected Vector3D CameraUpDirection
        {
            get
            {
                return this.Controller.CameraUpDirection;
            }

            set
            {
                this.Controller.CameraUpDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        protected CameraController Controller { get; set; }

        /// <summary>
        /// Gets or sets the last point (2D screen coordinates).
        /// </summary>
        protected Point LastPoint { get; set; }

        /// <summary>
        /// Gets or sets the last point (3D world coordinates).
        /// </summary>
        protected Point3D? LastPoint3D { get; set; }

        /// <summary>
        /// Gets or sets the manipulation watch.
        /// </summary>
        protected Stopwatch ManipulationWatch { get; set; }

        /// <summary>
        /// Gets the model up direction.
        /// </summary>
        /// <value>The model up direction.</value>
        protected Vector3D ModelUpDirection
        {
            get
            {
                return this.Controller.ModelUpDirection;
            }
        }

        /// <summary>
        /// Gets or sets the mouse down point at the nearest hit element (3D world coordinates).
        /// </summary>
        protected Point3D? MouseDownNearestPoint3D { get; set; }

        /// <summary>
        /// Gets or sets the mouse down point (2D screen coordinates).
        /// </summary>
        protected Point MouseDownPoint { get; set; }

        /// <summary>
        /// Gets or sets the mouse down point (3D world coordinates).
        /// </summary>
        protected Point3D? MouseDownPoint3D { get; set; }

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
        protected Viewport3D Viewport
        {
            get
            {
                return this.Controller.Viewport;
            }
        }

        /// <summary>
        /// Gets the height of the viewport.
        /// </summary>
        /// <value>The height of the viewport.</value>
        protected double ViewportHeight
        {
            get
            {
                return this.Controller.ActualHeight;
            }
        }

        /// <summary>
        /// Gets the width of the viewport.
        /// </summary>
        /// <value>The width of the viewport.</value>
        protected double ViewportWidth
        {
            get
            {
                return this.Controller.ActualWidth;
            }
        }

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
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        public virtual void Completed(ManipulationEventArgs e)
        {
            var elapsed = this.ManipulationWatch.ElapsedMilliseconds;
            if (elapsed > 0 && elapsed < this.Controller.SpinReleaseTime)
            {
                this.OnInertiaStarting((int)this.ManipulationWatch.ElapsedMilliseconds);
            }
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        public virtual void Delta(ManipulationEventArgs e)
        {
        }

        /// <summary>
        /// Executes the mousegesture command.
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

            this.Controller.PushCameraSetting();
            this.Controller.MouseMove += this.OnMouseMove;
            this.Controller.MouseUp += this.OnMouseUp;
            this.OnMouseDown(sender, null);
            this.Controller.Focus();
            this.Controller.CaptureMouse();
            this.Controller.PushCameraSetting();

            e.Handled = true;
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">
        /// The <see cref="ManipulationEventArgs"/> instance containing the event data.
        /// </param>
        public virtual void Started(ManipulationEventArgs e)
        {
            this.SetMouseDownPoint(e.CurrentPosition);
            this.LastPoint = this.MouseDownPoint;
            this.LastPoint3D = this.MouseDownPoint3D;
            this.ManipulationWatch.Restart();
        }

        /// <summary>
        /// Un projects a point from the screen (2D) to a point on plane (3D)
        /// </summary>
        /// <param name="p">
        /// The 2D point.
        /// </param>
        /// <param name="position">
        /// A point on the plane .
        /// </param>
        /// <param name="normal">
        /// The plane normal.
        /// </param>
        /// <returns>
        /// A 3D point.
        /// </returns>
        public Point3D? UnProject(Point p, Point3D position, Vector3D normal)
        {
            Ray3D ray = this.GetRay(p);
            if (ray == null)
            {
                return null;
            }

            Point3D i;
            return ray.PlaneIntersection(position, normal, out i) ? (Point3D?)i : null;
        }

        /// <summary>
        /// Un projects a point from the screen (2D) to a point on the plane trough the camera target point.
        /// </summary>
        /// <param name="p">
        /// The 2D point.
        /// </param>
        /// <returns>
        /// A 3D point.
        /// </returns>
        public Point3D? UnProject(Point p)
        {
            return this.UnProject(p, this.CameraTarget, this.CameraLookDirection);
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
        protected Ray3D GetRay(Point position)
        {
            Point3D point1, point2;
            bool ok = Viewport3DHelper.Point2DtoPoint3D(this.Viewport, position, out point1, out point2);
            if (!ok)
            {
                return null;
            }

            return new Ray3D { Origin = point1, Direction = point2 - point1 };
        }

        /// <summary>
        /// Called when inertia is starting.
        /// </summary>
        /// <param name="elapsedTime">
        /// The elapsed time (milliseconds).
        /// </param>
        protected virtual void OnInertiaStarting(int elapsedTime)
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
            this.Started(new ManipulationEventArgs(Mouse.GetPosition(this.Controller)));

            this.OldCursor = this.Controller.Cursor;
            this.Controller.Cursor = this.GetCursor();
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
            this.Delta(new ManipulationEventArgs(Mouse.GetPosition(this.Controller)));
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
            this.Controller.MouseMove -= this.OnMouseMove;
            this.Controller.MouseUp -= this.OnMouseUp;
            this.Controller.ReleaseMouseCapture();
            this.Controller.Cursor = this.OldCursor;
            this.Completed(new ManipulationEventArgs(Mouse.GetPosition(this.Controller)));
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
        protected Point Project(Point3D p)
        {
            return Viewport3DHelper.Point3DtoPoint2D(this.Viewport, p);
        }

        /// <summary>
        /// The set mouse down point.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        private void SetMouseDownPoint(Point position)
        {
            this.MouseDownPoint = position;

            Point3D nearestPoint;
            Vector3D normal;
            DependencyObject visual;
            if (Viewport3DHelper.FindNearest(
                this.Controller.Viewport, this.MouseDownPoint, out nearestPoint, out normal, out visual))
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