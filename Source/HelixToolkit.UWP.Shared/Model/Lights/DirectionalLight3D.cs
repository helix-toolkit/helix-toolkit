/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
#if WINUI
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
#endif
{
    using Model;
#if !WINUI   
    using Model.Scene;
#endif

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
