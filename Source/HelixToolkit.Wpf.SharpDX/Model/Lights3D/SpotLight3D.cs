// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SpotLight3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Decay Exponent of the spotlight.
//   The falloff the spotlight between inner and outer angle
//   depends on this value.
//   For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using HelixToolkit.Wpf.SharpDX.Core;
    using System.Windows;
    using System.Windows.Media.Media3D;

    public sealed class SpotLight3D : PointLight3D
    {
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Vector3D), typeof(SpotLight3D), new PropertyMetadata(new Vector3D(),
                (d, e) => {
                    ((d as IRenderable).RenderCore as SpotLightCore).Direction = ((Vector3D)e.NewValue).ToVector3();
                }));

        public static readonly DependencyProperty FalloffProperty =
            DependencyProperty.Register("Falloff", typeof(double), typeof(SpotLight3D), new PropertyMetadata(1.0,
                (d,e)=> {
                    ((d as IRenderable).RenderCore as SpotLightCore).FallOff = (float)(double)e.NewValue;
                }));

        public static readonly DependencyProperty InnerAngleProperty =
            DependencyProperty.Register("InnerAngle", typeof(double), typeof(SpotLight3D), new PropertyMetadata(5.0,
                (d, e) => {
                    ((d as IRenderable).RenderCore as SpotLightCore).InnerAngle = (float)(double)e.NewValue;
                }));

        public static readonly DependencyProperty OuterAngleProperty =
            DependencyProperty.Register("OuterAngle", typeof(double), typeof(SpotLight3D), new PropertyMetadata(45.0,
                (d, e) => {
                    ((d as IRenderable).RenderCore as SpotLightCore).OuterAngle = (float)(double)e.NewValue;
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
        /// <summary>
        /// Decay Exponent of the spotlight.
        /// The falloff the spotlight between inner and outer angle
        /// depends on this value.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
        /// </summary>
        public double Falloff
        {
            get { return (double)this.GetValue(FalloffProperty); }
            set { this.SetValue(FalloffProperty, value); }
        }

        /// <summary>
        /// Full outer angle of the spot (Phi) in degrees
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
        /// </summary>
        public double OuterAngle
        {
            get { return (double)this.GetValue(OuterAngleProperty); }
            set { this.SetValue(OuterAngleProperty, value); }
        }

        /// <summary>
        /// Full inner angle of the spot (Theta) in degrees. 
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb174697(v=vs.85).aspx
        /// </summary>
        public double InnerAngle
        {
            get { return (double)this.GetValue(InnerAngleProperty); }
            set { this.SetValue(InnerAngleProperty, value); }
        }   


        protected override IRenderCore OnCreateRenderCore()
        {
            return new SpotLightCore();
        }

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            (core as SpotLightCore).Direction = Direction.ToVector3();
            (core as SpotLightCore).InnerAngle = (float)InnerAngle;
            (core as SpotLightCore).OuterAngle = (float)OuterAngle;
            (core as SpotLightCore).FallOff = (float)Falloff;
        }
    }
}
