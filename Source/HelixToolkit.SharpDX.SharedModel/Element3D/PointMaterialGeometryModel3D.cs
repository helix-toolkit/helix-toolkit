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
    public class PointMaterialGeometryModel3D : GeometryModel3D
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty MaterialProperty =
            DependencyProperty.Register("Material", typeof(Material), typeof(PointMaterialGeometryModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as PointNode).Material = e.NewValue as Material;
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
        /// The hit test thickness property
        /// </summary>
        public static readonly DependencyProperty HitTestThicknessProperty =
            DependencyProperty.Register("HitTestThickness", typeof(double), typeof(PointMaterialGeometryModel3D), new PropertyMetadata(1.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as PointNode).HitTestThickness = (double)e.NewValue;
            }));

        /// <summary>
        /// Used only for point/line hit test
        /// </summary>
        public double HitTestThickness
        {
            get
            {
                return (double)this.GetValue(HitTestThicknessProperty);
            }
            set
            {
                this.SetValue(HitTestThicknessProperty, value);
            }
        }
        /// <summary>
        /// Called when [create render core].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new PointNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            base.AssignDefaultValuesToSceneNode(node);
            if (node is PointNode p)
            {
                p.Material = Material;
                p.HitTestThickness = HitTestThickness;
            }
        }
    }
}
