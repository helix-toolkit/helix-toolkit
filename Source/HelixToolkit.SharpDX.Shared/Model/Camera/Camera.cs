using System;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;

#if NETFX_CORE
namespace HelixToolkit.UWP.Cameras
#else
namespace HelixToolkit.Wpf.SharpDX.Cameras
#endif
{
    using HelixToolkit.Mathematics;
    using Model;
    using System.Globalization;

    public abstract class CameraCore : ObservableObject, ICamera
    {
        private Vector3 position;
        public Vector3 Position
        {
            set
            {
                Set(ref position, value);
            }
            get { return position; }
        }

        private Vector3 lookDirection;
        public Vector3 LookDirection
        {
            set
            {
                Set(ref lookDirection, value);
            }
            get { return lookDirection; }
        }

        private Vector3 upDirection;
        public Vector3 UpDirection
        {
            set
            {
                Set(ref upDirection, value);
            }
            get { return upDirection; }
        }

        public Vector3 Target
        {
            get { return position + lookDirection; }
        }

        private bool createLeftHandSystem = false;
        /// <summary>
        /// Gets or sets a value indicating whether to create a left hand system.
        /// </summary>
        /// <value>
        /// <c>true</c> if creating a left hand system; otherwise, <c>false</c>.
        /// </value>
        public bool CreateLeftHandSystem
        {
            set
            {
                Set(ref createLeftHandSystem, value);
            }
            get
            {
                return createLeftHandSystem;
            }
        }

        public abstract Matrix CreateProjectionMatrix(float aspectRatio);

        public abstract Matrix CreateViewMatrix();

        public override string ToString()
        {
            var target = Position + LookDirection;
            return string.Format(
                        CultureInfo.InvariantCulture,
                        "LookDirection:\t{0:0.000},{1:0.000},{2:0.000}",
                        LookDirection.X,
                        LookDirection.Y,
                        LookDirection.Z) + "\n"
                        + string.Format(
                        CultureInfo.InvariantCulture,
                        "UpDirection:\t{0:0.000},{1:0.000},{2:0.000}",
                        UpDirection.X,
                        UpDirection.Y,
                        UpDirection.Z) + "\n"
                        + string.Format(
                        CultureInfo.InvariantCulture,
                        "Position:\t\t{0:0.000},{1:0.000},{2:0.000}",
                        Position.X,
                        Position.Y,
                        Position.Z) + "\n"
                        + string.Format(
                        CultureInfo.InvariantCulture,
                        "Target:\t\t{0:0.000},{1:0.000},{2:0.000}",
                        target.X,
                        target.Y,
                        target.Z);
        }
    }

    public class MatrixCameraCore : CameraCore
    {
        public Matrix ProjectionMatrix { set; get; }
        public Matrix ViewMatrix { set; get; }
        public override Matrix CreateProjectionMatrix(float aspectRatio) { return ProjectionMatrix; }

        public override Matrix CreateViewMatrix() { return ViewMatrix; }

        public override string ToString()
        {
            return $"ProjMatrix: {ProjectionMatrix.ToString()} \nViewMatrix: {ViewMatrix.ToString()}";
        }
    }

    public abstract class ProjectionCameraCore : CameraCore
    {
        private float farPlane = 100;
        /// <summary>
        /// Gets or sets the far plane distance.
        /// </summary>
        /// <value>
        /// The far plane distance.
        /// </value>
        public float FarPlaneDistance
        {
            set
            {
                Set(ref farPlane, value);
            }
            get
            {
                return farPlane;
            }
        }

        private float nearPlane = 0.001f;
        /// <summary>
        /// Gets or sets the near plane distance.
        /// </summary>
        /// <value>
        /// The near plane distance.
        /// </value>
        public float NearPlaneDistance
        {
            set
            {
                Set(ref nearPlane, value);
            }
            get
            {
                return nearPlane;
            }
        }

        public override Matrix CreateViewMatrix()
        {
            return CreateLeftHandSystem ? MatrixHelper.LookAtLH(this.Position, this.Position + this.LookDirection, this.UpDirection)
                : Matrix.CreateLookAt(this.Position, this.Position + this.LookDirection, this.UpDirection);
        }

        public override string ToString()
        {
            return base.ToString() + "\n" +
                string.Format(
                        CultureInfo.InvariantCulture, "NearPlaneDist:\t{0}", NearPlaneDistance) + "\n"
                        + string.Format(CultureInfo.InvariantCulture, "FarPlaneDist:\t{0}", FarPlaneDistance);
        }
    }

    public class OrthographicCameraCore : ProjectionCameraCore
    {
        private float width = 100;
        public float Width
        {
            set
            {
                Set(ref width, value);
            }
            get
            {
                return width;
            }
        }

        public override Matrix CreateProjectionMatrix(float aspectRatio)
        {
            return this.CreateLeftHandSystem ? MatrixHelper.OrthoLH(
                    this.Width,
                    (float)(this.Width / aspectRatio),
                    this.NearPlaneDistance,
                    Math.Min(1e15f, this.FarPlaneDistance))
                    : MatrixHelper.OrthoRH(
                    this.Width,
                    (float)(this.Width / aspectRatio),
                    this.NearPlaneDistance,
                    Math.Min(1e15f, this.FarPlaneDistance));

        }

        public override string ToString()
        {
            return base.ToString() + "\n" + string.Format(CultureInfo.InvariantCulture, "Width:\t{0:0.###}", Width);
        }
    }

    public class PerspectiveCameraCore : ProjectionCameraCore
    {
        public float FieldOfView
        {
            set; get;
        } = 45;

        public override Matrix CreateProjectionMatrix(float aspectRatio)
        {
            var fov = this.FieldOfView * Math.PI / 180;
            Matrix projM;
            if (this.CreateLeftHandSystem)
            {
                projM = MatrixHelper.PerspectiveFovLH(
                    (float)fov,
                    aspectRatio,
                    NearPlaneDistance,
                    FarPlaneDistance);
            }
            else
            {
                projM = MatrixHelper.PerspectiveFovRH(
                    (float)fov, (float)aspectRatio, NearPlaneDistance, FarPlaneDistance);
            }
            if (float.IsNaN(projM.M33) || float.IsNaN(projM.M43))
            {
                projM.M33 = projM.M43 = -1;
            }
            return projM;
        }

        public override string ToString()
        {
            return base.ToString() + "\n" + string.Format(CultureInfo.InvariantCulture, "FieldOfView:\t{0:0.#}°", FieldOfView);
        }
    }
}
