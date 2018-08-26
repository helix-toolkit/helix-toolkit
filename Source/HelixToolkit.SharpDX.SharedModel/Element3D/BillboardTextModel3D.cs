/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
#if NETFX_CORE
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#else
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
    using Model.Scene;
    using Shaders;
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
                    ((d as Element3DCore).SceneNode as BillboardNode).FixedSize = (bool)e.NewValue;
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
            get { return (bool)GetValue(IsTransparentProperty); }
            set { SetValue(IsTransparentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the sampler description.
        /// </summary>
        /// <value>
        /// The sampler description.
        /// </value>
        public SamplerStateDescription SamplerDescription
        {
            get { return (SamplerStateDescription)GetValue(SamplerDescriptionProperty); }
            set { SetValue(SamplerDescriptionProperty, value); }
        }

        /// <summary>
        /// The sampler description property
        /// </summary>
        public static readonly DependencyProperty SamplerDescriptionProperty =
            DependencyProperty.Register("SamplerDescription", typeof(SamplerStateDescription), typeof(BillboardTextModel3D), new PropertyMetadata(DefaultSamplers.LinearSamplerClampAni1, (d,e) =>
            {
                ((d as Element3DCore).SceneNode as BillboardNode).SamplerDescription = (SamplerStateDescription)e.NewValue;
            }));
        #endregion

        #region Overridable Methods        

        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new BillboardNode();
        }
        /// <summary>
        /// Assigns the default values to core.
        /// </summary>
        /// <param name="core">The core.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            if (core is BillboardNode n)
            {
                n.FixedSize = FixedSize;
                n.IsTransparent = IsTransparent;
                n.SamplerDescription = SamplerDescription;
            }
            base.AssignDefaultValuesToSceneNode(core);       
        }
        #endregion
    }
}
