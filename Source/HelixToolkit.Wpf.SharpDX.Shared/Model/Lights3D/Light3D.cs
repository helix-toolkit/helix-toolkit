// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Light3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Direction of the light.
//   It applies to Directional Light and to Spot Light,
//   for all other lights it is ignored.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#if COREWPF
using HelixToolkit.SharpDX.Core;
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
    using global::SharpDX;
    using Model;
#if !COREWPF
    using Model.Scene;
#endif
    using System.Windows;
    using System.Windows.Media.Media3D;
    using Media = System.Windows.Media;

    public abstract class Light3D : Element3D
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(Light3D), new PropertyMetadata(Media.Colors.Gray, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as LightNode).Color = ((Media.Color)e.NewValue).ToColor4();
            }));

        /// <summary>
        /// Color of the light.
        /// For simplicity, this color applies to the diffuse and specular properties of the light.
        /// </summary>
        public Media.Color Color
        {
            get
            {
                return (Media.Color)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }

        public LightType LightType
        {
            get
            {
                return (SceneNode as LightNode).LightType;
            }
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            (core as LightNode).Color = Color.ToColor4();
            base.AssignDefaultValuesToSceneNode(core);
        }
    }
}
