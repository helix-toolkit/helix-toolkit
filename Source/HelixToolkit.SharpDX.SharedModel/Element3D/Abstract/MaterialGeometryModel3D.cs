/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
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
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GeometryModel3D" />
    public abstract class MaterialGeometryModel3D : GeometryModel3D
    {
        #region Dependency Properties
        /// <summary>
        /// Render shadow on this mesh if has shadow map
        /// </summary>
        public static readonly DependencyProperty RenderShadowMapProperty =
            DependencyProperty.Register("RenderShadowMap", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(false,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as MaterialGeometryNode).RenderShadowMap = (bool)e.NewValue;
                }));

        /// <summary>
        /// Render environment reflection map on this mesh if has environment map
        /// </summary>
        public static readonly DependencyProperty RenderEnvironmentMapProperty =
            DependencyProperty.Register("RenderEnvironmentMap", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(false,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as MaterialGeometryNode).RenderEnvironmentMap = (bool)e.NewValue;
                }));
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(MaterialGeometryModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MaterialGeometryNode).Material = e.NewValue as Material;
            }));

        /// <summary>
        /// Specifiy if model material is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public static readonly DependencyProperty IsTransparentProperty =
            DependencyProperty.Register("IsTransparent", typeof(bool), typeof(MaterialGeometryModel3D), new PropertyMetadata(false, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MaterialGeometryNode).IsTransparent = (bool)e.NewValue;
            }));

        /// <summary>
        /// Render shadow on this mesh if has shadow map
        /// <para>Default: false</para>
        /// </summary>
        public bool RenderShadowMap
        {
            get { return (bool)this.GetValue(RenderShadowMapProperty); }
            set { this.SetValue(RenderShadowMapProperty, value); }
        }

        /// <summary>
        /// Render environment map on this mesh if has environment map
        /// <para>Default: false</para>
        /// </summary>
        public bool RenderEnvironmentMap
        {
            get { return (bool)this.GetValue(RenderEnvironmentMapProperty); }
            set { this.SetValue(RenderEnvironmentMapProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public Material Material
        {
            get { return (Material)this.GetValue(MaterialProperty); }
            set { this.SetValue(MaterialProperty, value); }
        }

        /// <summary>
        /// Specifiy if model material is transparent. 
        /// During rendering, transparent objects are rendered after opaque objects. Transparent objects' order in scene graph are preserved.
        /// </summary>
        public bool IsTransparent
        {
            get { return (bool)GetValue(IsTransparentProperty); }
            set { SetValue(IsTransparentProperty, value); }
        }
        #endregion        
        /// <summary>
        /// Assigns the default values to scene node.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            if (node is MaterialGeometryNode n)
            {
                n.Material = this.Material;
                n.RenderEnvironmentMap = this.RenderEnvironmentMap;
                n.RenderShadowMap = this.RenderShadowMap;
            }
            base.AssignDefaultValuesToSceneNode(node);
        }
    }
}
