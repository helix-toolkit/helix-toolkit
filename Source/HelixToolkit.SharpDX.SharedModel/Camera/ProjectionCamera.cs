/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System.ComponentModel;
using System.Runtime.CompilerServices;

#if NETFX_CORE
using  Windows.UI.Xaml;
using Vector3D = SharpDX.Vector3;
using Point3D = SharpDX.Vector3;

namespace HelixToolkit.UWP
#elif WINUI
using Microsoft.UI.Xaml;
using Vector3D = SharpDX.Vector3;
using Point3D = SharpDX.Vector3;
using HelixToolkit.SharpDX.Core.Cameras;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using System.Windows.Media.Media3D;
#if COREWPF
using HelixToolkit.SharpDX.Core.Cameras;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Cameras;
#endif

    public interface IProjectionCameraModel : ICameraModel
    {
        double FarPlaneDistance
        {
            set; get;
        }
        double NearPlaneDistance
        {
            set; get;
        }
    }
    /// <summary>
    /// An abstract base class for perspective and orthographic projection cameras.
    /// </summary>
    public abstract class ProjectionCamera : Camera, IProjectionCameraModel
    {
        /// <summary>
        /// The create left hand system property
        /// </summary>
        public static readonly DependencyProperty CreateLeftHandSystemProperty =
            DependencyProperty.Register(
                "CreateLeftHandSystem", typeof(bool), typeof(ProjectionCamera), new PropertyMetadata(false, (d, e) =>
                {
                    ((d as Camera).CameraInternal as ProjectionCameraCore).CreateLeftHandSystem = (bool)e.NewValue;
                }));

        /// <summary>
        /// The far plane distance property.
        /// </summary>
        public static readonly DependencyProperty FarPlaneDistanceProperty =
            DependencyProperty.Register(
                "FarPlaneDistance", typeof(double), typeof(ProjectionCamera), new PropertyMetadata(1e3, (d, e) =>
                {
                    ((d as Camera).CameraInternal as ProjectionCameraCore).FarPlaneDistance = (float)(double)e.NewValue;
                }));

        /// <summary>
        /// The look direction property
        /// </summary>
        public static readonly DependencyProperty LookDirectionProperty = DependencyProperty.Register(
            "LookDirection", typeof(Vector3D), typeof(ProjectionCamera), new PropertyMetadata(new Vector3D(0, 0, -5), (d, e) =>
            {
#if NETFX_CORE|| WINUI
                ((d as Camera).CameraInternal as ProjectionCameraCore).LookDirection = (Vector3D)e.NewValue;
#else
                ((d as Camera).CameraInternal as ProjectionCameraCore).LookDirection = ((Vector3D)e.NewValue).ToVector3();
#endif
            }));

        /// <summary>
        /// The near plane distance property
        /// </summary>
        public static readonly DependencyProperty NearPlaneDistanceProperty =
            DependencyProperty.Register(
                "NearPlaneDistance", typeof(double), typeof(ProjectionCamera), new PropertyMetadata(0.01, (d, e) =>
                {
                    ((d as Camera).CameraInternal as ProjectionCameraCore).NearPlaneDistance = (float)(double)e.NewValue;
                }));

        /// <summary>
        /// The position property
        /// </summary>
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position",
            typeof(Point3D),
            typeof(ProjectionCamera),
            new PropertyMetadata(new Point3D(0, 0, +5), (d, e) =>
            {
#if NETFX_CORE|| WINUI
                ((d as Camera).CameraInternal as ProjectionCameraCore).Position = (Point3D)e.NewValue;
#else
                ((d as Camera).CameraInternal as ProjectionCameraCore).Position = ((Point3D)e.NewValue).ToVector3();
#endif
            }));

        /// <summary>
        /// Up direction property
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register(
            "UpDirection", typeof(Vector3D), typeof(ProjectionCamera), new PropertyMetadata(new Vector3D(0, 1, 0), (d, e) =>
            {
#if NETFX_CORE|| WINUI
                ((d as Camera).CameraInternal as ProjectionCameraCore).UpDirection = (Vector3D)e.NewValue;
#else
                ((d as Camera).CameraInternal as ProjectionCameraCore).UpDirection = ((Vector3D)e.NewValue).ToVector3();
#endif
            }));

        /// <summary>
        /// Gets or sets a value indicating whether to create a left hand system.
        /// </summary>
        /// <value>
        /// <c>true</c> if creating a left hand system; otherwise, <c>false</c>.
        /// </value>
        public override bool CreateLeftHandSystem
        {
            get
            {
                return (bool)this.GetValue(CreateLeftHandSystemProperty);
            }
            set
            {
                this.SetValue(CreateLeftHandSystemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the far plane distance.
        /// </summary>
        /// <value>
        /// The far plane distance.
        /// </value>
        public double FarPlaneDistance
        {
            get
            {
                return (double)this.GetValue(FarPlaneDistanceProperty);
            }
            set
            {
                this.SetValue(FarPlaneDistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>
        /// The look direction.
        /// </value>
        public override Vector3D LookDirection
        {
            get
            {
                return (Vector3D)this.GetValue(LookDirectionProperty);
            }
            set
            {
                this.SetValue(LookDirectionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the near plane distance.
        /// </summary>
        /// <value>
        /// The near plane distance.
        /// </value>
        public double NearPlaneDistance
        {
            get
            {
                return (double)this.GetValue(NearPlaneDistanceProperty);
            }
            set
            {
                this.SetValue(NearPlaneDistanceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        public override Point3D Position
        {
            get
            {
                return (Point3D)this.GetValue(PositionProperty);
            }
            set
            {
                this.SetValue(PositionProperty, value);
            }
        }

        /// <summary>
        /// Gets the target position.
        /// </summary>
        /// <value>
        /// The target.
        /// </value>
        public Point3D Target
        {
            get
            {
                return this.Position + this.LookDirection;
            }
        }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        public override Vector3D UpDirection
        {
            get
            {
                return (Vector3D)this.GetValue(UpDirectionProperty);
            }
            set
            {
                this.SetValue(UpDirectionProperty, value);
            }
        }
    }
}
