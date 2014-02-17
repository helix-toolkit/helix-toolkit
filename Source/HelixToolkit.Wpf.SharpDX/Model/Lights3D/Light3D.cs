namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using global::SharpDX;

    using global::SharpDX.Direct3D11;

    using Matrix = global::SharpDX.Matrix;

    public enum LightType : ushort
    {
        Ambient = 1, Directional = 2, Point = 3, Spot = 4,
    }

    public interface ILight3D
    {
    }

    public abstract class Light3D : Model3D, ILight3D, IDisposable
    {

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Vector3), typeof(Light3D), new UIPropertyMetadata(new Vector3()));

        public static readonly DependencyProperty DirectionTransformProperty =
            DependencyProperty.Register("DirectionTransform", typeof(Transform3D), typeof(Light3D), new UIPropertyMetadata(Transform3D.Identity, DirectionTransformPropertyChanged));

        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color4), typeof(Light3D), new UIPropertyMetadata(new Color4(0.2f, 0.2f, 0.2f, 1.0f), ColorChanged));


        public LightType LightType { get; protected set; }


        public Light3D()
        {
        }

        ~Light3D()
        {

        }


        private static void ColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Light3D)d).OnColorChanged(e);
        }

        protected virtual void OnColorChanged(DependencyPropertyChangedEventArgs e) { }

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

        /// <summary>
        /// Transforms the Direction Vector of the Light.
        /// </summary>
        public Transform3D DirectionTransform
        {
            get { return (Transform3D)this.GetValue(DirectionTransformProperty); }
            set { this.SetValue(DirectionTransformProperty, value); }
        }

        /// <summary>
        /// Color of the light.
        /// For simplicity, this color applies to the diffuse and specular properties of the light.
        /// </summary>
        public Color4 Color
        {
            get { return (Color4)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private static void DirectionTransformPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                var trafo = ((Transform3D)e.NewValue).Value;
                ((Light3D)d).Direction = new Vector3((float)trafo.OffsetX, (float)trafo.OffsetY, (float)trafo.OffsetZ);
            }
        }

        /// <summary>
        /// Number of lights in the scene
        /// </summary>
        public static int LightCount
        {
            get { return lightCount; }
            internal set
            {
                lightCount = value;
                if (value == 0)
                {
                    lightDirections = new Vector4[maxLights];
                    lightPositions = new Vector4[maxLights];
                    lightAtt = new Vector4[maxLights];
                    lightSpots = new Vector4[maxLights];
                    lightColors = new Color4[maxLights];
                    lightTypes = new int[maxLights];
                    lightViewMatrices = new Matrix[maxLights];
                    lightProjMatrices = new Matrix[maxLights];
                }
            }
        }

        /// <summary>
        /// The lighting model.
        /// </summary>
        public static class Model
        {
            //PhongPerVertex,
            //BlinnPhongPerVertex,
            public static readonly RenderTechnique RenderPhong = Techniques.RenderPhong;
            public static readonly RenderTechnique RenderBlinnPhong = Techniques.RenderBlinn;
            public static readonly RenderTechnique RenderColors = Techniques.RenderColors;
        }

        public Matrix LightViewMatrix
        {
            get { return lightViewMatrices[this.lightIndex]; }
            internal set { lightViewMatrices[this.lightIndex] = value; }
        }

        public Matrix LightProjectionMatrix
        {
            get { return lightProjMatrices[this.lightIndex]; }
            internal set { lightProjMatrices[this.lightIndex] = value; }
        }

        /// <summary>
        /// Light Type.
        /// </summary>
        public enum Type : int
        {
            Ambient = 0,
            Directional = 1,
            Point = 2,
            Spot = 3,
        }

        public override void Attach(IRenderHost host)
        {
            this.renderTechnique = host.RenderTechnique;
            base.Attach(host);

            if (this.LightType != LightType.Ambient)
            {
                this.lightIndex = lightCount++;
                lightCount = lightCount % maxLights;

                if (host.IsShadowMapEnabled)
                {
                    this.mLightView = this.effect.GetVariableByName("mLightView").AsMatrix();
                    this.mLightProj = this.effect.GetVariableByName("mLightProj").AsMatrix();
                }
            }
        }

        public override void Detach()
        {
            if (this.LightType != LightType.Ambient)
            {
                // "turn-off" the light
                lightColors[lightIndex] = new Color4(0, 0, 0, 0);
            }
            base.Detach();
        }

        public override void Dispose()
        {
            this.Detach();
        }

        protected const int maxLights = 16;
        protected static int lightCount = 0;
        protected static Vector4[] lightDirections = new Vector4[maxLights];
        protected static Vector4[] lightPositions = new Vector4[maxLights];
        protected static Vector4[] lightAtt = new Vector4[maxLights];
        protected static Vector4[] lightSpots = new Vector4[maxLights];
        protected static Color4[] lightColors = new Color4[maxLights];
        protected static int[] lightTypes = new int[maxLights];
        protected static Matrix[] lightViewMatrices = new Matrix[maxLights];
        protected static Matrix[] lightProjMatrices = new Matrix[maxLights];

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
            DependencyProperty.Register("Attenuation", typeof(Vector3), typeof(PointLightBase3D), new UIPropertyMetadata(new Vector3(1.0f, 0.0f, 0.0f)));

        public static readonly DependencyProperty RangeProperty =
            DependencyProperty.Register("Range", typeof(double), typeof(PointLightBase3D), new UIPropertyMetadata(1000.0));

        /// <summary>
        /// The position of the model in world space.
        /// </summary>
        public Point3D Position
        {
            get { return (Point3D)this.GetValue(PositionProperty); }
            private set { this.SetValue(PositionPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey PositionPropertyKey =
            DependencyProperty.RegisterReadOnly("Position", typeof(Point3D), typeof(PointLightBase3D), new UIPropertyMetadata(new Point3D()));

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
    }
}