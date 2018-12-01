using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Text;

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
    using Utilities;
    /// <summary>
    /// 
    /// </summary>
    public sealed class Texture2DArgs : EventArgs
    {
        /// <summary>
        /// The texture
        /// </summary>
        public readonly ShaderResourceViewProxy Texture;
        /// <summary>
        /// Initializes a new instance of the <see cref="Texture2DArgs"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public Texture2DArgs(ShaderResourceViewProxy texture)
        {
            Texture = texture;
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="Texture2DArgs"/> to <see cref="ShaderResourceViewProxy"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ShaderResourceViewProxy(Texture2DArgs args)
        {
            return args.Texture;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class OctreeArgs : EventArgs
    {
        /// <summary>
        /// The octree
        /// </summary>
        public readonly IOctreeBasic Octree;
        /// <summary>
        /// Initializes a new instance of the <see cref="OctreeArgs"/> class.
        /// </summary>
        /// <param name="octree">The octree.</param>
        public OctreeArgs(IOctreeBasic octree)
        {
            Octree = octree;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class TransformArgs : EventArgs
    {
        /// <summary>
        /// The transform
        /// </summary>
        public readonly Matrix Transform;
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformArgs"/> class.
        /// </summary>
        /// <param name="transform">The transform.</param>
        public TransformArgs(Matrix transform)
        {
            Transform = transform;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformArgs"/> class.
        /// </summary>
        /// <param name="transform">The transform.</param>
        public TransformArgs(ref Matrix transform)
        {
            Transform = transform;
        }
        /// <summary>
        /// Performs an implicit conversion from <see cref="TransformArgs"/> to <see cref="Matrix"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Matrix(TransformArgs args)
        {
            return args.Transform;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class Transform2DArgs : EventArgs
    {
        /// <summary>
        /// The transform
        /// </summary>
        public readonly Matrix3x2 Transform;
        /// <summary>
        /// Initializes a new instance of the <see cref="Transform2DArgs"/> class.
        /// </summary>
        /// <param name="transform">The transform.</param>
        public Transform2DArgs(Matrix3x2 transform)
        {
            Transform = transform;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="Transform2DArgs"/> class.
        /// </summary>
        /// <param name="transform">The transform.</param>
        public Transform2DArgs(ref Matrix3x2 transform)
        {
            Transform = transform;
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Transform2DArgs"/> to <see cref="Matrix3x2"/>.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Matrix3x2(Transform2DArgs args)
        {
            return args.Transform;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public sealed class BoolArgs : EventArgs
    {
        /// <summary>
        /// The true arguments
        /// </summary>
        public static readonly BoolArgs TrueArgs = new BoolArgs(true);
        /// <summary>
        /// The false arguments
        /// </summary>
        public static readonly BoolArgs FalseArgs = new BoolArgs(false);
        /// <summary>
        /// The value
        /// </summary>
        public readonly bool Value;
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolArgs"/> class.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public BoolArgs(bool value)
        {
            Value = value;
        }
    }

    public sealed class FrameStatisticsArg : EventArgs
    {
        public readonly double AverageValue;
        public readonly double AverageFrequency;
        public FrameStatisticsArg(double avgValue, double avgFrequency)
        {
            AverageValue = avgValue;
            AverageFrequency = avgFrequency;
        }
    }
}
