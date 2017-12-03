using SharpDX;
using System;
using System.ComponentModel;
using System.IO;
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
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

    public interface IEffectMaterialVariables : IMaterialRenderCore, IDisposable
    {
        event EventHandler<bool> OnInvalidateRenderer;
        /// <summary>
        /// Attach material variables and textures
        /// </summary>
        /// <returns></returns>
        bool AttachMaterial();
    }
}
