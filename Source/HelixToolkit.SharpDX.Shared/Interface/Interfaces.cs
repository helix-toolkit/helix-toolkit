// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interfaces.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using SharpDX.Direct3D11;
using SharpDX;
using System;

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    public interface IGUID
    {
        Guid GUID { get; }
    }

    public interface IResourceSharing : IDisposable
    {
        int ReferenceCount { get; }

        /// <summary>
        /// Add reference counter;
        /// </summary>
        /// <returns>Current count</returns>
        int AddReference();

        /// <summary>
        /// Decrease reference counter. When counter reach 0, release all internal resources if release = true
        /// </summary>
        /// <returns>Current count</returns>
        int RemoveReference(bool release);
        /// <summary>
        /// Decrease reference counter. When counter reach 0, release all internal resources automatically
        /// </summary>
        /// <returns>Current count</returns>
        int RemoveReference();
    }

    public interface IEffectMaterialVariables : IMaterialRenderCore, IDisposable
    {
        event System.EventHandler<bool> OnInvalidateRenderer;
        bool AttachMaterial(MeshGeometry3D model);
    }

    public interface IRenderTechnique
    {
        string Name { get; }

        Effect Effect { get; }

        EffectTechnique EffectTechnique { get; }

        Device Device { get; }

        InputLayout InputLayout { get; }
    }

    public interface ICamera
    {
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>
        /// The position.
        /// </value>
        System.Windows.Media.Media3D.Point3D Position { get; set; }

        /// <summary>
        /// Gets or sets the look direction.
        /// </summary>
        /// <value>
        /// The look direction.
        /// </value>
        System.Windows.Media.Media3D.Vector3D LookDirection { get; set; }

        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        System.Windows.Media.Media3D.Vector3D UpDirection { get; set; }
        /// <summary>
        /// Creates the view matrix.
        /// </summary>
        /// <returns>A <see cref="Matrix" />.</returns>
        Matrix CreateViewMatrix();

        /// <summary>
        /// Creates the projection matrix.
        /// </summary>
        /// <param name="aspectRatio">The aspect ratio.</param>
        /// <returns>A <see cref="Matrix" />.</returns>
        Matrix CreateProjectionMatrix(double aspectRatio);
    }
}
