/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;

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

    /// <summary>
    /// 
    /// </summary>
    public interface ITransform2D
    {
        /// <summary>
        /// Gets or sets the model matrix.
        /// </summary>
        /// <value>
        /// The model matrix.
        /// </value>
        Matrix3x2 ModelMatrix { set; get; }
        /// <summary>
        /// Gets or sets the parent matrix.
        /// </summary>
        /// <value>
        /// The parent matrix.
        /// </value>
        Matrix3x2 ParentMatrix { set; get; }
        /// <summary>
        /// Gets the total model matrix.
        /// </summary>
        /// <value>
        /// The total model matrix.
        /// </value>
        Matrix3x2 TotalModelMatrix { get; }
    }
}
