/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using System.Globalization;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Cameras
    {
        using Model;

        public abstract class CameraCore : ObservableObject, ICamera
        {
            private Vector3 position;
            public Vector3 Position
            {
                set
                {
                    Set(ref position, value);
                }
                get
                {
                    return position;
                }
            }

            private Vector3 lookDirection;
            public Vector3 LookDirection
            {
                set
                {
                    Set(ref lookDirection, value);
                }
                get
                {
                    return lookDirection;
                }
            }

            private Vector3 upDirection;
            public Vector3 UpDirection
            {
                set
                {
                    Set(ref upDirection, value);
                }
                get
                {
                    return upDirection;
                }
            }

            public Vector3 Target
            {
                get
                {
                    return position + lookDirection;
                }
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

            public abstract Matrix CreateProjectionMatrix(float aspectRatio, float nearPlane, float farPlane);

            public abstract Matrix CreateViewMatrix();

            public abstract FrustumCameraParams CreateCameraParams(float aspectRatio);
            public abstract FrustumCameraParams CreateCameraParams(float aspectRatio, float nearPlane, float farPlane);
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


            private Vector3 targetPosition;
            private Vector3 targetLookDirection;
            private Vector3 targetUpDirection;
            private Vector3 oldPosition;
            private Vector3 oldLookDir;
            private Vector3 oldUpDir;
            private float aniTime = 0;
            private float accumTime = 0;
            private long prevTicks = 0;
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
                float animationTime)
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
                    prevTicks = System.Diagnostics.Stopwatch.GetTimestamp();
                    OnUpdateAnimation(0);
                }
            }
            /// <summary>
            /// Called when [time step] to update camera animation.
            /// </summary>
            /// <returns></returns>
            public virtual bool OnTimeStep()
            {
                var ticks = System.Diagnostics.Stopwatch.GetTimestamp();
                var ellapsed = (float)(ticks - prevTicks) / System.Diagnostics.Stopwatch.Frequency * 1000;
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
                    var l = accumTime / aniTime;
                    var nextPos = Vector3.Lerp(oldPosition, targetPosition, l);
                    var nextLook = Vector3.Lerp(oldLookDir, targetLookDirection, l);
                    var nextUp = Vector3.Lerp(oldUpDir, targetUpDirection, l);
                    Position = nextPos;
                    LookDirection = nextLook;
                    UpDirection = nextUp;
                    return true;
                }
            }

            public void StopAnimation()
            {
                aniTime = 0;
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
                return CreateLeftHandSystem ? Matrix.LookAtLH(this.Position, this.Position + this.LookDirection, this.UpDirection)
                    : Matrix.LookAtRH(this.Position, this.Position + this.LookDirection, this.UpDirection);
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

            public override FrustumCameraParams CreateCameraParams(float aspectRatio)
            {
                return CreateCameraParams(aspectRatio, NearPlaneDistance, FarPlaneDistance);
            }

            public override FrustumCameraParams CreateCameraParams(float aspectRatio, float nearPlane, float farPlane)
            {
                return new FrustumCameraParams()
                {
                    AspectRatio = aspectRatio,
                    FOV = (float)Math.PI / 2,
                    LookAtDir = LookDirection,
                    UpDir = UpDirection,
                    Position = Position,
                    ZNear = nearPlane,
                    ZFar = farPlane
                };
            }

            public override Matrix CreateProjectionMatrix(float aspectRatio)
            {
                return CreateProjectionMatrix(aspectRatio, NearPlaneDistance, FarPlaneDistance);
            }

            public override Matrix CreateProjectionMatrix(float aspectRatio, float nearPlane, float farPlane)
            {
                return this.CreateLeftHandSystem ?
                    Matrix.OrthoLH(this.Width, (float)(this.Width / aspectRatio), nearPlane, Math.Min(1e15f, farPlane))
                    : Matrix.OrthoRH(this.Width, (float)(this.Width / aspectRatio), nearPlane, Math.Min(1e15f, farPlane));
            }


            public override string ToString()
            {
                return base.ToString() + "\n" + string.Format(CultureInfo.InvariantCulture, "Width:\t{0:0.###}", Width);
            }

#if CORE
            private float oldWidth;
            private float targetWidth;
            private float accumTime;
            private float aniTime;

            public void AnimateWidth(float newWidth, float animationTime)
            {
                if (animationTime == 0)
                {
                    UpdateCameraPositionByWidth(newWidth);
                    Width = newWidth;
                }
                else
                {
                    oldWidth = Width;
                    this.targetWidth = newWidth;
                    accumTime = 1;
                    aniTime = animationTime;
                    OnUpdateAnimation(0);
                }
            }

            protected override bool OnUpdateAnimation(float ellapsed)
            {
                bool res = base.OnUpdateAnimation(ellapsed);
                if (aniTime == 0)
                {
                    return res;
                }
                accumTime += ellapsed;
                if (accumTime > aniTime)
                {
                    UpdateCameraPositionByWidth(targetWidth);
                    Width = targetWidth;
                    aniTime = 0;
                    return res;
                }
                else
                {
                    var newWidth = oldWidth + (targetWidth - oldWidth) * (accumTime / aniTime);
                    UpdateCameraPositionByWidth(newWidth);
                    Width = newWidth;
                    return true;
                }
            }

            private void UpdateCameraPositionByWidth(double newWidth)
            {
                var ratio = newWidth / Width;
                var dir = LookDirection;
                var target = Target;
                var dist = dir.Length();
                var newDist = dist * ratio;
                dir.Normalize();
                var position = (target - dir * (float)newDist);
                var lookDir = dir * (float)newDist;
                Position = position;
                LookDirection = lookDir;
            }
#endif
        }

        public class PerspectiveCameraCore : ProjectionCameraCore
        {
            public float FieldOfView
            {
                set; get;
            } = 45;

            public override Matrix CreateProjectionMatrix(float aspectRatio)
            {
                return CreateProjectionMatrix(aspectRatio, NearPlaneDistance, FarPlaneDistance);
            }

            public override Matrix CreateProjectionMatrix(float aspectRatio, float nearPlane, float farPlane)
            {
                var fov = this.FieldOfView * Math.PI / 180;
                Matrix projM;
                if (this.CreateLeftHandSystem)
                {
                    projM = Matrix.PerspectiveFovLH((float)fov, aspectRatio, nearPlane, farPlane);
                }
                else
                {
                    projM = Matrix.PerspectiveFovRH((float)fov, (float)aspectRatio, nearPlane, farPlane);
                }
                if (float.IsNaN(projM.M33) || float.IsNaN(projM.M43))
                {
                    projM.M33 = projM.M43 = -1;
                }
                return projM;
            }

            public override FrustumCameraParams CreateCameraParams(float aspectRatio)
            {
                return CreateCameraParams(aspectRatio, NearPlaneDistance, FarPlaneDistance);
            }
            public override FrustumCameraParams CreateCameraParams(float aspectRatio, float nearPlane, float farPlane)
            {
                return new FrustumCameraParams()
                {
                    AspectRatio = aspectRatio,
                    FOV = FieldOfView / 180f * (float)(Math.PI),
                    LookAtDir = LookDirection,
                    UpDir = UpDirection,
                    Position = Position,
                    ZNear = nearPlane,
                    ZFar = farPlane
                };
            }
            public override string ToString()
            {
                return base.ToString() + "\n" + string.Format(CultureInfo.InvariantCulture, "FieldOfView:\t{0:0.#}°", FieldOfView);
            }
        }
    }
}
