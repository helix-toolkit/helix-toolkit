/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace HelixToolkit.UWP
{
    using global::SharpDX;
    using Model;
    using Model.Scene;
    using Windows.UI.Xaml;
    using Color = Windows.UI.Color;
    using Colors = Windows.UI.Colors;

    public abstract class Light3D : Element3D
    {
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(Light3D), new PropertyMetadata(Colors.Gray, (d,e)=>
            {
                ((d as Element3DCore).SceneNode as LightNode).Color = ((Color)e.NewValue).ToColor4();
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
