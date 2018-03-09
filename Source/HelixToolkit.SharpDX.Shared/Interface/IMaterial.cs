/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.ComponentModel;
using System.IO;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Shaders;
    using Core;

    /// <summary>
    /// 
    /// </summary>
    public interface IMaterial : INotifyPropertyChanged
    {
        string Name { set; get; }
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
    }

    /// <summary>
    /// A material proxy variable interface to manage all the material related resources
    /// </summary>
    public interface IEffectMaterialVariables : IMaterialRenderParams, IDisposable
    {
        /// <summary>
        /// Occurs when [on invalidate renderer].
        /// </summary>
        event EventHandler<EventArgs> OnInvalidateRenderer;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelstruct"></param>
        /// <returns></returns>
        bool UpdateMaterialVariables(ref ModelStruct modelstruct);

        /// <summary>
        /// Bind material texture maps to multiple shaders
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shaderPass"></param>
        /// <returns></returns>
        bool BindMaterialTextures(DeviceContext context, IShaderPass shaderPass);
    }
}
