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

namespace HelixToolkit.UWP
{
    using global::SharpDX;
    using Model;
    using Model.Scene;
    using Windows.UI.Xaml;

    public abstract class Light3D : Element3D
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Light3D), new PropertyMetadata(Color.Gray, (d,e)=>
            {
                ((d as Element3DCore).SceneNode as LightNode).Color = ((Color)e.NewValue);
            }));

        /// <summary>
        /// Color of the light.
        /// For simplicity, this color applies to the diffuse and specular properties of the light.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }

        public LightType LightType
        {
            get { return (SceneNode as LightNode).LightType; }
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            (core as LightNode).Color = Color.ToColor4();
            base.AssignDefaultValuesToSceneNode(core);          
        }
    }
}
