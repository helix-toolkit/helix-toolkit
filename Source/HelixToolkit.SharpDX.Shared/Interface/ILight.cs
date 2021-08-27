/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if !NETFX_CORE && !NET5_0
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
#else
namespace TT.HelixToolkit.UWP
#endif
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILight3D
    {
        /// <summary>
        /// Gets the type of the light.
        /// </summary>
        /// <value>
        /// The type of the light.
        /// </value>
        LightType LightType
        {
            get;
        }
    }
}
