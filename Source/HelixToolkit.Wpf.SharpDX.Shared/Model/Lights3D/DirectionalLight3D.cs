// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectionalLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using System.Windows;
using System.Windows.Media.Media3D;

#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
namespace HelixToolkit.Wpf.SharpDX
{
    using Model;
#if !COREWPF
    using Model.Scene;
#endif
    public sealed class DirectionalLight3D : Light3D
    {
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Vector3D), typeof(Light3D), new PropertyMetadata(new Vector3D(),
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as DirectionalLightNode).Direction = ((Vector3D)e.NewValue).ToVector3();
                }));

        /// <summary>
        /// Direction of the light.
        /// It applies to Directional Light and to Spot Light,
        /// for all other lights it is ignored.
        /// </summary>
        public Vector3D Direction
        {
            get
            {
                return (Vector3D)this.GetValue(DirectionProperty);
            }
            set
            {
                this.SetValue(DirectionProperty, value);
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new DirectionalLightNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode core)
        {
            base.AssignDefaultValuesToSceneNode(core);
            (core as DirectionalLightNode).Direction = Direction.ToVector3();
        }
    }
}
