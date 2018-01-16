// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectionalLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using HelixToolkit.Wpf.SharpDX.Core;
using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf.SharpDX
{
    public sealed class DirectionalLight3D : Light3D
    {
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Vector3D), typeof(Light3D), new PropertyMetadata(new Vector3D(),
                (d, e) => {
                    ((d as IRenderable).RenderCore as DirectionalLightCore).Direction = ((Vector3D)e.NewValue).ToVector3();
                }));

        /// <summary>
        /// Direction of the light.
        /// It applies to Directional Light and to Spot Light,
        /// for all other lights it is ignored.
        /// </summary>
        public Vector3D Direction
        {
            get { return (Vector3D)this.GetValue(DirectionProperty); }
            set { this.SetValue(DirectionProperty, value); }
        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new DirectionalLightCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as DirectionalLightCore).Direction = Direction.ToVector3();
        }
    }
}
