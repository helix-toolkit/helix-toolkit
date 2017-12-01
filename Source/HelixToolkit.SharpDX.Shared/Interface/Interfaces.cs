// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Interfaces.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using SharpDX.Direct3D11;
using SharpDX;
using System;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Core;
    using System.ComponentModel;
    using System.IO;

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
        event EventHandler<bool> OnInvalidateRenderer;
        /// <summary>
        /// Attach material variables and textures
        /// </summary>
        /// <returns></returns>
        bool AttachMaterial();
    }

    public interface IRenderTechnique
    {
        string Name { get; }

        Effect Effect { get; }

        EffectTechnique EffectTechnique { get; }

        Device Device { get; }

        InputLayout InputLayout { get; }
    }

    public interface IBillboardText
    {
        BillboardType Type { get; }
        Stream Texture { get; }

        Stream AlphaTexture { get; }
        void DrawTexture();
        Vector3Collection Positions { get; }
        IList<Vector2> TextureOffsets { get; }
        Vector2Collection TextureCoordinates { get; }
        Color4Collection Colors { get; }
        float Width { get; }
        float Height { get; }
    }

    public enum BillboardType
    {
        SingleText, MultipleText, SingleImage
    }

    public interface IMaterial : INotifyPropertyChanged
    {
        string Name { set; get; }
    }

    public interface IPhongMaterial : IMaterial
    {
        Color4 AmbientColor { set; get; }
        Color4 DiffuseColor { set; get; }
        Color4 EmissiveColor { set; get; }
        Color4 ReflectiveColor { set; get; }
        Color4 SpecularColor { set; get; }
        float SpecularShininess { set; get; }
        Stream DiffuseMap { set; get; }
        Stream DiffuseAlphaMap { set; get; }
        Stream NormalMap { set; get; }
        Stream DisplacementMap { set; get; }
    }
}
