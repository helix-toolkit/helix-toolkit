// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeferredRenderer.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Number Samples Propery can be set directly to the deferred renderer
//   It couses a re-init of the renderer
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using global::SharpDX;
    using global::SharpDX.Direct3D;
    using global::SharpDX.Direct3D11;
    using global::SharpDX.DXGI;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Buffer = global::SharpDX.Direct3D11.Buffer;
    using Device = global::SharpDX.Direct3D11.Device;

#if DEFERRED
#if SSAO
    public static class DeferredRenderPasses
    {
        public static string RenderDirectDiffuse = "Direct Diffuse";
        public static string RenderBluredDiffuse = "Blured Diffuse";
        public static string RenderDirect = "Direct";
        public static string RenderBlured = "Blured";

        public static IEnumerable<string> RenderPasses { get { return renderPasses; } }

        private static List<string> renderPasses = new List<string>
        { 
            RenderDirectDiffuse,
            RenderBluredDiffuse,
            RenderDirect,
            RenderBlured,
        };

        public static string Blur4x4 = "Blur 4x4";
        public static string BlurSeparableGaussian = "Blur Separable Gaussian";
        public static string BlurCrossBilateral = "Blur Cross Bilteral";

        public static IEnumerable<string> BlurPasses { get { return blurPasses; } }

        private static List<string> blurPasses = new List<string>
        {
            Blur4x4,
            BlurSeparableGaussian,
            BlurCrossBilateral
        };
    }
#endif

    public class DeferredRenderer : IDisposable
    {        
        ///  G-Buffer constants        
        private const int NUMSUBSAMPLES = 4;
        private const int NUMGBUFFER = 4;
        private const int NUMSSBUFFER = 3;

        private Device device;
        private IRenderHost renderHost;
        private Format format;
        private int targetHeight, targetWidth, numberSamplesMSAA = 4;

        private Texture2D depthStencilBuffer;        
        private DeferredLightingVariables deferredLightingVariables;

        private LightGeometryData screenQuad;
        private LightGeometryData screenSphere;
        private LightGeometryData screenCone;

        private Color4 ambientLightColor = new Color4(0.2f, 0.2f, 0.2f, 1.0f);
        
#if SSAO
        private Texture2D[] gBuffer;
        private RenderTargetView[] gBufferRenderTargetView;
        private ShaderResourceView[] gBufferShaderResourceView; 

        private Texture2D[] ssBuffer;
        private RenderTargetView[] ssBufferRenderTargetView;
        private ShaderResourceView[] ssBufferShaderResourceView;
        
        private DepthStencilView depthStencilBufferView;
        private ShaderResourceView gBufferDepthStencilResourceView;

        private ScreenSpaceProcessingVariables screenSpaceVariables;
        private ShaderResourceView randNormalMapShaderResourceView;

        public bool RenderSSAO { get; set; }
        public string RenderPass { get; set; }
        public string BlurPass { get; set; }
        public bool FXAAEnabled { get; set; }
        public double DoubleProp1 { get; set; }
#else
        private Texture2D[] gBuffer;
        private RenderTargetView[] gBufferRenderTargetView;
        private ShaderResourceView[] gBufferShaderResourceView;
        private DepthStencilView depthStencilBufferView;
        private ShaderResourceView gBufferDepthStencilResourceView;
#endif

        /// <summary>
        /// Number Samples Propery can be set directly to the deferred renderer
        /// It couses a re-init of the renderer
        /// </summary>
        public int NumberSamplesMSAA
        {
            get
            {
                return this.numberSamplesMSAA;
            }
            set
            {
                this.numberSamplesMSAA = value;
                this.InitBuffers(this.renderHost, this.format);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DeferredRenderer()
        {
#if SSAO
            this.RenderPass = DeferredRenderPasses.RenderDirect;
            this.BlurPass = DeferredRenderPasses.Blur4x4;
            this.RenderSSAO = false;
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="format"></param>
        internal void InitBuffers(IRenderHost host, Format format = Format.R32G32B32A32_Float)
        {
            // set class variables
            this.renderHost = host;
            this.device = host.Device;
            this.format = format;
            this.targetWidth = Math.Max((int)host.ActualWidth, 100);
            this.targetHeight = Math.Max((int)host.ActualHeight, 100);

            // variable containers
            this.deferredLightingVariables = new DeferredLightingVariables(renderHost.EffectsManager, renderHost.RenderTechniquesManager);
            this.deferredLightingVariables.vBackgroundColor.Set(this.renderHost.ClearColor);

            // clear old buffers
            this.ClearGBuffer();
            this.screenQuad.Dispose();
            this.screenSphere.Dispose();
            this.screenCone.Dispose();

            // create new buffers    
            this.InitGBuffer(format);
            this.InitQuadBuffer();
            this.InitSphereBuffer();
            this.InitConeBuffer();

#if SSAO
            // variable containers
            this.screenSpaceVariables = new ScreenSpaceProcessingVariables(renderHost.EffectsManager, renderHost.RenderTechniquesManager);

            // clear old buffers
            this.ClearSSBuffer();

            // create new buffers   
            this.InitSSBuffer(format);
#endif

            // flush
            this.device.ImmediateContext.Flush();
        }

        /// <summary>
        /// This render-function is use to directly display the 4 G-Buffer textures in one screen. 
        /// It does not compute any lighting, it just copies the (resized) buffer-textures into the back-buffer. 
        /// It is called per frame. 
        /// </summary>
        internal void RenderGBufferOutput(RenderContext context, ref Texture2D renderTarget, bool merge = false)
        {
            var midX = this.targetWidth / 2;
            var midY = this.targetHeight / 2;

            if (merge)
            {
                context.DeviceContext.CopySubresourceRegion(this.gBuffer[0], 0,
                    new ResourceRegion(0, 0, 0, midX, midY, 1),
                    renderTarget, 0, 0, 0, 0);

                context.DeviceContext.CopySubresourceRegion(this.gBuffer[1], 0,
                    new ResourceRegion(midX, 0, 0, 2 * midX, midY, 1),
                    renderTarget, 0, midX, 0, 0);

                context.DeviceContext.CopySubresourceRegion(this.gBuffer[2], 0,
                    new ResourceRegion(0, midY, 0, midX, 2 * midY, 1),
                    renderTarget, 0, 0, midY, 0);

                context.DeviceContext.CopySubresourceRegion(this.gBuffer[3], 0,
                    new ResourceRegion(midX, midY, 0, 2 * midX, 2 * midY, 1),
                    renderTarget, 0, midX, midY, 0);
            }
            else
            {
                context.DeviceContext.CopySubresourceRegion(this.gBuffer[0], 0,
                    new ResourceRegion(0, 0, 0, midX, midY, 1),
                    renderTarget, 0, 0, 0, 0);

                context.DeviceContext.CopySubresourceRegion(this.gBuffer[1], 0,
                    new ResourceRegion(0, 0, 0, midX, midY, 1),
                    renderTarget, 0, midX, 0, 0);

                //context.DeviceContext.CopySubresourceRegion(this.ssBuffer[0], 0,
                context.DeviceContext.CopySubresourceRegion(this.gBuffer[2], 0,
                    new ResourceRegion(0, 0, 0, midX, midY, 1),
                    renderTarget, 0, 0, midY, 0);


                context.DeviceContext.CopySubresourceRegion(this.gBuffer[3], 0,
                    new ResourceRegion(0, 0, 0, midX, midY, 1),
                    renderTarget, 0, midX, midY, 0);
            }
        }

        /// <summary>
        /// The Render function
        /// </summary>
        internal void RenderDeferred(RenderContext renderContext, IRenderer renderRenderable)
        {
            
#if SSAO
            if (this.RenderSSAO)
            {
                if (this.RenderPass == DeferredRenderPasses.RenderDirectDiffuse)
                {
                    // bind quad geometry (good for a full screen-space pass)
                    this.BindQuadBuffer(renderContext);

                    // set SSAO render target and render SSAO
                    this.SetSSBufferTarget(0, renderContext);
                    this.RenderScreenSpaceAO(renderContext);

                    // render lighting to buffer
                    this.SetSSBufferTarget(1, renderContext);
                    this.RenderLighting(renderContext, renderRenderable.Renderables.OfType<ILight3D>());

                    // reset default render targets render the buffer-merge pass
                    if (FXAAEnabled)
                    {
                        this.SetSSBufferTarget(2, renderContext);
                    }
                    else
                    {
                        this.renderHost.SetDefaultRenderTargets();
                    }

                    this.BindQuadBuffer(renderContext);
                    this.RenderMerge(renderContext);

                    // perform FXAA pass
                    if (FXAAEnabled)
                    {
                        this.renderHost.SetDefaultRenderTargets();
                        this.RenderFXAA(renderContext);
                    }

                }
                else if (this.RenderPass == DeferredRenderPasses.RenderBlured)
                {
                    // bind quad geometry (good for a full screen-space pass)
                    this.BindQuadBuffer(renderContext);

                    // set SSAO render target and render SSAO
                    this.SetSSBufferTarget(0, renderContext);
                    this.RenderScreenSpaceAO(renderContext);

                    // render blur and merge
                    this.RenderBlurPass(renderContext, renderRenderable, FXAAEnabled ? 2 : -1);

                    // perform FXAA pass
                    if (FXAAEnabled)
                    {
                        this.renderHost.SetDefaultRenderTargets();
                        this.RenderFXAA(renderContext);
                    }
                }
                else if (this.RenderPass == DeferredRenderPasses.RenderBluredDiffuse)
                {
                    // bind quad geometry (good for a full screen-space pass)
                    this.BindQuadBuffer(renderContext);

                    // set SSAO render target and render SSAO
                    this.SetSSBufferTarget(0, renderContext);
                    this.RenderScreenSpaceAO(renderContext);

                    int target = this.RenderBlurPass(renderContext, renderRenderable, 0);

                    // render lighting to ping-pong buffer 1
                    this.SetSSBufferTarget(1-target, renderContext);
                    this.RenderLighting(renderContext, renderRenderable.Renderables.OfType<ILight3D>());

                    // render merge-pass of buffer 0 and 1
                    if (FXAAEnabled)
                    {
                        this.SetSSBufferTarget(2, renderContext);
                    }
                    else
                    {
                        this.renderHost.SetDefaultRenderTargets();
                    }

                    this.BindQuadBuffer(renderContext);
                    this.RenderMerge(renderContext);

                    if (FXAAEnabled)
                    {
                        this.renderHost.SetDefaultRenderTargets();
                        this.RenderFXAA(renderContext);
                    }
                }
                else
                {
                    // bind quad geometry (good for a full screen-space pass)
                    this.BindQuadBuffer(renderContext);

                    // reset default render targets 
                    if (FXAAEnabled)
                    {
                        this.SetSSBufferTarget(2, renderContext);
                    }
                    else
                    {
                        this.renderHost.SetDefaultRenderTargets();
                    }

                    // render AO buffer only
                    this.RenderScreenSpaceAO(renderContext);

                    if (FXAAEnabled)
                    {
                        this.renderHost.SetDefaultRenderTargets();
                        this.BindQuadBuffer(renderContext);
                        this.RenderFXAA(renderContext);
                    }
                }
            }
            else 
            {
                // reset render targets and run lighting pass
                //this.renderHost.SetDefaultColorTargets(this.depthStencilBufferView);
                if (FXAAEnabled)
                {
                    this.SetSSBufferTarget(2, renderContext);
                }
                else
                {
                    this.renderHost.SetDefaultRenderTargets();
                }

                // set the lights
                this.RenderLighting(renderContext, renderRenderable.Renderables.OfType<ILight3D>());

                if (FXAAEnabled)
                {
                    this.renderHost.SetDefaultRenderTargets();
                    this.BindQuadBuffer(renderContext);
                    this.RenderFXAA(renderContext);
                }
#else
            {
                // reset render targets and run lighting pass
                //this.renderHost.SetDefaultColorTargets(this.depthStencilBufferView);
                this.renderHost.SetDefaultRenderTargets();
                // set the lights                     
                this.RenderLighting(renderContext, renderRenderable.Items.OfType<ILight3D>());
#endif
            }
        }

        /// <summary>
        /// This is the main lighting render function. 
        /// It is called per frame. 
        /// </summary>
        private void RenderLighting(RenderContext context, IEnumerable<ILight3D> lightsCollection)
        {
            // --- extract all lights from collections an build one IEnumerable seqence
            var lights = lightsCollection.Where(l => l is Light3D).Concat(lightsCollection.Where(l => l is Light3DCollection).SelectMany(x => (x as Light3DCollection).Items.Select(xx => xx as ILight3D)));

            // --- eye position
            this.deferredLightingVariables.vEyePos.Set(context.Camera.Position.ToVector4());

#if DEFERRED_MSAA
            // --- num MSAA samples
            this.deferredLightingVariables.nMsaaSamples.Set(this.numberSamplesMSAA);
#endif

            // --- bind quad geometry (good for ambient and directional lights)
            this.BindQuadBuffer(context);

            // -- get the ambient light  (there must be only one)
            var ambientLight = lightsCollection.Where(l => l is AmbientLight3D);
            if (ambientLight.Count() > 0)
            {
                ambientLightColor = ((AmbientLight3D)ambientLight.First()).Color;
            }

            // --- set and render the ambient light pass 
            this.deferredLightingVariables.vLightAmbient.Set(ambientLightColor);
            this.deferredLightingVariables.renderPassAmbient.Apply(context.DeviceContext);
            context.DeviceContext.DrawIndexed(this.screenQuad.IndexCount, 0, 0);


            // --- all next passes are with additive blending
            var pass = this.deferredLightingVariables.renderPassDirectionalLight;

            // --- go over all directional lights
            foreach (DirectionalLight3D light in lights.Where(l => l is DirectionalLight3D))
            {
                // set light variables    
                this.deferredLightingVariables.vLightColor.Set(light.Color);
                this.deferredLightingVariables.vLightDir.Set(light.Direction.ToVector4());

                // --- render the geometry
                pass.Apply(context.DeviceContext);
                context.DeviceContext.DrawIndexed(this.screenQuad.IndexCount, 0, 0);
            }

            // --- now bind sphere geometry (good for point-lights)
            this.BindSphereBuffer(context);

            // --- set the point-light pass (with additive blending)
            pass = this.deferredLightingVariables.renderPassPointLight;

            // --- go over all point lights
            foreach (PointLight3D light in lights.Where(l => l is PointLight3D))
            {                
                // set light variables    
                this.deferredLightingVariables.vLightColor.Set(light.Color);
                this.deferredLightingVariables.vLightPos.Set(light.Position.ToVector4());
                this.deferredLightingVariables.vLightAtt.Set(light.Attenuation.ToVector4(100f));

                double lightRadius = EstimateRadius(light.Attenuation);
                Matrix lightModelMatrix = Matrix.Scaling((float)lightRadius) * Matrix.Translation(light.Position.ToVector3());

                this.deferredLightingVariables.mLightModel.SetMatrix(lightModelMatrix);
                this.deferredLightingVariables.mLightView.SetMatrix(context.viewMatrix);
                this.deferredLightingVariables.mLightProj.SetMatrix(context.projectionMatrix);

                // --- render the geometry
                pass.Apply(context.DeviceContext);
                context.DeviceContext.DrawIndexed(this.screenSphere.IndexCount, 0, 0);
            }

            // --- bind cone buffer for spot-lights
            this.BindConeBuffer(context);

            // --- set the spot-light pass (with additive blending)
            pass = this.deferredLightingVariables.renderPassSpotLight;

            // --- go over all spot-lights
            foreach (SpotLight3D light in lights.Where(l => l is SpotLight3D))
            {
                var lightRadius = (float)EstimateRadius(light.Attenuation);

                double phiHalfRad = light.OuterAngle / 360.0 * Math.PI;
                float scaleX = lightRadius;
                float scaleYZ = lightRadius * (float)Math.Tan(phiHalfRad);

                Vector3 baseX = light.Direction;
                baseX.Normalize();
                Vector3 baseY, baseZ;
                MakeBasis(baseX, out baseY, out baseZ);

                var spot = new Vector4((float)Math.Cos(light.OuterAngle / 360.0 * Math.PI), (float)Math.Cos(light.InnerAngle / 360.0 * Math.PI), (float)light.Falloff, 0);
                this.deferredLightingVariables.vLightSpot.Set(ref spot);
                this.deferredLightingVariables.vLightDir.Set(baseX);
                this.deferredLightingVariables.vLightColor.Set(light.Color);
                this.deferredLightingVariables.vLightPos.Set(light.Position.ToVector4());
                this.deferredLightingVariables.vLightAtt.Set(light.Attenuation.ToVector4(100f));

                Matrix lightModelMatrix = new Matrix(
                    baseX.X * scaleX, baseX.Y * scaleX, baseX.Z * scaleX, 0f,
                    baseY.X * scaleYZ, baseY.Y * scaleYZ, baseY.Z * scaleYZ, 0f,
                    baseZ.X * scaleYZ, baseZ.Y * scaleYZ, baseZ.Z * scaleYZ, 0f,
                    (float)light.Position.X, (float)light.Position.Y, (float)light.Position.Z, 1.0f
                );

                this.deferredLightingVariables.mLightModel.SetMatrix(lightModelMatrix);
                this.deferredLightingVariables.mLightView.SetMatrix(context.viewMatrix);
                this.deferredLightingVariables.mLightProj.SetMatrix(context.projectionMatrix);

                // --- render the geometry
                pass.Apply(context.DeviceContext);
                context.DeviceContext.DrawIndexed(this.screenCone.IndexCount, 0, 0);
            }

            // --- deactivate blending
            context.DeviceContext.OutputMerger.SetBlendState(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="attenuation"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        private static double EstimateRadius(Vector3 attenuation, double tolerance = 0.01)
        {
            double a = attenuation.Z;
            double b = attenuation.Y;
            double c = attenuation.X;
            if ((a == 0) && (b == 0))
            {
                return 10.0f; // there is no attenuation -> pick an arbitrary radius
            }
            else if (a == 0)
            {
                return (1.0 / tolerance); // no quadratic attenuation
            }
            else
            {
                double discriminant = b * b - 4.0f * a * (c - 1.0f / tolerance);
                return (-b + Math.Sqrt(discriminant) / (2.0f * a));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseX"></param>
        /// <param name="baseY"></param>
        /// <param name="baseZ"></param>
        private static void MakeBasis(Vector3 baseX, out Vector3 baseY, out Vector3 baseZ)
        {
            if (Math.Abs(baseX.Z) >= Math.Abs(baseX.X))
            {
                if (Math.Abs(baseX.Z) >= Math.Abs(baseX.Y))
                {
                    baseY = new Vector3(1f, 1f, -(baseX.X + baseX.Y) / baseX.Z);
                }
                else
                {
                    baseY = new Vector3(1f, -(baseX.X + baseX.Z) / baseX.Y, 1f);
                }
            }
            else
            {
                if (Math.Abs(baseX.X) >= Math.Abs(baseX.Y))
                {
                    baseY = new Vector3(-(baseX.Y + baseX.Z) / baseX.X, 1f, 1f);
                }
                else
                {
                    baseY = new Vector3(1f, -(baseX.X + baseX.Z) / baseX.Y, 1f);
                }
            }
            baseY.Normalize();
            baseZ = Vector3.Cross(baseX, baseY);
        }

#if SSAO
        /// <summary>
        /// Call a screen-space AO pass over the g-buffer
        /// </summary>
        private void RenderScreenSpaceAO(RenderContext contxt)
        {
            var aspectRatio = contxt.Canvas.ActualWidth / contxt.Canvas.ActualHeight;
            Matrix projectionMatrix = contxt.Camera.CreateProjectionMatrix(aspectRatio);
            Matrix inverseProjectionMatrix = projectionMatrix.Inverted();

            Matrix viewProjectionMatrix = contxt.Camera.GetViewProjectionMatrix(aspectRatio);
            Matrix inverseViewProjectionMatrix = viewProjectionMatrix.Inverted();
            Matrix inverseViewMatrix = contxt.Camera.GetViewMatrix().Inverted();

            this.deferredLightingVariables.mLightProj.SetMatrix(projectionMatrix);
            this.deferredLightingVariables.mInvProjection.SetMatrix(inverseProjectionMatrix);
            this.deferredLightingVariables.mInvView.SetMatrix(inverseViewMatrix);
            this.deferredLightingVariables.mViewProjection.SetMatrix(viewProjectionMatrix);
            this.deferredLightingVariables.vInvViewportSize.Set(new Vector4(1.0f / (float)contxt.Canvas.ActualWidth, 1.0f / (float)contxt.Canvas.ActualHeight, 0.0f, 0.0f));

            // --- perform screen-space processing
            this.screenSpaceVariables.ssSSAOPass.Apply(contxt.DeviceContext);
            contxt.DeviceContext.DrawIndexed(this.screenQuad.IndexCount, 0, 0);
        }


        /// <summary>
        /// Performs the currently activated blur pass on the content of ScreenSpaceBuffer 0. If renderTargetIndex is set
        /// to -1, the results are rendered to the default render targets, otherwise to the ScreenSpaceBuffer with the index renderTargetIndex (0-2).
        /// If the renderTargetIndex cannot be rendered to, the function chooses another ScreenSpaceBuffer and returns its index.
        /// </summary>
        /// <param name="renderContext"></param>
        /// <param name="renderRenderable"></param>
        /// <param name="renderTargetIndex"></param>
        /// <returns></returns>
        internal int RenderBlurPass(RenderContext renderContext, IRenderer renderRenderable, int renderTargetIndex)
        {
            if (this.BlurPass == DeferredRenderPasses.Blur4x4)
            {
                int target = (renderTargetIndex == 0) ? 1 : renderTargetIndex;
                if (target == -1)
                {
                    this.renderHost.SetDefaultRenderTargets();
                }
                else
                {
                    this.SetSSBufferTarget(target, renderContext);
                }
                this.RenderBlur4x4(renderContext);
                return target;
            }
            else if (this.BlurPass == DeferredRenderPasses.BlurSeparableGaussian)
            {
                int target = (renderTargetIndex == 1) ? 0 : renderTargetIndex;
                this.SetSSBufferTarget(1, renderContext);
                this.RenderBlurH(renderContext);
                if (target == -1)
                {
                    this.renderHost.SetDefaultRenderTargets();
                }
                else
                {
                    this.SetSSBufferTarget(target, renderContext);
                }
                this.RenderBlurV(renderContext);
                return target;
            }
            else if (this.BlurPass == DeferredRenderPasses.BlurCrossBilateral)
            {
                 int target = (renderTargetIndex == 1) ? 0 : renderTargetIndex;
                this.SetSSBufferTarget(1, renderContext);
                
                if (target == -1)
                {
                    this.renderHost.SetDefaultRenderTargets();
                }
                else
                {
                    this.SetSSBufferTarget(target, renderContext);
                }                
                return target;
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>        
        private void RenderMerge(RenderContext context)
        {
            // --- perform screen-space rendering
            this.screenSpaceVariables.ssMergePass.Apply(context.DeviceContext);
            context.DeviceContext.DrawIndexed(this.screenQuad.IndexCount, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>        
        private void RenderBlurH(RenderContext context)
        {
            // --- perform screen-space rendering
            this.screenSpaceVariables.ssBlurHPass.Apply(context.DeviceContext);
            context.DeviceContext.DrawIndexed(this.screenQuad.IndexCount, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>        
        private void RenderBlurV(RenderContext context)
        {
            // --- perform screen-space rendering
            this.screenSpaceVariables.ssBlurVPass.Apply(context.DeviceContext);
            context.DeviceContext.DrawIndexed(this.screenQuad.IndexCount, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>        
        private void RenderBlur4x4(RenderContext context)
        {
            // --- perform screen-space rendering
            this.screenSpaceVariables.ssBlur4x4Pass.Apply(context.DeviceContext);
            context.DeviceContext.DrawIndexed(this.screenQuad.IndexCount, 0, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenderFXAA(RenderContext context)
        {
            // --- perform screen-space rendering
            this.screenSpaceVariables.ssFXAAPass.Apply(context.DeviceContext);
            context.DeviceContext.DrawIndexed(this.screenQuad.IndexCount, 0, 0);
        }

#endif



        /// <summary>
        /// Create the G-Buffer
        /// call it once on init
        /// </summary>
        private void InitGBuffer(Format format = Format.R32G32B32A32_Float)
        {
            // alloc buffer arrays
            this.gBuffer = new Texture2D[NUMGBUFFER];
            this.gBufferRenderTargetView = new RenderTargetView[NUMGBUFFER];
            this.gBufferShaderResourceView = new ShaderResourceView[NUMGBUFFER];

#if DEFERRED_MSAA
                for (int i = 0; i < NUMGBUFFER; i++)
                {
                    // texture
                    this.gBuffer[i] = (new Texture2D(device, new Texture2DDescription()
                    {
                        Format = format,
                        BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                        Width = targetWidth,
                        Height = targetHeight,
                        MipLevels = 1,
                        ArraySize = 1,
                        SampleDescription = new SampleDescription(this.NumberSamplesMSAA, 0),
                        Usage = ResourceUsage.Default,
                        OptionFlags = ResourceOptionFlags.None,
                        CpuAccessFlags = CpuAccessFlags.None,
                    }));

                    // render target
                    this.gBufferRenderTargetView[i] = new RenderTargetView(this.device, this.gBuffer[i]);

                    // shader resource
                    this.gBufferShaderResourceView[i] = new ShaderResourceView(this.device, this.gBuffer[i], new ShaderResourceViewDescription()
                    {
                        Format = format,
                        Dimension = ShaderResourceViewDimension.Texture2DMultisampled,
                        Texture2DMS = new ShaderResourceViewDescription.Texture2DMultisampledResource()
                    });
                }

                // Create Depth-Stencil map for g-buffer            
                this.depthStencilBuffer = new Texture2D(device, new Texture2DDescription()
                {
                    Format = Format.R32_Typeless, //!!!! because of depth and shader resource                
                    ArraySize = 1,
                    MipLevels = 1,
                    Width = targetWidth,
                    Height = targetHeight,
                    SampleDescription = new SampleDescription(this.NumberSamplesMSAA, 0),
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource, //!!!!
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None,
                });

                this.depthStencilBufferView = new DepthStencilView(device, depthStencilBuffer, new DepthStencilViewDescription()
                {
                    Format = Format.D32_Float,
                    Dimension = DepthStencilViewDimension.Texture2DMultisampled,
                    Texture2DMS = new DepthStencilViewDescription.Texture2DMultisampledResource()
                });

                this.gBufferDepthStencilResourceView = new ShaderResourceView(device, this.depthStencilBuffer, new ShaderResourceViewDescription()
                {
                    Format = Format.R32_UInt,
                    Dimension = ShaderResourceViewDimension.Texture2DMultisampled,
                    Texture2DMS = new ShaderResourceViewDescription.Texture2DMultisampledResource()
                });

                // --- set the g-buffer resources
                for (int i = 0; i < gBuffer.Length; i++)
                {
                    this.deferredLightingVariables.gBufferShaderResourceVariables[i].SetResource(this.gBufferShaderResourceView[i]);
                }

                this.deferredLightingVariables.gDepthStencilShaderResourceVariables.SetResource(this.gBufferDepthStencilResourceView);    
#else
            /// Create the render targets for our unoptimized G-Buffer, 
            /// which just uses 32-bit floats for everything
            for (int i = 0; i < NUMGBUFFER; i++)
            {
                // texture
                this.gBuffer[i] = new Texture2D(device, new Texture2DDescription()
                {
                    Format = format,
                    //Format = Format.R32G32B32A32_Float,
                    //Format = Format.B8G8R8A8_UNorm,
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    Width = targetWidth,
                    Height = targetHeight,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.None,
                });

                // render target
                this.gBufferRenderTargetView[i] = new RenderTargetView(this.device, this.gBuffer[i]);

                // shader resource
                this.gBufferShaderResourceView[i] = new ShaderResourceView(this.device, this.gBuffer[i], new ShaderResourceViewDescription()
                {
                    Format = format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                    {
                        MipLevels = 1,
                        MostDetailedMip = 0,
                    }
                });

                // set shader resources
                this.deferredLightingVariables.gBufferShaderResourceVariables[i].SetResource(this.gBufferShaderResourceView[i]);
            }

            /// Create Depth-Stencil map for g-buffer            
            this.depthStencilBuffer = new Texture2D(device, new Texture2DDescription()
            {
                Format = Format.R24G8_Typeless, //!!!! because of depth and shader resource
                //Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = targetWidth,
                Height = targetHeight,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource, //!!!!
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
            });

            this.depthStencilBufferView = new DepthStencilView(device, depthStencilBuffer, new DepthStencilViewDescription()
            {
                Format = Format.D24_UNorm_S8_UInt,
                Dimension = DepthStencilViewDimension.Texture2D,
                Texture2D = new DepthStencilViewDescription.Texture2DResource()
                {
                    MipSlice = 0
                }
            });

            this.gBufferDepthStencilResourceView = new ShaderResourceView(device, depthStencilBuffer, new ShaderResourceViewDescription()
            {
                Format = Format.R24_UNorm_X8_Typeless,
                Dimension = ShaderResourceViewDimension.Texture2D,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                {
                    MipLevels = 1
                }
            });

            this.deferredLightingVariables.gDepthStencilShaderResourceVariables.SetResource(this.gBufferDepthStencilResourceView);
#endif
        }

        /// <summary>
        /// Totaly clear the g-buffer
        /// </summary>
        private void ClearGBuffer()
        {
            Disposer.RemoveAndDispose(ref this.depthStencilBuffer);
            Disposer.RemoveAndDispose(ref this.depthStencilBufferView);
            Disposer.RemoveAndDispose(ref this.gBufferDepthStencilResourceView);
            // cleanup buffer if any
            if (this.gBuffer != null)
            {
                for (int i = 0; i < gBuffer.Length; i++)
                {
                    var buf = gBuffer[i];
                    var view = gBufferRenderTargetView[i];
                    var res = gBufferShaderResourceView[i];
                    Disposer.RemoveAndDispose(ref view);
                    Disposer.RemoveAndDispose(ref res);
                    Disposer.RemoveAndDispose(ref buf);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal void SetGBufferTargets(RenderContext context)
        {
            this.SetGBufferTargets(this.targetWidth, this.targetHeight, context);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        internal void SetGBufferTargets(int width, int height, RenderContext context)
        {
            // --- set rasterizes state here with proper shadow-bias, as depth-bias and slope-bias in the rasterizer            
          //  this.device.ImmediateContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);
          //  this.device.ImmediateContext.OutputMerger.SetTargets(this.depthStencilBufferView, this.gBufferRenderTargetView);

            context.DeviceContext.Rasterizer.SetViewport(0, 0, width, height, 0.0f, 1.0f);
            context.DeviceContext.OutputMerger.SetTargets(this.depthStencilBufferView, this.gBufferRenderTargetView);
            // 0 normal
            // 1 diffuse
            // 2 spec
            // 3 pos
            ClearRenderTargetViews();
        }

        internal void ClearRenderTargetViews()
        {
            // --- clear buffers
            this.device.ImmediateContext.ClearDepthStencilView(this.depthStencilBufferView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
            this.device.ImmediateContext.ClearRenderTargetView(this.gBufferRenderTargetView[0], new Color4());
            this.device.ImmediateContext.ClearRenderTargetView(this.gBufferRenderTargetView[1], this.renderHost.ClearColor); //this.renderHost.ClearColor);
            this.device.ImmediateContext.ClearRenderTargetView(this.gBufferRenderTargetView[2], new Color4());
            this.device.ImmediateContext.ClearRenderTargetView(this.gBufferRenderTargetView[3], new Color4());
        }


        /// <summary>
        /// Create the geometry and the VBO (vertex and index buffers) 
        /// for light-geoemtry 
        /// Call it once in the scene initialization 
        /// </summary>
        private void InitQuadBuffer()
        {
            var vertices = new[]
            {
                new Vector4(-1.0f, -1.0f, 0.0f, 1.0f), 
                new Vector4(+1.0f, -1.0f, 0.0f, 1.0f),
                new Vector4(+1.0f, +1.0f, 0.0f, 1.0f),
                new Vector4(-1.0f, +1.0f, 0.0f, 1.0f),
            };
            int[] indices = { 0, 1, 2, 2, 3, 0 };

            this.screenQuad = new LightGeometryData()
            {
                IndexBuffer = this.device.CreateBuffer(BindFlags.IndexBuffer, sizeof(int), indices),
                VertexBuffer = this.device.CreateBuffer(BindFlags.VertexBuffer, Vector4.SizeInBytes, vertices),
                IndexCount = indices.Length,
            };
        }

        /// <summary>
        /// Create the geometry and the VBO (vertex and index buffers) 
        /// for light-geoemtry 
        /// Call it once in the scene initialization 
        /// </summary>
        private void InitSphereBuffer()
        {            
            var mesh = new MeshBuilder(true, false);
            mesh.AddSphere(new Vector3(0, 0, 0), 1.0);
            MeshGeometry3D meshGeometry = mesh.ToMeshGeometry3D();

            var vertices = meshGeometry.Positions.Select(p => new Vector4(p, 1.0f)).ToArray();

            this.screenSphere = new LightGeometryData()
            {
                IndexBuffer = this.device.CreateBuffer(BindFlags.IndexBuffer, sizeof(int), meshGeometry.Indices.Array, meshGeometry.Indices.Count),
                VertexBuffer = this.device.CreateBuffer(BindFlags.VertexBuffer, Vector4.SizeInBytes, vertices),
                IndexCount = meshGeometry.Indices.Count,
            };
        }

        /// <summary>
        /// Create the geometry and the VBO (vertex and index buffers) 
        /// for light-geoemtry 
        /// Call it once in the scene initialization 
        /// </summary>
        private void InitConeBuffer()
        {
            MeshBuilder mesh = new MeshBuilder(true, false);
            mesh.AddCone(new Vector3(1.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f), 1.0, true, 32);
            MeshGeometry3D meshGeometry = mesh.ToMeshGeometry3D();
            
            var vertices = meshGeometry.Positions.Select(p => new Vector4(p, 1.0f)).ToArray();

            this.screenCone = new LightGeometryData()
            {
                IndexBuffer = this.device.CreateBuffer(BindFlags.IndexBuffer, sizeof(int), meshGeometry.Indices.Array, meshGeometry.Indices.Count),
                VertexBuffer = this.device.CreateBuffer(BindFlags.VertexBuffer, Vector4.SizeInBytes, vertices),
                IndexCount = meshGeometry.Indices.Count,
            };
        }

        /// <summary>
        /// Bind the quad-buffer (context switch!)
        /// call it in the render function
        /// minimize the number of calls
        /// </summary>
        private void BindQuadBuffer(RenderContext context)
        {
            // --- set quad context
            context.DeviceContext.InputAssembler.InputLayout = this.deferredLightingVariables.screenGeometryLayout;
            context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.DeviceContext.InputAssembler.SetIndexBuffer(this.screenQuad.IndexBuffer, Format.R32_UInt, 0);
            context.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.screenQuad.VertexBuffer, Vector4.SizeInBytes, 0));
        }

        /// <summary>
        /// Bind the sphere-buffer (context switch!)
        /// call it in the render function
        /// minimize the number of calls
        /// </summary>
        private void BindSphereBuffer(RenderContext context)
        {
            // --- set sphere context
            context.DeviceContext.InputAssembler.InputLayout = this.deferredLightingVariables.screenGeometryLayout;
            context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.DeviceContext.InputAssembler.SetIndexBuffer(this.screenSphere.IndexBuffer, Format.R32_UInt, 0);
            context.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.screenSphere.VertexBuffer, Vector4.SizeInBytes, 0));
        }

        /// <summary>
        /// Bind the cone-buffer (context switch!)
        /// call it in the render function
        /// minimize the number of calls
        /// </summary>
        private void BindConeBuffer(RenderContext context)
        {
            // --- set cone context
            context.DeviceContext.InputAssembler.InputLayout = this.deferredLightingVariables.screenGeometryLayout;
            context.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            context.DeviceContext.InputAssembler.SetIndexBuffer(this.screenCone.IndexBuffer, Format.R32_UInt, 0);
            context.DeviceContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(this.screenCone.VertexBuffer, Vector4.SizeInBytes, 0));
        }



#if SSAO
        /// <summary>
        /// Init the Color-Buffer as resource
        /// </summary>
        private void InitSSBuffer(Format format = Format.R32G32B32A32_Float)
        {
            // alloc buffer arrays
            this.ssBuffer = new Texture2D[NUMSSBUFFER];
            this.ssBufferRenderTargetView = new RenderTargetView[NUMSSBUFFER];
            this.ssBufferShaderResourceView = new ShaderResourceView[NUMSSBUFFER];

            for (int i = 0; i < NUMSSBUFFER; i++)
            {
                // init texture
                this.ssBuffer[i] = new Texture2D(device, new Texture2DDescription()
                {
                    Format = format,
                    BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                    Width = targetWidth,
                    Height = targetHeight,
                    MipLevels = 1,
                    ArraySize = 1,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Default,
                    OptionFlags = ResourceOptionFlags.None,
                    CpuAccessFlags = CpuAccessFlags.None,
                });

                // init render target
                this.ssBufferRenderTargetView[i] = new RenderTargetView(this.device, this.ssBuffer[i]);

                // init shader resource
                this.ssBufferShaderResourceView[i] = new ShaderResourceView(this.device, this.ssBuffer[i], new ShaderResourceViewDescription()
                {
                    Format = format,
                    Dimension = ShaderResourceViewDimension.Texture2D,
                    Texture2D = new ShaderResourceViewDescription.Texture2DResource()
                    {
                        MipLevels = 1,
                        MostDetailedMip = 0,
                    }
                });

                // set shader resources
                this.screenSpaceVariables.ssBufferShaderResourceVariables[i].SetResource(this.ssBufferShaderResourceView[i]);
            }


            // init random normals texture
            this.randNormalMapShaderResourceView = TextureLoader.FromFileAsShaderResourceView(device, @"./Textures/random4x4_dot3.png");
            //this.randNormalMapShaderResourceView = ShaderResourceView.FromFile(device, @"./Textures/random_dot3.jpg");
            // set shader resources
            this.screenSpaceVariables.randNormalsShaderResourceVariable.SetResource(this.randNormalMapShaderResourceView);
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearSSBuffer()
        {
            if (this.ssBuffer != null)
            {
                for (int i = 0; i < ssBufferShaderResourceView.Length; i++)
                {
                    Disposer.RemoveAndDispose(ref this.ssBufferShaderResourceView[i]);
                    Disposer.RemoveAndDispose(ref this.ssBufferRenderTargetView[i]);
                    Disposer.RemoveAndDispose(ref this.ssBuffer[i]);
                }
            }
            Disposer.RemoveAndDispose(ref this.randNormalMapShaderResourceView);
        }

        
        /// <summary>
        /// Set screen-space buffer as render target
        /// </summary>
        private void SetSSBufferTarget(int bufferIndex, RenderContext context)
        {
            context.DeviceContext.OutputMerger.SetTargets(this.ssBufferRenderTargetView[bufferIndex]);
            //context.DeviceContext.ClearRenderTargetView(this.ssBufferRenderTargetView, new Color4(1,1,1,1));            
        }
#endif

        /// <summary>
        /// CG is not working on DX11 objects, 
        /// so we have to cleanup here by ourseves
        /// </summary>
        public void Dispose()
        {
            this.ClearGBuffer();            
            this.screenQuad.Dispose();
            this.screenSphere.Dispose();
            this.screenCone.Dispose();
            Disposer.RemoveAndDispose(ref this.deferredLightingVariables);

#if SSAO
            this.ClearSSBuffer();
            Disposer.RemoveAndDispose(ref this.screenSpaceVariables);
#endif
        }



        /// <summary>
        /// helper-struct that wrapps the VBO 
        /// for particular scene objects, 
        /// like quad, sphere, or cone for deferred lights
        /// </summary>
        private struct LightGeometryData : IDisposable
        {
            public Buffer VertexBuffer;
            public Buffer IndexBuffer;
            public int IndexCount;

            public void Dispose()
            {
                Disposer.RemoveAndDispose(ref this.VertexBuffer);
                Disposer.RemoveAndDispose(ref this.IndexBuffer);
            }
        }

        /// <summary>
        /// Helper-class which wrapps all parameters needed for deferred lighting
        /// </summary>
        private class DeferredLightingVariables : IDisposable
        {
            public DeferredLightingVariables(IEffectsManager effectsContainer, IRenderTechniquesManager techniquesManager)
            {
                var deferredLightingTechnique = techniquesManager.RenderTechniques[DeferredRenderTechniqueNames.DeferredLighting];

                this.screenGeometryEffect = effectsContainer.GetEffect(deferredLightingTechnique);
                this.screenGeometryLayout = effectsContainer.GetLayout(deferredLightingTechnique);
                this.screenGeometryTechnique = screenGeometryEffect.GetTechniqueByName(deferredLightingTechnique.Name);

                this.renderPassAmbient = this.screenGeometryTechnique.GetPassByName("AmbientPass");
                this.renderPassDirectionalLight = this.screenGeometryTechnique.GetPassByName("DirectionalLightPass");
                this.renderPassPointLight = this.screenGeometryTechnique.GetPassByName("PointLightPass");
                this.renderPassSpotLight = this.screenGeometryTechnique.GetPassByName("SpotLightPass");

                this.vBackgroundColor = this.screenGeometryEffect.GetVariableByName("vBackgroundColor").AsVector();
                this.vLightDir = this.screenGeometryEffect.GetVariableByName("vLightDir").AsVector();
                this.vLightPos = this.screenGeometryEffect.GetVariableByName("vLightPos").AsVector();
                this.vLightAtt = this.screenGeometryEffect.GetVariableByName("vLightAtt").AsVector();
                this.vLightSpot = this.screenGeometryEffect.GetVariableByName("vLightSpot").AsVector();
                this.vLightColor = this.screenGeometryEffect.GetVariableByName("vLightColor").AsVector();
                this.vLightAmbient = this.screenGeometryEffect.GetVariableByName("vLightAmbient").AsVector();

                this.vEyePos = this.screenGeometryEffect.GetVariableByName("vEyePos").AsVector();
                this.mLightModel = this.screenGeometryEffect.GetVariableByName("mLightModel").AsMatrix();
                this.mLightView = this.screenGeometryEffect.GetVariableByName("mLightView").AsMatrix();
                this.mLightProj = this.screenGeometryEffect.GetVariableByName("mLightProj").AsMatrix();
                this.nMsaaSamples = this.screenGeometryEffect.GetVariableByName("nMsaaSamples").AsScalar();

                // for SSAO
                this.vInvViewportSize = this.screenGeometryEffect.GetVariableByName("vInvViewportSize").AsVector();
                this.mInvProjection = this.screenGeometryEffect.GetVariableByName("mInvProjection").AsMatrix();
                this.mInvView = this.screenGeometryEffect.GetVariableByName("mInvView").AsMatrix();
                this.mViewProjection = this.screenGeometryEffect.GetVariableByName("mViewProjection").AsMatrix();

                this.gDepthStencilShaderResourceVariables = this.screenGeometryEffect.GetVariableByName("DepthTexture").AsShaderResource();

                this.gBufferShaderResourceVariables = new EffectShaderResourceVariable[NUMGBUFFER];
                this.gBufferShaderResourceVariables[0] = this.screenGeometryEffect.GetVariableByName("NormalTexture").AsShaderResource();
                this.gBufferShaderResourceVariables[1] = this.screenGeometryEffect.GetVariableByName("DiffuseAlbedoTexture").AsShaderResource();
                this.gBufferShaderResourceVariables[2] = this.screenGeometryEffect.GetVariableByName("SpecularAlbedoTexture").AsShaderResource();
                this.gBufferShaderResourceVariables[3] = this.screenGeometryEffect.GetVariableByName("PositionTexture").AsShaderResource();
            }

            // effect variables
            public Effect screenGeometryEffect;
            public InputLayout screenGeometryLayout;

            public EffectPass renderPassAmbient;
            public EffectPass renderPassDirectionalLight;
            public EffectPass renderPassPointLight;
            public EffectPass renderPassSpotLight;
            public EffectTechnique screenGeometryTechnique;

            public EffectVectorVariable vBackgroundColor;
            public EffectVectorVariable vLightDir;
            public EffectVectorVariable vLightPos;
            public EffectVectorVariable vLightAtt;
            public EffectVectorVariable vLightSpot;        //(outer angle , inner angle, falloff, light-range!!!)            
            public EffectVectorVariable vLightColor;
            public EffectVectorVariable vLightAmbient;
            public EffectVectorVariable vEyePos;
            public EffectMatrixVariable mLightModel;
            public EffectMatrixVariable mLightView;
            public EffectMatrixVariable mLightProj;
            public EffectScalarVariable nMsaaSamples;

            // for SSAO
            public EffectVectorVariable vInvViewportSize;
            public EffectMatrixVariable mInvProjection;
            public EffectMatrixVariable mInvView;
            public EffectMatrixVariable mViewProjection;

            public EffectShaderResourceVariable[] gBufferShaderResourceVariables;
            public EffectShaderResourceVariable gDepthStencilShaderResourceVariables;

            public void Dispose()
            {
                //Disposer.RemoveAndDispose(ref this.screenGeometryTechnique);
                Disposer.RemoveAndDispose(ref this.renderPassAmbient);
                Disposer.RemoveAndDispose(ref this.renderPassPointLight);
                Disposer.RemoveAndDispose(ref this.renderPassDirectionalLight);
                Disposer.RemoveAndDispose(ref this.renderPassSpotLight);

                Disposer.RemoveAndDispose(ref this.vBackgroundColor);
                Disposer.RemoveAndDispose(ref this.vLightDir);
                Disposer.RemoveAndDispose(ref this.vLightPos);
                Disposer.RemoveAndDispose(ref this.vLightAtt);
                Disposer.RemoveAndDispose(ref this.vLightSpot);
                Disposer.RemoveAndDispose(ref this.vLightColor);
                Disposer.RemoveAndDispose(ref this.vLightAmbient);

                Disposer.RemoveAndDispose(ref this.vEyePos);
                Disposer.RemoveAndDispose(ref this.mLightModel);
                Disposer.RemoveAndDispose(ref this.mLightView);
                Disposer.RemoveAndDispose(ref this.mLightProj);
                Disposer.RemoveAndDispose(ref this.nMsaaSamples);

                // for SSAO
                Disposer.RemoveAndDispose(ref this.vInvViewportSize);
                Disposer.RemoveAndDispose(ref this.mInvProjection);
                Disposer.RemoveAndDispose(ref this.mInvView);
                Disposer.RemoveAndDispose(ref this.mViewProjection);

                if (this.gBufferShaderResourceVariables != null)
                {
                    for (int i = 0; i < this.gBufferShaderResourceVariables.Length; i++)
                    {
                        var buf = gBufferShaderResourceVariables[i];
                        Disposer.RemoveAndDispose(ref buf);
                    }
                }
                Disposer.RemoveAndDispose(ref this.gDepthStencilShaderResourceVariables);
            }
        }

#if SSAO
        /// <summary>
        /// Helper-class that wrapps shader variables needed for screen-space processing
        /// </summary>
        private class ScreenSpaceProcessingVariables : IDisposable
        {
            public ScreenSpaceProcessingVariables(IEffectsManager effectsContainer, IRenderTechniquesManager renderTechniquesManager)
            {

                var deferredLighting = renderTechniquesManager.RenderTechniques[DeferredRenderTechniqueNames.DeferredLighting];
                this.screenSpaceEffect = effectsContainer.GetEffect(deferredLighting);
                this.screenSpaceLayout = effectsContainer.GetLayout(deferredLighting);
                this.screenSpaceTechnique = screenSpaceEffect.GetTechniqueByName(DeferredRenderTechniqueNames.ScreenSpace);

                this.ssBlurHPass = screenSpaceTechnique.GetPassByName("BlurHPass");
                this.ssBlurVPass = screenSpaceTechnique.GetPassByName("BlurVPass");
                this.ssBlur4x4Pass = screenSpaceTechnique.GetPassByName("Blur4x4Pass");
                this.ssSSAOPass = screenSpaceTechnique.GetPassByName("SSAOPass");
                this.ssMergePass = screenSpaceTechnique.GetPassByName("MergePass");
                this.ssFXAAPass = screenSpaceTechnique.GetPassByName("FXAAPass");
                this.randNormalsShaderResourceVariable = this.screenSpaceEffect.GetVariableByName("RandNormalsTexture").AsShaderResource();

                this.ssBufferShaderResourceVariables = new EffectShaderResourceVariable[NUMSSBUFFER];
                this.ssBufferShaderResourceVariables[0] = this.screenSpaceEffect.GetVariableByName("ScreenSpaceBufferTexture0").AsShaderResource();
                this.ssBufferShaderResourceVariables[1] = this.screenSpaceEffect.GetVariableByName("ScreenSpaceBufferTexture1").AsShaderResource();
                this.ssBufferShaderResourceVariables[2] = this.screenSpaceEffect.GetVariableByName("ScreenSpaceBufferTexture2").AsShaderResource();
            }

            // effect variables
            public Effect screenSpaceEffect;
            public InputLayout screenSpaceLayout;
            public EffectTechnique screenSpaceTechnique;
            public EffectPass ssBlurHPass, ssBlurVPass, ssBlur4x4Pass, ssSSAOPass, ssMergePass;
            public EffectPass ssFXAAPass;

            public EffectShaderResourceVariable[] ssBufferShaderResourceVariables;
            public EffectShaderResourceVariable randNormalsShaderResourceVariable;

            public void Dispose()
            {
                Disposer.RemoveAndDispose(ref this.ssBlurHPass);
                Disposer.RemoveAndDispose(ref this.ssBlurVPass);
                Disposer.RemoveAndDispose(ref this.ssBlur4x4Pass);
                Disposer.RemoveAndDispose(ref this.ssSSAOPass);
                Disposer.RemoveAndDispose(ref this.ssMergePass);
                Disposer.RemoveAndDispose(ref this.ssFXAAPass);

                Disposer.RemoveAndDispose(ref this.screenSpaceTechnique);

                if (this.ssBufferShaderResourceVariables != null)
                {
                    for (int i = 0; i < NUMSSBUFFER; i++)
                    {
                        Disposer.RemoveAndDispose(ref this.ssBufferShaderResourceVariables[i]);
                    }
                }

                Disposer.RemoveAndDispose(ref this.randNormalsShaderResourceVariable);
            }
        } 
#endif

    } 
#else
    public class DeferredRenderer : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
#endif
}