/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
namespace HelixToolkit.SharpDX.Core.Controls
{
    using Cameras;

    public sealed class RotateHandler : MouseGestureHandler
    {
        /// <summary>
        /// The change look at.
        /// </summary>
        private readonly bool changeLookAt;

        /// <summary>
        /// The x rotation axis.
        /// </summary>
        private Vector3 rotationAxisX;

        /// <summary>
        /// The y rotation axis.
        /// </summary>
        private Vector3 rotationAxisY;

        /// <summary>
        /// The rotation point.
        /// </summary>
        private Vector2 rotationPoint;

        /// <summary>
        /// The 3D rotation point.
        /// </summary>
        private Vector3 rotationPoint3D;
        /// <summary>
        /// Gets the camera rotation mode.
        /// </summary>
        /// <value>
        /// The camera rotation mode.
        /// </value>
        private CameraRotationMode CameraRotationMode
        {
            get
            {
                return this.Controller.CameraRotationMode;
            }
        }

        public RotateHandler(CameraController controller, bool changeLookAt = false) 
            : base(controller)
        {
            this.changeLookAt = changeLookAt;
        }

        /// <summary>
        /// Occurs when the position is changed during a manipulation.
        /// </summary>
        /// <param name="e">The <see cref="T:SharpDX.Vector2" /> instance containing the event data.</param>
        public override void Delta(Vector2 e)
        {
            base.Delta(e);
            this.Rotate(this.LastPoint, e, this.rotationPoint3D);
            this.LastPoint = e;
        }

        /// <summary>
        /// Change the "look-at" point.
        /// </summary>
        /// <param name="target">
        /// The target.
        /// </param>
        /// <param name="animationTime">
        /// The animation time.
        /// </param>
        public void LookAt(Vector3 target, float animationTime)
        {
            if (!this.Controller.IsPanEnabled)
            {
                return;
            }

            this.Camera.LookAt(target, animationTime);
        }

        /// <summary>
        /// Rotate the camera around the specified point.
        /// </summary>
        /// <param name="p0">
        /// The p 0.
        /// </param>
        /// <param name="p1">
        /// The p 1.
        /// </param>
        /// <param name="rotateAround">
        /// The rotate around.
        /// </param>
        /// <param name="stopOther">Stop other manipulation</param>
        public void Rotate(Vector2 p0, Vector2 p1, Vector3 rotateAround, bool stopOther = true)
        {
            if (!this.Controller.IsRotationEnabled)
            {
                return;
            }
            if (stopOther)
            {
                Controller.StopZooming();
                Controller.StopPanning();
            }
            p0 = Vector2.Multiply(p0, Controller.AllowRotateXY);
            p1 = Vector2.Multiply(p1, Controller.AllowRotateXY);
            var newPos = Camera.Position;
            var newLook = Camera.LookDirection;
            var newUp = Vector3.Normalize(Camera.UpDirection);
            switch (this.Controller.CameraRotationMode)
            {
                case CameraRotationMode.Trackball:
                    CameraMath.RotateTrackball(CameraMode, ref p0, ref p1, ref rotateAround, (float)RotationSensitivity,
                        Controller.Width, Controller.Height, Camera, inv, out newPos, out newLook, out newUp);
                    break;
                case CameraRotationMode.Turntable:
                    var p = p1 - p0;
                    CameraMath.RotateTurntable(CameraMode, ref p, ref rotateAround, (float)RotationSensitivity,
                        Controller.Width, Controller.Height, Camera, inv, ModelUpDirection, out newPos, out newLook, out newUp);
                    break;
                case CameraRotationMode.Turnball:
                    CameraMath.RotateTurnball(CameraMode, ref p0, ref p1, ref rotateAround, (float)RotationSensitivity,
                        Controller.Width, Controller.Height, Camera, inv, out newPos, out newLook, out newUp);
                    break;
            }
            Camera.LookDirection = newLook;
            Camera.Position = newPos;
            Camera.UpDirection = newUp;
        }

        /// <summary>
        /// Occurs when the manipulation is started.
        /// </summary>
        /// <param name="e">The <see cref="T:SharpDX.Vector2" /> instance containing the event data.</param>
        protected override void Started(Vector2 e)
        {
            base.Started(e);
            this.rotationPoint = new Vector2(
                this.Controller.Width / 2, this.Controller.Height / 2);
            this.rotationPoint3D = this.Camera.Target;

            switch (this.CameraMode)
            {
                case CameraMode.WalkAround:
                    this.rotationPoint = this.MouseDownPoint;
                    this.rotationPoint3D = this.Camera.Position;
                    break;
                default:
                    if (Controller.FixedRotationPointEnabled)
                    {
                        this.rotationPoint3D = Controller.FixedRotationPoint;
                    }
                    else if (this.changeLookAt && this.MouseDownNearestPoint3D != null)
                    {
                        this.LookAt(this.MouseDownNearestPoint3D.Value, 0);
                        this.rotationPoint3D = this.Camera.Target;
                    }
                    else if (this.Controller.RotateAroundMouseDownPoint && this.MouseDownNearestPoint3D != null)
                    {
                        this.rotationPoint = this.MouseDownPoint;
                        this.rotationPoint3D = this.MouseDownNearestPoint3D.Value;
                    }

                    break;
            }

            switch (this.CameraRotationMode)
            {
                case CameraRotationMode.Trackball:
                    break;
                case CameraRotationMode.Turntable:
                    break;
                case CameraRotationMode.Turnball:
                    CameraMath.InitTurnballRotationAxes(e, (int)Controller.Width, (int)Controller.Height, Camera,
                        out rotationAxisX, out rotationAxisY);
                    break;
            }

            this.Controller.StopSpin();
        }

        /// <summary>
        /// Called when inertia is starting.
        /// </summary>
        /// <param name="elapsedTime">
        /// The elapsed time.
        /// </param>
        protected override void OnInertiaStarting(double elapsedTime)
        {
            var delta = this.LastPoint - this.MouseDownPoint;
            var deltaV = new Vector2((float)delta.X, (float)delta.Y);
            // Debug.WriteLine("SpinInertiaStarting: " + elapsedTime + "ms " + delta.Length + "px");
            this.Controller.StartSpin(
                4 * deltaV * (float)(this.Controller.SpinReleaseTime / elapsedTime),
                this.MouseDownPoint,
                this.rotationPoint3D);
        }
    }
}
