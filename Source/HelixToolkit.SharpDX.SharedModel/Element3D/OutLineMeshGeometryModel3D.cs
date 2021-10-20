/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if NETFX_CORE
using  Windows.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Windows.UI.Colors;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using Color = Windows.UI.Color;
using Colors = Microsoft.UI.Colors;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using System.Windows;
using Color = System.Windows.Media.Color;
using Colors = System.Windows.Media.Colors;
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

    public class OutLineMeshGeometryModel3D : MeshGeometryModel3D
    {
        public static readonly DependencyProperty EnableOutlineProperty = DependencyProperty.Register("EnableOutline", typeof(bool), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshOutlineNode).EnableOutline = (bool)e.NewValue;
            }));

        public bool EnableOutline
        {
            set
            {
                SetValue(EnableOutlineProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableOutlineProperty);
            }
        }

        public static DependencyProperty OutlineColorProperty = DependencyProperty.Register("OutlineColor", typeof(Color), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(Colors.White,
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshOutlineNode).OutlineColor = ((Color)e.NewValue).ToColor4();
            }));

        public Color OutlineColor
        {
            set
            {
                SetValue(OutlineColorProperty, value);
            }
            get
            {
                return (Color)GetValue(OutlineColorProperty);
            }
        }

        public static DependencyProperty IsDrawGeometryProperty = DependencyProperty.Register("IsDrawGeometry", typeof(bool), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshOutlineNode).IsDrawGeometry = (bool)e.NewValue;
            }));

        public bool IsDrawGeometry
        {
            set
            {
                SetValue(IsDrawGeometryProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDrawGeometryProperty);
            }
        }


        public static DependencyProperty OutlineFadingFactorProperty = DependencyProperty.Register("OutlineFadingFactor", typeof(double), typeof(OutLineMeshGeometryModel3D),
            new PropertyMetadata(1.5, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as MeshOutlineNode).OutlineFadingFactor = (float)(double)e.NewValue;
            }));

        public double OutlineFadingFactor
        {
            set
            {
                SetValue(OutlineFadingFactorProperty, value);
            }
            get
            {
                return (double)GetValue(OutlineFadingFactorProperty);
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new MeshOutlineNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            if (core is MeshOutlineNode c)
            {
                c.OutlineColor = this.OutlineColor.ToColor4();
                c.EnableOutline = this.EnableOutline;
                c.OutlineFadingFactor = (float)this.OutlineFadingFactor;
                c.IsDrawGeometry = this.IsDrawGeometry;
            }

            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}