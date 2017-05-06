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

    using global::SharpDX;

    using global::SharpDX.Direct3D11;

    using HelixToolkit.Wpf.SharpDX.Utilities;

    using Matrix = global::SharpDX.Matrix;
    using Model.Lights3D;

    public enum LightType : ushort
    {
        Ambient = 0, Directional = 1, Point = 2, Spot = 3,
    }

    public interface ILight3D
    {
        Light3DSceneShared Light3DSceneShared { get; }
    }

    public abstract class Light3D : Model3D, ILight3D, IDisposable
    {
        public Light3DSceneShared Light3DSceneShared { private set; get; }
        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Vector3), typeof(Light3D), new AffectsRenderPropertyMetadata(new Vector3(), 
                (d,e)=> {
                    (d as Light3D).DirectionInternal = (Vector3)e.NewValue;
                }));

        public static readonly DependencyProperty DirectionTransformProperty =
            DependencyProperty.Register("DirectionTransform", typeof(Transform3D), typeof(Light3D), new AffectsRenderPropertyMetadata(Transform3D.Identity, DirectionTransformPropertyChanged));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color4), typeof(Light3D), new AffectsRenderPropertyMetadata(new Color4(0.2f, 0.2f, 0.2f, 1.0f), ColorChanged));


        public LightType LightType { get; protected set; }


        public Light3D()
        {
        }

        ~Light3D()
        {

        }


        private static void ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Light3D)d).ColorInternal = (Color4)e.NewValue;
            ((Light3D)d).OnColorChanged(e);
        }

        protected virtual void OnColorChanged(DependencyPropertyChangedEventArgs e) { }

        /// <summary>
        /// Direction of the light.
        /// It applies to Directional Light and to Spot Light,
        /// for all other lights it is ignored.
        /// </summary>
        [TypeConverter(typeof(Vector3Converter))]
        public Vector3 Direction
        {
            get { return (Vector3)this.GetValue(DirectionProperty); }
            set { this.SetValue(DirectionProperty, value); }
        }
        internal Vector3 DirectionInternal { private set; get; }
        /// <summary>
        /// Transforms the Direction Vector of the Light.
        /// </summary>
        public Transform3D DirectionTransform
        {
            get { return (Transform3D)this.GetValue(DirectionTransformProperty); }
            set { this.SetValue(DirectionTransformProperty, value); }
        }
        internal Transform3D DirectionTransformInternal { private set; get; } = Transform3D.Identity;
        /// <summary>
        /// Color of the light.
        /// For simplicity, this color applies to the diffuse and specular properties of the light.
        /// </summary>
        [TypeConverter(typeof(Color4Converter))]
        public Color4 Color
        {
            get { return (Color4)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        internal Color4 ColorInternal { private set; get; } = new Color4(0.2f, 0.2f, 0.2f, 1.0f);

        /// <summary>
        /// 
        /// </summary>
        private static void DirectionTransformPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var light = d as Light3D;
            light.DirectionTransformInternal = e.NewValue == null ? null : (Transform3D)e.NewValue;
            if (light.DirectionTransformInternal != null)
            {
                var trafo = light.DirectionTransformInternal.Value;
                light.Direction = new Vector3((float)trafo.OffsetX, (float)trafo.OffsetY, (float)trafo.OffsetZ);
            }
        }

        /// <summary>
        /// The lighting model.
        /// </summary>
        //public static class Model
        //{
        //    //PhongPerVertex,
        //    //BlinnPhongPerVertex,
        //    public static readonly RenderTechnique RenderPhong = Techniques.RenderPhong;
        //    public static readonly RenderTechnique RenderBlinnPhong = Techniques.RenderBlinn;
        //    public static readonly RenderTechnique RenderColors = Techniques.RenderColors;
        //}

        public Matrix LightViewMatrix
        {
            get { return Light3DSceneShared.LightViewMatrices[this.lightIndex]; }
            internal set { Light3DSceneShared.LightViewMatrices[this.lightIndex] = value; }
        }

        public Matrix LightProjectionMatrix
        {
            get { return Light3DSceneShared.LightProjMatrices[this.lightIndex]; }
            internal set { Light3DSceneShared.LightProjMatrices[this.lightIndex] = value; }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            Light3DSceneShared = host.Light3DSceneShared;
            if (this.LightType != LightType.Ambient)
            {
                this.lightIndex = host.Light3DSceneShared.LightCount++;
                host.Light3DSceneShared.LightCount = host.Light3DSceneShared.LightCount % Light3DSceneShared.MaxLights;

                if (host.IsShadowMapEnabled)
                {
                    this.mLightView = this.effect.GetVariableByName("mLightView").AsMatrix();
                    this.mLightProj = this.effect.GetVariableByName("mLightProj").AsMatrix();
                }
            }
            return true;
        }

        protected override void OnDetach()
        {
            if (this.LightType != LightType.Ambient && Light3DSceneShared != null)
            {
                // "turn-off" the light
                Light3DSceneShared.LightColors[lightIndex] = NoLight;
            }
            base.OnDetach();
        }

        protected override void OnRender(RenderContext context)
        {
            
        }

        protected static readonly Color4 NoLight = new Color4(0,0,0,0);
        protected override bool CanRender(RenderContext context)
        {
            if (!base.CanRender(context))
            {
                Light3DSceneShared.LightColors[lightIndex] = NoLight;
                this.vLightColor.Set(Light3DSceneShared.LightColors);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void Dispose()
        {
            this.Detach();
        }

        protected EffectVectorVariable vLightDir;
        protected EffectVectorVariable vLightPos;
        protected EffectVectorVariable vLightColor;
        protected EffectVectorVariable vLightAtt;
        protected EffectVectorVariable vLightSpot;
        protected EffectMatrixVariable mLightView;
        protected EffectMatrixVariable mLightProj;
        protected EffectScalarVariable iLightType;
        protected int lightIndex = 0;
    }

    public abstract class PointLightBase3D : Light3D
    {
        public static readonly DependencyProperty AttenuationProperty =
            DependencyProperty.Register("Attenuation", typeof(Vector3), typeof(PointLightBase3D), new AffectsRenderPropertyMetadata(new Vector3(1.0f, 0.0f, 0.0f),
                (d, e) => {
                    (d as PointLightBase3D).AttenuationInternal = (Vector3)e.NewValue;
                }));

        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(double), typeof(PointLightBase3D), new AffectsRenderPropertyMetadata(1000.0,
                (d, e) => {
                    (d as PointLightBase3D).RangeInternal = (double)e.NewValue;
                }));

        /// <summary>
        /// The position of the model in world space.
        /// </summary>
        public Point3D Position
        {
            get { return (Point3D)this.GetValue(PositionProperty); }
            private set { this.SetValue(PositionPropertyKey, value); }
        }

        internal Point3D PositionInternal { private set; get; }

        private static readonly DependencyPropertyKey PositionPropertyKey =
            DependencyProperty.RegisterReadOnly("Position", typeof(Point3D), typeof(PointLightBase3D), new AffectsRenderPropertyMetadata(new Point3D(),
                (d,e)=> {
                    (d as PointLightBase3D).PositionInternal = (Point3D)e.NewValue;
                }));

        public static readonly DependencyProperty PositionProperty = PositionPropertyKey.DependencyProperty;

        protected override void OnTransformChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnTransformChanged(e);
            this.Position = this.modelMatrix.TranslationVector.ToPoint3D();
        }

        /// <summary>
        /// Attenuation coefficients:
        /// X = constant attenuation,
        /// Y = linar attenuation,
        /// Z = quadratic attenuation.
        /// For details see: http://msdn.microsoft.com/en-us/library/windows/desktop/bb172279(v=vs.85).aspx
        /// </summary>
        public Vector3 Attenuation
        {
            get { return (Vector3)this.GetValue(AttenuationProperty); }
            set { this.SetValue(AttenuationProperty, value); }
        }
        internal Vector3 AttenuationInternal { private set; get; } = new Vector3(1.0f, 0.0f, 0.0f);
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

        internal double RangeInternal { private set; get; } = 1000;
    }
}
