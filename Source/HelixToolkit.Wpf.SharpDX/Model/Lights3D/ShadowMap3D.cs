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

    using global::SharpDX;

    using global::SharpDX.Direct3D;

    using global::SharpDX.Direct3D11;

    using global::SharpDX.DXGI;

    using HelixToolkit.Wpf.SharpDX.Utilities;
    using Core;
    using Model;
    using System;

    public class ShadowMap3D : Element3D
    {
        public static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register("Resolution", typeof(Vector2), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(new Vector2(1024, 1024), (d, e) =>
            {
                var resolution = (Vector2)e.NewValue;
                ((d as ShadowMap3D).RenderCore as ShadowMapCore).Width = (int)resolution.X;
                ((d as ShadowMap3D).RenderCore as ShadowMapCore).Height = (int)resolution.Y;
            }));

        public static readonly DependencyProperty FactorPCFProperty =
                DependencyProperty.Register("FactorPCF", typeof(double), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(1.5, (d,e)=>
                {
                    ((d as ShadowMap3D).RenderCore as ShadowMapCore).FactorPCF = (float)(double)e.NewValue;
                }));

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

        [TypeConverter(typeof(Vector2Converter))]
        public Vector2 Resolution
        {
            get { return (Vector2)this.GetValue(ResolutionProperty); }
            set { this.SetValue(ResolutionProperty, value); }
        }

        public double FactorPCF
        {
            get { return (double)this.GetValue(FactorPCFProperty); }
            set { this.SetValue(FactorPCFProperty, value); }
        }

        public double Bias
        {
            get { return (double)this.GetValue(BiasProperty); }
            set { this.SetValue(BiasProperty, value); }
        }

        public double Intensity
        {
            get { return (double)this.GetValue(IntensityProperty); }
            set { this.SetValue(IntensityProperty, value); }
        }

        public Light3DSceneShared Light3DSceneShared
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override IRenderCore OnCreateRenderCore()
        {
            return new ShadowMapCore();
        }

        private ShadowMapCore shadowCore;

        protected override void AssignDefaultValuesToCore(IRenderCore core)
        {
            base.AssignDefaultValuesToCore(core);
            var c = core as ShadowMapCore;
            c.FactorPCF = (float)FactorPCF;
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
            return base.CanRender(context) && renderHost.IsShadowMapEnabled;
        }
        protected override void OnRender(IRenderContext context)
        {
            var root = context.RenderHost.Renderable.Renderables.Where(x=>x is Light3D).Select(x=>(Light3D)x);
            foreach (var light in root)
            {
                Camera lightCamera = null;
                //if (light is PointLightBase3D)
                //{
                //    var plight = (PointLightBase3D)light;
                //    lightCamera = new PerspectiveCamera()
                //    {
                //        Position = plight.Position,
                //        LookDirection = plight.Direction,
                //        UpDirection = Vector3.UnitY.ToVector3D(),
                //    };                        
                //}
                // else 
                if (light is DirectionalLight3D)
                {
                    var dlight = (DirectionalLight3D)light;
                    var dir = light.DirectionInternal.Normalized();
                    var pos = -100 * dir;                       

                    lightCamera = new OrthographicCamera()
                    {
                        LookDirection = dir.ToVector3D(),
                        Position = (System.Windows.Media.Media3D.Point3D)(pos.ToVector3D()),                            
                        UpDirection = Vector3.UnitZ.ToVector3D(),
                        Width = 25,
                        NearPlaneDistance = 1,
                        FarPlaneDistance = 500,
                    };
                    shadowCore.LightViewProjectMatrix = lightCamera.GetViewMatrix() * lightCamera.GetProjectionMatrix(shadowCore.Width/shadowCore.Height);
                    shadowCore.Render(context);
                }
            }
        }
    }
}
