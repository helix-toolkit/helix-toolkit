/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System;
using System.ComponentModel;
using System.IO;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    /// <summary>
    /// 
    /// </summary>
    public interface IMaterial : INotifyPropertyChanged
    {
        string Name { set; get; }
        Guid Guid { get; }
        MaterialVariable CreateMaterialVariables(IEffectsManager manager, IRenderTechnique technique);
    }
    /// <summary>
    /// 
    /// </summary>
    public interface IPhongMaterial : IMaterial
    {
        /// <summary>
        /// Gets or sets the color of the ambient.
        /// </summary>
        /// <value>
        /// The color of the ambient.
        /// </value>
        Color4 AmbientColor { set; get; }
        /// <summary>
        /// Gets or sets the color of the diffuse.
        /// </summary>
        /// <value>
        /// The color of the diffuse.
        /// </value>
        Color4 DiffuseColor { set; get; }
        /// <summary>
        /// Gets or sets the color of the emissive.
        /// </summary>
        /// <value>
        /// The color of the emissive.
        /// </value>
        Color4 EmissiveColor { set; get; }
        /// <summary>
        /// Gets or sets the color of the reflective.
        /// </summary>
        /// <value>
        /// The color of the reflective.
        /// </value>
        Color4 ReflectiveColor { set; get; }
        /// <summary>
        /// Gets or sets the color of the specular.
        /// </summary>
        /// <value>
        /// The color of the specular.
        /// </value>
        Color4 SpecularColor { set; get; }
        /// <summary>
        /// Gets or sets the specular shininess.
        /// </summary>
        /// <value>
        /// The specular shininess.
        /// </value>
        float SpecularShininess { set; get; }
        /// <summary>
        /// Gets or sets the diffuse map.
        /// </summary>
        /// <value>
        /// The diffuse map.
        /// </value>
        Stream DiffuseMap { set; get; }
        /// <summary>
        /// Gets or sets the diffuse alpha map.
        /// </summary>
        /// <value>
        /// The diffuse alpha map.
        /// </value>
        Stream DiffuseAlphaMap { set; get; }
        /// <summary>
        /// Gets or sets the normal map.
        /// </summary>
        /// <value>
        /// The normal map.
        /// </value>
        Stream NormalMap { set; get; }
        /// <summary>
        /// Gets or sets the displacement map.
        /// </summary>
        /// <value>
        /// The displacement map.
        /// </value>
        Stream DisplacementMap { set; get; }
        /// <summary>
        /// Use to select which channel will be used as displacement value. Also scale the selected channel.
        /// </summary>
        Vector4 DisplacementMapScaleMask { set; get; }
        /// <summary>
        /// Gets or sets the diffuse map sampler.
        /// </summary>
        /// <value>
        /// The diffuse map sampler.
        /// </value>
        SamplerStateDescription DiffuseMapSampler { set; get; }
        /// <summary>
        /// Gets or sets the normal map sampler.
        /// </summary>
        /// <value>
        /// The normal map sampler.
        /// </value>
        SamplerStateDescription NormalMapSampler { set; get; }
        /// <summary>
        /// Gets or sets the diffuse alpha map sampler.
        /// </summary>
        /// <value>
        /// The diffuse alpha map sampler.
        /// </value>
        SamplerStateDescription DiffuseAlphaMapSampler { set; get; }
        /// <summary>
        /// Gets or sets the displacement map sampler.
        /// </summary>
        /// <value>
        /// The displacement map sampler.
        /// </value>
        SamplerStateDescription DisplacementMapSampler { set; get; }

        bool RenderDiffuseMap { set; get; }
        bool RenderDiffuseAlphaMap { set; get; }
        bool RenderNormalMap { set; get; }
        bool RenderDisplacementMap { set; get; }
        bool RenderEnvironmentMap { set; get; }
        bool RenderShadowMap { set; get; }
        bool EnableTessellation { set; get; }

        float MaxDistanceTessellationFactor { set; get; }

        float MinDistanceTessellationFactor { set; get; }

        float MaxTessellationDistance { set; get; }

        float MinTessellationDistance { set; get; }

        Matrix UVTransform { set; get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IMaterialVariablePool
    {
        int Count { get; }
        MaterialVariable Register(IMaterial material, IRenderTechnique technique);
    }
}
