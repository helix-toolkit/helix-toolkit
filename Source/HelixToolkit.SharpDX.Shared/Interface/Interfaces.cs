// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interfaces.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

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

    public interface IEffectMaterialVariables : IMaterialRenderCore, IDisposable
    {
        event System.EventHandler<bool> OnInvalidateRenderer;
        bool AttachMaterial(MeshGeometry3D model);
    }

}
