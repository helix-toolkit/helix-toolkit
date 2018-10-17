// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseGestureHandler.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   An abstract base class for the mouse gesture handlers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.UWP
{
    using System.Diagnostics;
    using Windows.Foundation;
    using Windows.UI.Core;
    using SharpDX;
    using Point3D = SharpDX.Vector3;
    using Vector3D = SharpDX.Vector3;
    using Point = Windows.Foundation.Point;
    using System;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// An abstract base class for the mouse gesture handlers.
    /// </summary>
    internal abstract class MouseGestureHandler
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
            //this.ManipulationWatch = new Stopwatch();
        }

        /// <summary>
        /// Gets the origin.
        /// </summary>
        public Point3D Origin
        {
            get
            {
                if (Controller.RotateAroundMouseDownPoint && this.MouseDownNearestPoint3D != null)
                {
                    return this.MouseDownNearestPoint3D.Value;
                }
                else if (this.MouseDownPoint3D != null)
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
                return this.Controller.ActualCamera as ProjectionCamera;
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
        protected Point3D? LastPoint3D;
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
        protected Point3D? MouseDownNearestPoint3D;

        /// <summary>
        /// Gets or sets the mouse down point (2D screen coordinates).
        /// </summary>
        protected Vector2 MouseDownPoint;

        /// <summary>
        /// Gets or sets the mouse down point (3D world coordinates).
        /// </summary>
        protected Point3D? MouseDownPoint3D;

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
        private CoreCursor OldCursor { get; set; }

        /// <summary>
        /// Occurs when the manipulation is completed.
        /// </summary>
        /// <param name="e">
        /// The <see cref="Vector2"/> instance containing the event data.
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
        /// The <see cref="Vector2"/> instance containing the event data.
        /// </param>
        public virtual void Delta(Vector2 e)
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
        public void Execute(object sender, PointerRoutedEventArgs e)
        {
            if (!this.CanExecute())
            {
                return;
            }
            this.Controller.Viewport.PointerReleased -= OnMouseUp;
            this.Controller.Viewport.PointerMoved -= OnMouseMove;

            this.Controller.Viewport.PointerReleased += OnMouseUp;
            //this.Viewport.Focus();
            this.Controller.Viewport.CapturePointer(e.Pointer);
            this.OnMouseDown(sender, e);
            this.Controller.Viewport.PointerMoved += OnMouseMove;
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
            inv = Camera.CreateLeftHandSystem ? -1 : 1;
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
        public Point3D? UnProject(Point p, Point3D position, Vector3D normal)
        {
            var ray = this.GetRay(p);
            if (ray == null)
            {
                return null;
            }

            return ray.PlaneIntersection(position, normal);
        }

        public Point3D? UnProject(Vector2 p, Point3D position, Vector3D normal)
        {
            var ray = this.GetRay(p);
            if (ray == null)
            {
                return null;
            }

            return ray.PlaneIntersection(position, normal);
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
        public Point3D? UnProject(Point p)
        {
            return this.UnProject(p, this.Camera.Target, this.Camera.LookDirection);
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
        protected abstract CoreCursorType GetCursor();

        /// <summary>
        /// Get the ray into the view volume given by the position in 2D (screen coordinates)
        /// </summary>
        /// <param name="position">
        /// A 2D point.
        /// </param>
        /// <returns>
        /// A ray
        /// </returns>
        protected Ray GetRay(Point position)
        {
            return this.Controller.Viewport.UnProject(position);
        }

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
        /// Called when the mouse button is pressed down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="PointerRoutedEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnMouseDown(object sender, PointerRoutedEventArgs e)
        {
            this.Started(e.GetCurrentPoint(this.Controller.Viewport).Position);

            this.OldCursor = Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor;
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(this.GetCursor(), OldCursor.Id);
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
        protected virtual void OnMouseMove(object sender, PointerRoutedEventArgs e)
        {
            this.Delta(e.GetCurrentPoint(this.Controller.Viewport).Position.ToVector2());
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
        protected virtual void OnMouseUp(object sender, PointerRoutedEventArgs e)
        {
            this.Controller.Viewport.PointerMoved -= this.OnMouseMove;
            this.Controller.Viewport.PointerReleased -= this.OnMouseUp;
            this.Controller.Viewport.ReleasePointerCapture(e.Pointer);
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = this.OldCursor;
            this.Completed(e.GetCurrentPoint(this.Controller.Viewport).Position);
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
            return this.Controller.Viewport.Project(p);
        }

        /// <summary>
        /// Sets mouse down point.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        private void SetMouseDownPoint(Point position)
        {
            this.MouseDownPoint = position.ToVector2();

            if (!this.Controller.Viewport.FixedRotationPointEnabled 
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