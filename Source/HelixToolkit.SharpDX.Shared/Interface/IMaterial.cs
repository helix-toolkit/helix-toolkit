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
    using System.Collections.Generic;

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

    /// <summary>
    /// A material proxy variable interface to manage all the material related resources
    /// </summary>
    public interface IEffectMaterialVariables : IMaterialRenderCore, IDisposable
    {
        event EventHandler<bool> OnInvalidateRenderer;
        /// <summary>
        /// Update material constant buffer, including all the colors and rendering setting
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool UpdateMaterialConstantBuffer(DeviceContext context);
        /// <summary>
        /// Bind the material texture maps to registers defined inside shader
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        /// <returns></returns>
        bool BindMaterialTextures(DeviceContext context, IShader shader);
        /// <summary>
        /// Bind material texture maps to multiple shaders
        /// </summary>
        /// <param name="context"></param>
        /// <param name="shader"></param>
        /// <returns></returns>
        bool BindMaterialTextures(DeviceContext context, IEnumerable<IShader> shader);
    }
}
