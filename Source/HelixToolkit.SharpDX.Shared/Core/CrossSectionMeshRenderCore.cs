using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    public class CrossSectionMeshRenderCore : MeshRenderCore
    {
        #region Shader Variables
        /// <summary>
        /// Defines the planeParamsVar
        /// </summary>
        private EffectMatrixVariable planeParamsVar;

        /// <summary>
        /// Defines the planeEnabledVar
        /// </summary>
        private EffectVectorVariable planeEnabledVar;

        /// <summary>
        /// Defines the crossSectionColorVar
        /// </summary>
        private EffectVectorVariable crossSectionColorVar;

        /// <summary>
        /// Defines the sectionFillTextureVar
        /// </summary>
        private EffectShaderResourceVariable sectionFillTextureVar;

        /// <summary>
        /// Used to draw back faced triangles onto stencil buffer
        /// </summary>
        private RasterizerState fillStencilRasterState;

        /// <summary>
        /// Used to set stencil buffer to 1 on back faces
        /// </summary>
        private DepthStencilState fillCrossSectionStencilState;

        /// <summary>
        /// Pass fragment on area only the stencil buffer = 1
        /// </summary>
        private DepthStencilState fillStencilState;
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

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                planeParamsVar = Collect(Effect.GetVariableByName(ShaderVariableNames.CrossPlaneParams).AsMatrix());
                planeEnabledVar = Collect(Effect.GetVariableByName(ShaderVariableNames.EnableCrossPlane).AsVector());
                crossSectionColorVar = Collect(Effect.GetVariableByName(ShaderVariableNames.CrossSectionColor).AsVector());
                sectionFillTextureVar = Collect(Effect.GetVariableByName(ShaderVariableNames.SectionFillTexture).AsShaderResource());
                return true;
            }
            else
            {
                return false;
            }
        }

        protected override bool CreateRasterState(RasterizerStateDescription description, bool force)
        {
            if(!base.CreateRasterState(description, force))
            {
                return false;
            }
            #region Create states
            RemoveAndDispose(ref fillStencilRasterState);
            RemoveAndDispose(ref fillStencilState);
            RemoveAndDispose(ref fillCrossSectionStencilState);
            this.fillStencilRasterState = Collect(new RasterizerState(this.Device,
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

            fillStencilState = Collect(new DepthStencilState(this.Device,
                new DepthStencilStateDescription()
                {
                    IsDepthEnabled = true,
                    IsStencilEnabled = true,
                    DepthWriteMask = DepthWriteMask.Zero,
                    DepthComparison = Comparison.Less,
                    StencilWriteMask = 0xFF,
                    StencilReadMask = 0,
                    BackFace = new DepthStencilOperationDescription()
                    {
                        PassOperation = StencilOperation.Replace,
                        Comparison = Comparison.Always,
                        DepthFailOperation = StencilOperation.Keep,
                        FailOperation = StencilOperation.Keep
                    },
                    FrontFace = new DepthStencilOperationDescription()
                    {
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Never,
                        DepthFailOperation = StencilOperation.Keep,
                        FailOperation = StencilOperation.Keep
                    }
                }));

            fillCrossSectionStencilState = Collect(new DepthStencilState(this.Device,
                new DepthStencilStateDescription()
                {
                    IsDepthEnabled = false,
                    IsStencilEnabled = true,
                    DepthWriteMask = DepthWriteMask.Zero,
                    DepthComparison = Comparison.Less,
                    FrontFace = new DepthStencilOperationDescription()
                    {
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Keep,
                        PassOperation = StencilOperation.Keep,
                        Comparison = Comparison.Equal
                    },
                    BackFace = new DepthStencilOperationDescription()
                    {
                        Comparison = Comparison.Never,
                        FailOperation = StencilOperation.Keep,
                        DepthFailOperation = StencilOperation.Keep,
                        PassOperation = StencilOperation.Keep
                    },
                    StencilReadMask = 0xFF,
                    StencilWriteMask = 0
                }));
            #endregion
            return true;
        }

        protected override void SetShaderVariables(IRenderMatrices context)
        {
            base.SetShaderVariables(context);
            planeParamsVar.SetMatrix(PlaneParams);
            planeEnabledVar.Set(PlaneEnabled);
            crossSectionColorVar.Set(SectionColor);
        }

        protected override void OnRender(IRenderMatrices renderContext)
        {
            base.OnRender(renderContext);
            //Draw backface into stencil buffer
            renderContext.DeviceContext.Rasterizer.State = fillStencilRasterState;
            DepthStencilView dsView;
            var renderTargets = renderContext.DeviceContext.OutputMerger.GetRenderTargets(1, out dsView);
            if (dsView == null)
            {
                return;
            }
            renderContext.DeviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Stencil, 0, 0);
            var pass = this.EffectTechnique.GetPassByIndex(1);
            pass.Apply(renderContext.DeviceContext);
            renderContext.DeviceContext.OutputMerger.SetRenderTargets(dsView, new RenderTargetView[0]);//Remove render target
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(fillStencilState, 1); //Draw backface onto stencil buffer, set value to 1
            OnDraw(renderContext.DeviceContext, InstanceBuffer);

            //Draw full screen quad to fill cross section            
            renderContext.DeviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            renderContext.DeviceContext.Rasterizer.State = RasterState;

            pass = this.EffectTechnique.GetPassByIndex(2);
            pass.Apply(renderContext.DeviceContext);
            renderContext.DeviceContext.OutputMerger.SetRenderTargets(dsView, renderTargets[0]);//Rebind render target
            renderContext.DeviceContext.OutputMerger.SetDepthStencilState(fillCrossSectionStencilState, 1); //Only pass stencil buffer test if value is 1
            renderContext.DeviceContext.Draw(4, 0);

            //Decrement ref count. See OutputMerger.GetRenderTargets remarks
            dsView.Dispose();
            renderTargets[0].Dispose();
        }
    }
}
