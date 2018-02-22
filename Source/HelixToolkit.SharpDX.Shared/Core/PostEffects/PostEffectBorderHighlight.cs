/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Wpf.SharpDX.Render;
using HelixToolkit.Wpf.SharpDX.Shaders;
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
    public class PostEffectMeshBorderHighlightCore : RenderCoreBase<ClipPlaneStruct>
    {
        public PostEffect Effect
        {
            set; get;
        } = new PostEffect(DefaultRenderTechniqueNames.PostEffectMeshOutline);

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
            context.IsCustomPass = true;
            foreach (var mesh in context.RenderHost.PerFramePostEffectCores)
            {
                if (mesh.PostEffects.Contains(Effect))
                {
                    context.CustomPassName = DefaultPassNames.MeshOutlineP1;
                    var pass = mesh.EffectTechnique[DefaultPassNames.MeshOutlineP1];
                    if (pass.IsNULL) { continue; }
                    pass.BindStates(deviceContext, StateType.BlendState);
                    deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(pass.DepthStencilState, 1);
                    mesh.Render(context, deviceContext);
                    context.CustomPassName = DefaultPassNames.MeshOutlineP2;
                    pass = mesh.EffectTechnique[DefaultPassNames.MeshOutlineP2];
                    if (pass.IsNULL) { continue; }
                    pass.BindStates(deviceContext, StateType.BlendState);
                    deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(pass.DepthStencilState, 0);
                    mesh.Render(context, deviceContext);
                }
            }           
            context.IsCustomPass = false;
            //Draw full screen quad to fill cross section            
            deviceContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            screenQuadPass.BindShader(deviceContext);
            deviceContext.DeviceContext.OutputMerger.SetDepthStencilState(screenQuadPass.DepthStencilState, 0);
            screenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.RasterState);
            deviceContext.DeviceContext.Draw(4, 0);

            //Decrement ref count. See OutputMerger.GetRenderTargets remarks
            dsView.Dispose();
            foreach (var t in renderTargets)
            { t.Dispose(); }
        }

        protected override void OnUpdatePerModelStruct(ref ClipPlaneStruct model, IRenderContext context)
        {
            
        }
    }
}
