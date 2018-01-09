/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransform
    {
        /// <summary>
        /// Local transform
        /// </summary>
        Matrix ModelMatrix { set; get; }
        /// <summary>
        /// Transform from its parent
        /// </summary>
        Matrix ParentMatrix { set; get; }
        /// <summary>
        /// Total model transform by ModelMatrix * ParentMatrix
        /// </summary>
        Matrix TotalModelMatrix { get; }
    }
}
