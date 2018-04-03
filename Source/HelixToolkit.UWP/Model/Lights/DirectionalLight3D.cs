/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
namespace HelixToolkit.UWP
{
    using Model;
    using Model.Scene;
    using SharpDX;
    using Windows.UI.Xaml;

    public sealed class DirectionalLight3D : Light3D
    {
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Vector3), typeof(Light3D), new PropertyMetadata(new Vector3(),
                (d, e) => {
                    ((d as Element3DCore).SceneNode as DirectionalLightNode).Direction = ((Vector3)e.NewValue);
                }));

        /// <summary>
        /// Direction of the light.
        /// It applies to Directional Light and to Spot Light,
        /// for all other lights it is ignored.
        /// </summary>
        public Vector3 Direction
        {
            get { return (Vector3)this.GetValue(DirectionProperty); }
            set { this.SetValue(DirectionProperty, value); }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new DirectionalLightNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            base.AssignDefaultValuesToSceneNode(core);
            (core as DirectionalLightNode).Direction = Direction;
        }
    }
}
