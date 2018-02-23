/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    using Render;
    using Shaders;
    public class PostEffectMeshBorderHighlightCore : RenderCoreBase<ClipPlaneStruct>
    {
        public string EffectName
        {
            set; get;
        } = DefaultRenderTechniqueNames.PostEffectMeshOutline;

        private IList<MeshRenderCore> meshes;
        public IList<MeshRenderCore> Meshes
        {
            set
            {
                SetAffectsRender(ref meshes, value);
            }
            get
            {
                return meshes;
            }
        }

        public Color4 BorderColor
        {
            set
            {
                SetAffectsRender(ref modelStruct.CrossSectionColors, value);
            }
            get { return modelStruct.CrossSectionColors.ToColor4(); }
        }

        private IShaderPass screenQuadPass;

        private IShaderPass screenBlurPass;

        private IShaderPass screenOutlinePass;

        private ShaderResouceViewProxy renderTargetFull;

        private ShaderResouceViewProxy renderTargetBlur;

        private int textureSlot;

        private int samplerSlot;

        private SamplerState sampler;

        private const int downSamplingScale = 1;

        private Texture2DDescription renderTargetDesc = new Texture2DDescription()
        {
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None, Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
            Usage = ResourceUsage.Default, ArraySize = 1, MipLevels = 1, OptionFlags = ResourceOptionFlags.None,
            SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0)
        };

        private Texture2DDescription renderTargetBlurDesc = new Texture2DDescription()
        {
            BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm,
            Usage = ResourceUsage.Default,
            ArraySize = 1,
            MipLevels = 1,
            OptionFlags = ResourceOptionFlags.None,
            SampleDescription = new global::SharpDX.DXGI.SampleDescription(1, 0)
        };

        private ShaderResourceViewDescription targetResourceViewDesc = new ShaderResourceViewDescription()
        {
            Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm, Dimension = ShaderResourceViewDimension.Texture2D,
            Texture2D = new ShaderResourceViewDescription.Texture2DResource()
            {
                MipLevels = 1,
                MostDetailedMip = 0,
            }
        };

        private RenderTargetViewDescription renderTargetViewDesc = new RenderTargetViewDescription()
        {
            Format = global::SharpDX.DXGI.Format.B8G8R8A8_UNorm, Dimension = RenderTargetViewDimension.Texture2D,
            Texture2D = new RenderTargetViewDescription.Texture2DResource() {  MipSlice = 0 }
        };

        public PostEffectMeshBorderHighlightCore()
        {
            BorderColor = Color.Blue;
        }

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ClipParamsCB, ClipPlaneStruct.SizeInBytes);
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                screenQuadPass = technique.GetPass(DefaultPassNames.ScreenQuad);
                screenBlurPass = technique.GetPass(DefaultPassNames.MeshOutlineBlur);
                screenOutlinePass = technique.GetPass(DefaultPassNames.MeshOutline);
                textureSlot = screenOutlinePass.GetShader(ShaderStage.Pixel).ShaderResourceViewMapping.TryGetBindSlot(DefaultBufferNames.DiffuseMapTB);
                samplerSlot = screenOutlinePass.GetShader(ShaderStage.Pixel).SamplerMapping.TryGetBindSlot(DefaultSamplerStateNames.DiffuseMapSampler);
                sampler = Collect(technique.EffectsManager.StateManager.Register(DefaultSamplers.LinearSamplerClampAni4));
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool CanRender(IRenderContext context)
        {
            return true;
        }

        protected override void OnRender(IRenderContext context, DeviceContextProxy deviceContext)
        {
            DepthStencilView dsView;
            var renderTargets = deviceContext.DeviceContext.OutputMerger.GetRenderTargets(1, out dsView);
            if (dsView == null)
            {
                return;
            }
            deviceContext.DeviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Stencil, 0, 0);
            if(renderTargetFull == null || renderTargetDesc.Width != (int)(context.ActualWidth) || renderTargetDesc.Height != (int)(context.ActualHeight))
            {
                RemoveAndDispose(ref renderTargetFull);
                renderTargetDesc.Width = (int)(context.ActualWidth);
                renderTargetDesc.Height = (int)(context.ActualHeight);
                renderTargetFull = Collect(new ShaderResouceViewProxy(deviceContext.DeviceContext.Device, renderTargetDesc));
                renderTargetFull.CreateView(renderTargetViewDesc);
                renderTargetFull.CreateView(targetResourceViewDesc);

                RemoveAndDispose(ref renderTargetBlur);
                renderTargetBlurDesc.Width = (int)(context.ActualWidth / downSamplingScale);
                renderTargetBlurDesc.Height = (int)(context.ActualHeight / downSamplingScale);
                renderTargetBlur = Collect(new ShaderResouceViewProxy(deviceContext.DeviceContext.Device, renderTargetBlurDesc));
                renderTargetBlur.CreateView(renderTargetViewDesc);
                renderTargetBlur.CreateView(targetResourceViewDesc);
            }


            deviceContext.DeviceContext.OutputMerger.SetRenderTargets(dsView, new RenderTargetView[0]);
            context.IsCustomPass = true;
            foreach (var mesh in context.RenderHost.PerFramePostEffectCores)
            {
                if (mesh.HasPostEffect(EffectName))
                {
                    context.CustomPassName = DefaultPassNames.MeshOutlineP1;
                    var pass = mesh.EffectTechnique[DefaultPassNames.MeshOutlineP1];
                    if (pass.IsNULL) { continue; }
                    pass.BindShader(deviceContext);
                    pass.BindStates(deviceContext, StateType.BlendState);
                    deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(pass.DepthStencilState, 1);
                    mesh.Render(context, deviceContext);
                }
            }

            
            //deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            deviceContext.DeviceContext.ClearRenderTargetView(renderTargets[0], Color.Black);
            #region Draw screen quad, filter by stencil buffer
            BindTarget(dsView, renderTargetFull, deviceContext, renderTargetDesc.Width, renderTargetDesc.Height);
            //screenQuadPass.BindShader(deviceContext);
            //screenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState);
            //deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(screenQuadPass.DepthStencilState, 0);
            //deviceContext.DeviceContext.Draw(4, 0);    
            foreach (var mesh in context.RenderHost.PerFramePostEffectCores)
            {
                if (mesh.HasPostEffect(EffectName))
                {
                    context.CustomPassName = DefaultPassNames.MeshOutlineP2;
                    var pass = mesh.EffectTechnique[DefaultPassNames.MeshOutlineP2];
                    if (pass.IsNULL) { continue; }
                    pass.BindShader(deviceContext);
                    pass.BindStates(deviceContext, StateType.BlendState);
                    deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(pass.DepthStencilState, 0);
                    mesh.Render(context, deviceContext);
                }
            }
            context.IsCustomPass = false;
            #endregion
            deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            #region Do Blur
            BindTarget(dsView, renderTargetBlur, deviceContext, renderTargetBlurDesc.Width, renderTargetBlurDesc.Height);          
            screenBlurPass.GetShader(ShaderStage.Pixel).BindSampler(deviceContext, samplerSlot, sampler);
            screenBlurPass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, renderTargetFull.TextureView);
            screenBlurPass.BindShader(deviceContext);
            screenBlurPass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState | StateType.DepthStencilState);
            deviceContext.DeviceContext.Draw(4, 0);
            #endregion

            #region Draw outline to original target
            context.RenderHost.SetDefaultRenderTargets(false);          
            screenOutlinePass.GetShader(ShaderStage.Pixel).BindSampler(deviceContext, samplerSlot, sampler);
            screenOutlinePass.GetShader(ShaderStage.Pixel).BindTexture(deviceContext, textureSlot, renderTargetFull.TextureView);
            screenOutlinePass.BindShader(deviceContext);
            deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(screenOutlinePass.DepthStencilState, 1);
            screenOutlinePass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState);
            deviceContext.DeviceContext.Draw(4, 0);
            #endregion

            //Decrement ref count. See OutputMerger.GetRenderTargets remarks
            dsView.Dispose();
            foreach (var t in renderTargets)
            { t.Dispose(); }
        }

        private static void BindTarget(DepthStencilView dsv, RenderTargetView targetView, DeviceContext context, int width, int height, bool clear = true)
        {
            if (clear)
            {
                context.ClearRenderTargetView(targetView, Color.Red);
            }
            context.OutputMerger.SetRenderTargets(dsv, new RenderTargetView[] { targetView });
            context.Rasterizer.SetViewport(0, 0, width, height);
            context.Rasterizer.SetScissorRectangle(0, 0, width, height);
        }

        protected override void OnUpdatePerModelStruct(ref ClipPlaneStruct model, IRenderContext context)
        {
            
        }

        protected override void OnRenderCustom(IRenderContext context, DeviceContextProxy deviceContext, IShaderPass shaderPass)
        {
        }

        protected override void OnRenderShadow(IRenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
}
