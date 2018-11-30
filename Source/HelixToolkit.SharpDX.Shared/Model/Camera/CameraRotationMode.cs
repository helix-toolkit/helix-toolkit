/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    /// <summary>
    /// Camera rotation modes.
    /// </summary>
    public enum CameraRotationMode
    {
        /// <summary>
        /// Turntable is constrained to two axes of rotation (model up and right direction)
        /// </summary>
        Turntable,

        /// <summary>
        /// Turnball using three axes (look direction, right direction and up direction (on the left/right edges)).
        /// </summary>
        Turnball,

        /// <summary>
        /// Using a virtual trackball.
        /// </summary>
        Trackball
    }
}