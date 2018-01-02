// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShadowMap3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.ComponentModel;
    using System.Windows;
    using System.Linq;
    using Media3D = System.Windows.Media.Media3D;
    using global::SharpDX;

    using Utilities;
    using Core;

    public class ShadowMap3D : Element3D
    {
        public static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register("Resolution", typeof(Vector2), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(new Vector2(1024, 1024), (d, e) =>
            {
                var resolution = (Vector2)e.NewValue;
                ((d as ShadowMap3D).RenderCore as ShadowMapCore).Width = (int)resolution.X;
                ((d as ShadowMap3D).RenderCore as ShadowMapCore).Height = (int)resolution.Y;
            }));

        //public static readonly DependencyProperty FactorPCFProperty =
        //        DependencyProperty.Register("FactorPCF", typeof(double), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(1.5, (d,e)=>
        //        {
        //            ((d as ShadowMap3D).RenderCore as ShadowMapCore).FactorPCF = (float)(double)e.NewValue;
        //        }));

        public static readonly DependencyProperty BiasProperty =
                DependencyProperty.Register("Bias", typeof(double), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(0.0015, (d, e)=>
                {
                    ((d as ShadowMap3D).RenderCore as ShadowMapCore).Bias = (float)(double)e.NewValue;
                }));

        public static readonly DependencyProperty IntensityProperty =
                DependencyProperty.Register("Intensity", typeof(double), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(0.5, (d, e)=>
                {
                    ((d as ShadowMap3D).RenderCore as ShadowMapCore).Intensity = (float)(double)e.NewValue;
                }));

        public static readonly DependencyProperty LightCameraProperty =
                DependencyProperty.Register("LightCamera", typeof(ProjectionCamera), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(null, (d, e) =>
                {
                    (d as ShadowMap3D).lightCamera = (ProjectionCamera)e.NewValue;
                }));

        [TypeConverter(typeof(Vector2Converter))]
        public Vector2 Resolution
        {
            get { return (Vector2)this.GetValue(ResolutionProperty); }
            set { this.SetValue(ResolutionProperty, value); }
        }
        /// <summary>
        /// PCF sampling size
        /// </summary>
        //public double FactorPCF
        //{
        //    get { return (double)this.GetValue(FactorPCFProperty); }
        //    set { this.SetValue(FactorPCFProperty, value); }
        //}
        /// <summary>
        /// 
        /// </summary>
        public double Bias
        {
            get { return (double)this.GetValue(BiasProperty); }
            set { this.SetValue(BiasProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public double Intensity
        {
            get { return (double)this.GetValue(IntensityProperty); }
            set { this.SetValue(IntensityProperty, value); }
        }
        /// <summary>
        /// Distance of the directional light from origin
        /// </summary>
        public ProjectionCamera LightCamera
        {
            get { return (ProjectionCamera)this.GetValue(LightCameraProperty); }
            set { this.SetValue(LightCameraProperty, value); }
        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new ShadowMapCore();
        }

        private ShadowMapCore shadowCore;

        private readonly OrthographicCamera orthoCamera = new OrthographicCamera() { NearPlaneDistance = 1, FarPlaneDistance = 500 };
        private readonly PerspectiveCamera persCamera = new PerspectiveCamera() { NearPlaneDistance = 1, FarPlaneDistance = 500 };
        private ProjectionCamera lightCamera;

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            var c = core as ShadowMapCore;
            //c.FactorPCF = (float)FactorPCF;
            c.Intensity = (float)Intensity;
            c.Bias = (float)Bias;
            c.Width = (int)(Resolution.X);
            c.Height = (int)(Resolution.Y);
        }

        protected override bool OnAttach(IRenderHost host)
        {
            base.OnAttach(host);
            shadowCore = RenderCore as ShadowMapCore;            
            return true;
        }

        protected override bool CanRender(IRenderContext context)
        {
            return base.CanRender(context) && renderHost.IsShadowMapEnabled && !context.IsShadowPass;
        }

        protected override void OnRender(IRenderContext context)
        {
            ProjectionCamera camera = lightCamera;
            if (lightCamera == null)
            {
                var root = context.RenderHost.Renderable.Renderables
                    .Where(x => x is ILight3D && (((ILight3D)x).LightType == LightType.Directional || ((ILight3D)x).LightType == LightType.Spot)).Take(1);
                foreach (var light in root)
                {                  
                    if (light is DirectionalLight3D)
                    {
                        var dlight = (DirectionalLight3D)light;
                        var dir = Vector4.Transform(dlight.DirectionInternal.ToVector4(0), dlight.ModelMatrix).Normalized();
                        var pos = -100 * dir;
                        orthoCamera.LookDirection = new Media3D.Vector3D(dir.X, dir.Y, dir.Z);
                        orthoCamera.Position = new Media3D.Point3D(pos.X, pos.Y, pos.Z);
                        orthoCamera.UpDirection = Vector3.UnitZ.ToVector3D();
                        orthoCamera.Width = 50;
                        camera = orthoCamera;
                    }
                    else if (light is SpotLight3D)
                    {
                        var splight = (SpotLight3D)light;
                        persCamera.Position = splight.Position + new Media3D.Vector3D(splight.ModelMatrix.M41, splight.ModelMatrix.M42, splight.ModelMatrix.M43);
                        var look = Vector4.Transform(splight.DirectionInternal.ToVector4(0), splight.ModelMatrix);
                        persCamera.LookDirection = new Media3D.Vector3D(look.X, look.Y, look.Z);
                        persCamera.FarPlaneDistance = splight.Range;
                        persCamera.FieldOfView = splight.OuterAngle;
                        persCamera.UpDirection = Vector3.UnitZ.ToVector3D();
                        camera = persCamera;
                    }
                }
            }
            if (camera == null)
            { return; }
            shadowCore.LightViewProjectMatrix = camera.GetViewMatrix() * camera.GetProjectionMatrix(shadowCore.Width / shadowCore.Height);
            shadowCore.Render(context);
        }
    }
}
