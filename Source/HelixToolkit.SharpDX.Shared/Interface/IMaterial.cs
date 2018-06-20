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
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Render;
    using Shaders;
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
    public interface IEffectMaterialVariables : IDisposable
    {
        /// <summary>
        /// Gets or sets the default name of the shader pass.
        /// </summary>
        /// <value>
        /// The default name of the shader pass.
        /// </value>
        string DefaultShaderPassName { set; get; }
        /// <summary>
        /// Gets the material pass.
        /// </summary>
        /// <value>
        /// The material pass.
        /// </value>
        ShaderPass MaterialPass { get; }
        /// <summary>
        /// Gets or sets a value indicating whether [render diffuse map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render diffuse map]; otherwise, <c>false</c>.
        /// </value>
        bool RenderDiffuseMap { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [render diffuse alpha map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render diffuse alpha map]; otherwise, <c>false</c>.
        /// </value>
        bool RenderDiffuseAlphaMap { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [render normal map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render normal map]; otherwise, <c>false</c>.
        /// </value>
        bool RenderNormalMap { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [render displacement map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render displacement map]; otherwise, <c>false</c>.
        /// </value>
        bool RenderDisplacementMap { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether [render shadow map].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [render shadow map]; otherwise, <c>false</c>.
        /// </value>
        bool RenderShadowMap { set; get; }
        /// <summary>
        /// Reflect the environment cube map
        /// </summary>
        bool RenderEnvironmentMap { set; get; }
        /// <summary>
        /// Occurs when [on invalidate renderer].
        /// </summary>
        event EventHandler<EventArgs> OnInvalidateRenderer;
        /// <summary>
        /// Attaches the specified technique.
        /// </summary>
        /// <param name="technique">The technique.</param>
        /// <returns></returns>
        bool Attach(IRenderTechnique technique);
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
        bool BindMaterialTextures(DeviceContextProxy context, ShaderPass shaderPass);
    }
}
