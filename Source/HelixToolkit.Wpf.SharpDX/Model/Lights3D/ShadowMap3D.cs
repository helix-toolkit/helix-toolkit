// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShadowMap3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.ComponentModel;
    using System.Windows;

    using global::SharpDX;

    using global::SharpDX.Direct3D;

    using global::SharpDX.Direct3D11;

    using global::SharpDX.DXGI;

    using HelixToolkit.Wpf.SharpDX.Utilities;

    public class ShadowMap3D : Element3D
    {
        private Texture2D depthBufferSM;
        private Texture2D colorBufferSM;
        private DepthStencilView depthViewSM;
        private RenderTargetView renderTargetSM;
        private ShaderResourceView texShadowMapView;
        private ShaderResourceView texColorMapView;
        private EffectShaderResourceVariable texShadowMapVariable;
        private EffectVectorVariable vShadowMapInfoVariable;
        private EffectVectorVariable vShadowMapSizeVariable;
        private RenderContext shadowPassContext;
        //private int faktor = 1;
        //private int oneK = 1024;
        private int width = 1024, height = 1024;

        public static readonly DependencyProperty ResolutionProperty =
            DependencyProperty.Register("Resolution", typeof(Vector2), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(new Vector2(1024, 1024), ResolutionChanged));

        private static void ResolutionChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {            
            var obj = (ShadowMap3D)d;
            if (obj.IsAttached)
            {
                obj.Detach();
                obj.Attach(obj.renderHost);
            }
        }

        public static readonly DependencyProperty FactorPCFProperty =
                DependencyProperty.Register("FactorPCF", typeof(double), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(1.5));

        public static readonly DependencyProperty BiasProperty =
                DependencyProperty.Register("Bias", typeof(double), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(0.0015));

        public static readonly DependencyProperty IntensityProperty =
                DependencyProperty.Register("Intensity", typeof(double), typeof(ShadowMap3D), new AffectsRenderPropertyMetadata(0.5));


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

        protected override RenderTechnique SetRenderTechnique(IRenderHost host)
        {
            return host.RenderTechniquesManager.RenderTechniques[DefaultRenderTechniqueNames.Colors];
        }

        protected override bool OnAttach(IRenderHost host)
        {
            this.width = (int)(Resolution.X + 0.5f); //faktor* oneK;
            this.height = (int)(this.Resolution.Y + 0.5f); // faktor* oneK;

            if (!host.IsShadowMapEnabled)
            {
                return false;
            }

            // gen shadow map
            this.depthBufferSM = new Texture2D(Device, new Texture2DDescription()
            {
                Format = Format.R32_Typeless, //!!!! because of depth and shader resource
                //Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = width,
                Height = height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource, //!!!!
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            });

            this.colorBufferSM = new Texture2D(this.Device, new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            });

            this.renderTargetSM = new RenderTargetView(this.Device, colorBufferSM)
            {
            };

            this.depthViewSM = new DepthStencilView(Device, depthBufferSM, new DepthStencilViewDescription()
            {
                Format = Format.D32_Float,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource()
                {
                    MipSlice = 0
                }
            });

            this.texShadowMapView = new ShaderResourceView(Device, depthBufferSM, new ShaderResourceViewDescription()
            {
                Format = Format.R32_Float,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                {
                    MipLevels = 1,
                    MostDetailedMip = 0,
                }
            }); //!!!!

            this.texColorMapView = new ShaderResourceView(this.Device, colorBufferSM, new ShaderResourceViewDescription()
            {
                Format = Format.B8G8R8A8_UNorm,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                {
                    MipLevels = 1,
                    MostDetailedMip = 0,
                }
            });

            this.texShadowMapVariable = effect.GetVariableByName("texShadowMap").AsShaderResource();
            this.vShadowMapInfoVariable = effect.GetVariableByName("vShadowMapInfo").AsVector();
            this.vShadowMapSizeVariable = effect.GetVariableByName("vShadowMapSize").AsVector();
            this.shadowPassContext = new RenderContext(host, this.effect, Device.ImmediateContext);
            return true;
        }

        protected override void OnDetach()
        {
            Disposer.RemoveAndDispose(ref this.depthBufferSM);
            Disposer.RemoveAndDispose(ref this.depthViewSM);
            Disposer.RemoveAndDispose(ref this.colorBufferSM);

            Disposer.RemoveAndDispose(ref this.texColorMapView);
            Disposer.RemoveAndDispose(ref this.texShadowMapView);  

            Disposer.RemoveAndDispose(ref this.texShadowMapVariable);
            Disposer.RemoveAndDispose(ref this.vShadowMapInfoVariable);
            Disposer.RemoveAndDispose(ref this.vShadowMapSizeVariable);

            Disposer.RemoveAndDispose(ref this.shadowPassContext);
            //this.renderHost.IsShadowMapEnabled = false;            
            base.OnDetach();
        }

        protected override bool CanRender(RenderContext context)
        {
            if (base.CanRender(context))
            {
                if (!this.renderHost.IsShadowMapEnabled)
                {
                    return false;
                }
                return true;
            }
            return false;
        }
        protected override void OnRender(RenderContext context)
        {
            // --- set rasterizes state here with proper shadow-bias, as depth-bias and slope-bias in the rasterizer            
            this.Device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);
            this.Device.ImmediateContext.OutputMerger.SetTargets(depthViewSM);            
            this.Device.ImmediateContext.ClearDepthStencilView(depthViewSM, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            var root = context.Canvas.Renderable.Renderables;
            foreach (var item in root)
            {
                var light = item as Light3D;
                if (light != null)
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
                        var pos = -50 * light.DirectionInternal;
                        
                        //lightCamera = new PerspectiveCamera()
                        //{
                        //    LookDirection = dir.ToVector3D(),
                        //    Position = (System.Windows.Media.Media3D.Point3D)(pos.ToVector3D()),
                        //    UpDirection = Vector3.UnitZ.ToVector3D(),                            
                        //    NearPlaneDistance = 1,
                        //    FarPlaneDistance = 100,
                        //    FieldOfView = 10,
                        //};

                        lightCamera = new OrthographicCamera()
                        {
                            LookDirection = dir.ToVector3D(),
                            Position = (System.Windows.Media.Media3D.Point3D)(pos.ToVector3D()),                            
                            UpDirection = Vector3.UnitZ.ToVector3D(),
                            Width = 100,
                            NearPlaneDistance = 1,
                            FarPlaneDistance = 500,
                        };
                    }

                    if (lightCamera != null)
                    {
                        var sceneCamera = context.Camera;
                        
                        light.LightViewMatrix = CameraExtensions.GetViewMatrix(lightCamera);
                        light.LightProjectionMatrix = CameraExtensions.GetProjectionMatrix(lightCamera, context.Canvas.ActualWidth / context.Canvas.ActualHeight);

                        this.shadowPassContext.IsShadowPass = true;
                        this.shadowPassContext.Camera = lightCamera;
                        foreach (var e in root)
                        {
                            var smodel = e as IThrowingShadow;
                            if (smodel != null)
                            {
                                if (smodel.IsThrowingShadow)
                                {
                                    var model = smodel as IRenderable;
                                    model.Render(this.shadowPassContext);
                                }
                            }
                        }
                        context.Camera = sceneCamera;
                    }
                }
            }

            this.texShadowMapVariable.SetResource(this.texShadowMapView);            
            //this.texShadowMapVariable.SetResource(this.texColorMapView);            
            this.vShadowMapInfoVariable.Set(new Vector4((float)this.Intensity, (float)this.FactorPCF, (float)this.Bias, 0));
            this.vShadowMapSizeVariable.Set(new Vector2(width, height));

            //System.Console.WriteLine("ShadowMap rendered!");
            context.Canvas.SetDefaultRenderTargets();
        }
    }
}
