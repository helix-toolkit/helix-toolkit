/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using Model;
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="GeometryModel3D" />
    public abstract class MaterialGeometryModel3D : GeometryModel3D
    {
        #region Dependency Properties
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
        /// 
        /// </summary>
        public Material Material
        {
            get
            {
                return (Material)this.GetValue(MaterialProperty);
            }
            set
            {
                this.SetValue(MaterialProperty, value);
            }
        }

        /// <summary>
        /// Specifiy if model material is transparent. 
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
            }
            base.AssignDefaultValuesToSceneNode(node);
        }
    }
}
