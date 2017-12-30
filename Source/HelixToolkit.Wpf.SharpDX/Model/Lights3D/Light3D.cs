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

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Media.Media3D;
    using Media = System.Windows.Media;
    using global::SharpDX;

    using Utilities;
    using Model;

    public enum LightType : ushort
    {
        Ambient = 0, Directional = 1, Point = 2, Spot = 3, ThreePoint = 4, None = 5
    }

    public interface ILight3D
    {
        LightType LightType
        {
            get;
        }
        Light3DSceneShared Light3DSceneShared { get; }
    }

    public abstract class Light3D : Model3D, ILight3D, IDisposable
    {
        public Light3DSceneShared Light3DSceneShared { private set; get; }
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Vector3D), typeof(Light3D), new AffectsRenderPropertyMetadata(new Vector3D(), 
                (d,e)=> {
                    (d as Light3D).DirectionInternal = ((Vector3D)e.NewValue).ToVector3();
                }));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Media.Color), typeof(Light3D), new AffectsRenderPropertyMetadata(Media.Colors.Gray, (d,e)=>
            {
                ((Light3D)d).ColorInternal = ((Media.Color)e.NewValue).ToColor4();
                ((Light3D)d).OnColorChanged(e);
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
        /// Color of the light.
        /// For simplicity, this color applies to the diffuse and specular properties of the light.
        /// </summary>
        public Media.Color Color
        {
            get { return (Media.Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }


        public LightType LightType { get; protected set; }

        internal Vector3 DirectionInternal { private set; get; }

        internal Color4 ColorInternal { private set; get; } = new Color4(0.2f, 0.2f, 0.2f, 1.0f);

        protected int lightIndex { private set; get; }

        protected virtual void OnColorChanged(DependencyPropertyChangedEventArgs e) { }

        protected override bool OnAttach(IRenderHost host)
        {
            Light3DSceneShared = host.Light3DSceneShared;
            if (this.LightType != LightType.Ambient)
            {
                this.lightIndex = host.Light3DSceneShared.LightCount++;
                host.Light3DSceneShared.LightCount = host.Light3DSceneShared.LightCount % LightsBufferModel.MaxLights;
            }
            return true;
        }

        protected override void OnDetach()
        {
            if (this.LightType != LightType.Ambient && Light3DSceneShared != null)
            {
                // "turn-off" the light
                Light3DSceneShared.LightModels.Lights[lightIndex].LightEnabled = 0;
            }
            base.OnDetach();
        }

        protected override bool CanRender(IRenderContext context)
        {
            var render = base.CanRender(context) && !renderHost.IsDeferredLighting;
            Light3DSceneShared.LightModels.Lights[lightIndex].LightEnabled = render ? 1 : 0;
            return render;
        }
    }

    public abstract class PointLightBase3D : Light3D
    {
        public static readonly DependencyProperty AttenuationProperty =
            DependencyProperty.Register("Attenuation", typeof(Vector3D), typeof(PointLightBase3D), new AffectsRenderPropertyMetadata(new Vector3D(1.0f, 0.0f, 0.0f),
                (d, e) => {
                    (d as PointLightBase3D).AttenuationInternal = ((Vector3D)e.NewValue).ToVector3();
                }));

        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(double), typeof(PointLightBase3D), new AffectsRenderPropertyMetadata(1000.0,
                (d, e) => {
                    (d as PointLightBase3D).RangeInternal = (double)e.NewValue;
                }));

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register("Position", typeof(Point3D), typeof(PointLightBase3D), new AffectsRenderPropertyMetadata(new Point3D(),
                (d,e)=> {
                    (d as PointLightBase3D).PositionInternal = ((Point3D)e.NewValue).ToVector3();
                }));

        /// <summary>
        /// The position of the model in world space.
        /// </summary>
        public Point3D Position
        {
            get { return (Point3D)this.GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }

        /// <summary>
        /// Attenuation coefficients:
        /// X = constant attenuation,
        /// Y = linar attenuation,
        /// Z = quadratic attenuation.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx
        /// </summary>
        public Vector3D Attenuation
        {
            get { return (Vector3D)this.GetValue(AttenuationProperty); }
            set { this.SetValue(AttenuationProperty, value); }
        }

        /// <summary>
        /// Range of this light. This is the maximum distance 
        /// of a pixel being lit by this light.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx
        /// </summary>
        public double Range
        {
            get { return (double)this.GetValue(RangeProperty); }
            set { this.SetValue(RangeProperty, value); }
        }

        internal Vector3 PositionInternal { private set; get; }
        internal Vector3 AttenuationInternal { private set; get; } = new Vector3(1.0f, 0.0f, 0.0f);
        internal double RangeInternal { private set; get; } = 1000;
    }
}
