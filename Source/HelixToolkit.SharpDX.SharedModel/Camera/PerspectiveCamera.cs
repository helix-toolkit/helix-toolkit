/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Cameras;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Cameras;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Cameras;
#endif
    public interface IPerspectiveCameraModel
    {
        double FieldOfView
        {
            set; get;
        }
    }
    /// <summary>
    /// Represents a perspective projection camera.
    /// </summary>
    public class PerspectiveCamera : ProjectionCamera, IPerspectiveCameraModel
    {
        /// <summary>
        /// The field of view property
        /// </summary>
        public static readonly DependencyProperty FieldOfViewProperty = DependencyProperty.Register(
            "FieldOfView", typeof(double), typeof(PerspectiveCamera), new PropertyMetadata(45.0,
                (d, e) =>
                {
                    ((d as Camera).CameraInternal as PerspectiveCameraCore).FieldOfView = (float)(double)e.NewValue;
                }));

        /// <summary>
        /// Gets or sets the field of view.
        /// </summary>
        /// <value>
        /// The field of view.
        /// </value>
        public double FieldOfView
        {
            get
            {
                return (double)this.GetValue(FieldOfViewProperty);
            }
            set
            {
                this.SetValue(FieldOfViewProperty, value);
            }
        }

        protected override CameraCore CreatePortableCameraCore()
        {
            return new PerspectiveCameraCore();
        }

        protected override void OnCoreCreated(CameraCore core)
        {
            base.OnCoreCreated(core);
            (core as PerspectiveCameraCore).FarPlaneDistance = (float)this.FarPlaneDistance;
            (core as PerspectiveCameraCore).FieldOfView = (float)this.FieldOfView;
            (core as PerspectiveCameraCore).NearPlaneDistance = (float)this.NearPlaneDistance;
        }

#if !NETFX_CORE && !WINUI
        protected override Freezable CreateInstanceCore()
        {
            return new PerspectiveCamera();
        }
#endif
    }
}
