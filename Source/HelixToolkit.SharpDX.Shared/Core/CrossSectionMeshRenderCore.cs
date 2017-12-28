/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Utilities;

    public class CrossSectionMeshRenderCore : MeshRenderCore
    {
        #region Shader Variables

        /// <summary>
        /// Used to draw back faced triangles onto stencil buffer
        /// </summary>
        private RasterizerState backfaceRasterState;

        #endregion

        /// <summary>
        /// Defines the sectionColor
        /// </summary>
        public Color4 SectionColor = Color.Firebrick;

        /// <summary>
        /// Defines the planeEnabled
        /// </summary>
        public Bool4 PlaneEnabled = new Bool4(false, false, false, false);

        /// <summary>
        /// Defines the planeParams
        /// </summary>
        public Matrix PlaneParams = new Matrix();

        private ClipPlaneStruct clipParameter;

        private IBufferProxy clipParamCB;

        private IShaderPass drawBackfacePass;
        private IShaderPass drawScreenQuadPass;

        protected override bool OnAttach(IRenderTechnique technique)
        {
            clipParamCB = technique.ConstantBufferPool.Register(GetClipParamsCBDescription());
            drawBackfacePass = technique[DefaultPassNames.Backface];
            drawScreenQuadPass = technique[DefaultPassNames.ScreenQuad];
            return base.OnAttach(technique);
        }

        protected virtual ConstantBufferDescription GetClipParamsCBDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ClipParamsCB, ClipPlaneStruct.SizeInBytes);
        }

        protected override bool CreateRasterState(RasterizerStateDescription description, bool force)
        {
            if(!base.CreateRasterState(description, force))
            {
                return false;
            }
            #region Create states
            RemoveAndDispose(ref backfaceRasterState);
            this.backfaceRasterState = Collect(new RasterizerState(this.Device,
                new RasterizerStateDescription()
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Front,
                    DepthBias = description.DepthBias,
                    DepthBiasClamp = description.DepthBiasClamp,
                    SlopeScaledDepthBias = description.SlopeScaledDepthBias,
                    IsDepthClipEnabled = description.IsDepthClipEnabled,
                    IsFrontCounterClockwise = description.IsFrontCounterClockwise,
                    IsMultisampleEnabled = false,
                    IsScissorEnabled = false
                }));
            #endregion
            return true;
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
            base.OnUpdatePerModelStruct(ref model, context);
            clipParameter.CrossSectionColors = SectionColor;
            clipParameter.EnableCrossPlane = PlaneEnabled;
            clipParameter.CrossPlaneParams = PlaneParams;
        }

        protected override void OnUploadPerModelConstantBuffers(DeviceContext context)
        {
            base.OnUploadPerModelConstantBuffers(context);
            clipParamCB.UploadDataToBuffer(context, ref clipParameter);
        }

        protected override void OnRender(IRenderContext renderContext)
        {
            base.OnRender(renderContext);
            // Draw backface into stencil buffer
            DepthStencilView dsView;
            var renderTargets = renderContext.DeviceContext.OutputMerger.GetRenderTargets(1, out dsView);
            if (dsView == null)
            {
                return;
            }
            renderContext.DeviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Stencil, 0, 0);
            renderContext.DeviceContext.OutputMerger.SetRenderTargets(dsView, new RenderTargetView[0]);//Remove render target
            renderContext.DeviceContext.Rasterizer.State = backfaceRasterState;
            drawBackfacePass.BindShader(renderContext.DeviceContext);
            drawBackfacePass.BindStates(renderContext.DeviceContext, StateType.BlendState);
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(drawBackfacePass.DepthStencilState, 1); //Draw backface onto stencil buffer, set value to 1
            OnDraw(renderContext.DeviceContext, InstanceBuffer);

            //Draw full screen quad to fill cross section            
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            renderContext.DeviceContext.Rasterizer.State = RasterState;
            drawScreenQuadPass.BindShader(renderContext.DeviceContext);
            drawScreenQuadPass.BindStates(renderContext.DeviceContext, StateType.BlendState);
            renderContext.DeviceContext.OutputMerger.SetRenderTargets(dsView, renderTargets);//Rebind render target
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(drawScreenQuadPass.DepthStencilState, 1); //Only pass stencil buffer test if value is 1
            renderContext.DeviceContext.Draw(4, 0);

            //Decrement ref count. See OutputMerger.GetRenderTargets remarks
            dsView.Dispose();
            foreach (var t in renderTargets)
            { t.Dispose(); }
        }
    }
}
