/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Shaders;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model;
using HelixToolkit.SharpDX.Core.Model.Scene;
using HelixToolkit.SharpDX.Core.Shaders;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF && !WINUI
    using Model.Scene;
    using Shaders;
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GeometryModel3D" />
    public class BillboardTextModel3D : GeometryModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// Fixed sized billboard. Default = true. 
        /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
        /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
        /// </summary>
        public static readonly DependencyProperty FixedSizeProperty = DependencyProperty.Register("FixedSize", typeof(bool), typeof(BillboardTextModel3D),
            new PropertyMetadata(true,
                (d, e) =>
                {
                    (d as BillboardTextModel3D).material.FixedSize = (bool)e.NewValue;
                }));

        /// <summary>
        /// Fixed sized billboard. Default = true. 
        /// <para>When FixedSize = true, the billboard render size will be scale to normalized device coordinates(screen) size</para>
        /// <para>When FixedSize = false, the billboard render size will be actual size in 3D world space</para>
        /// </summary>
        public bool FixedSize
        {
            set
            {
                SetValue(FixedSizeProperty, value);
            }
            get
            {
                return (bool)GetValue(FixedSizeProperty);
            }
        }

        /// <summary>
        /// Specifiy if billboard texture is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public static readonly DependencyProperty IsTransparentProperty =
            DependencyProperty.Register("IsTransparent", typeof(bool), typeof(BillboardTextModel3D), new PropertyMetadata(false, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as BillboardNode).IsTransparent = (bool)e.NewValue;
            }));

        /// <summary>
        /// Specifiy if  billboard texture is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public bool IsTransparent
        {
            get
            {
                return (bool)GetValue(IsTransparentProperty);
            }
            set
            {
                SetValue(IsTransparentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the sampler description.
        /// </summary>
        /// <value>
        /// The sampler description.
        /// </value>
        public SamplerStateDescription SamplerDescription
        {
            get
            {
                return (SamplerStateDescription)GetValue(SamplerDescriptionProperty);
            }
            set
            {
                SetValue(SamplerDescriptionProperty, value);
            }
        }

        /// <summary>
        /// The sampler description property
        /// </summary>
        public static readonly DependencyProperty SamplerDescriptionProperty =
            DependencyProperty.Register("SamplerDescription", typeof(SamplerStateDescription), typeof(BillboardTextModel3D), new PropertyMetadata(DefaultSamplers.LinearSamplerClampAni1, (d, e) =>
            {
                (d as BillboardTextModel3D).material.SamplerDescription = (SamplerStateDescription)e.NewValue;
            }));
        #endregion

        #region Overridable Methods        

        protected readonly BillboardMaterialCore material = new BillboardMaterialCore();
        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new BillboardNode() { Material = material };
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            if (core is BillboardNode n)
            {
                material.FixedSize = FixedSize;
                n.IsTransparent = IsTransparent;
                material.SamplerDescription = SamplerDescription;
            }
            base.AssignDefaultValuesToSceneNode(core);
        }
        #endregion
    }
}
