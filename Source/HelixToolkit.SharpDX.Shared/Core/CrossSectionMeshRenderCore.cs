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

    public class CrossSectionMeshRenderCore : PatchMeshRenderCore, ICrossSectionRenderParams
    {
        #region Shader Variables

        /// <summary>
        /// Used to draw back faced triangles onto stencil buffer
        /// </summary>
        private RasterizerState backfaceRasterState;

        #endregion
        #region Properties
        /// <summary>
        /// Defines the sectionColor
        /// </summary>       
        public Color4 SectionColor
        {
            set
            {
                SetAffectsRender(ref clipParameter.CrossSectionColors, value);
            }
            get { return clipParameter.CrossSectionColors.ToColor4(); }
        }

        /// <summary>
        /// Defines the plane1Enabled
        /// </summary>
        public bool Plane1Enabled
        {
            set
            {
                if(clipParameter.EnableCrossPlane.X != value)
                {
                    clipParameter.EnableCrossPlane.X = value;
                    InvalidateRenderer();
                }
            }
            get { return clipParameter.EnableCrossPlane.X; }
        }

        /// <summary>
        /// Defines the plane2Enabled
        /// </summary>
        public bool Plane2Enabled
        {
            set
            {
                if (clipParameter.EnableCrossPlane.Y != value)
                {
                    clipParameter.EnableCrossPlane.Y = value;
                    InvalidateRenderer();
                }
            }
            get { return clipParameter.EnableCrossPlane.Y; }
        }

        /// <summary>
        /// Defines the plane3Enabled
        /// </summary>
        public bool Plane3Enabled
        {
            set
            {
                if (clipParameter.EnableCrossPlane.Z != value)
                {
                    clipParameter.EnableCrossPlane.Z = value;
                    InvalidateRenderer();
                }
            }
            get { return clipParameter.EnableCrossPlane.Z; }
        }

        /// <summary>
        /// Defines the plane4Enabled
        /// </summary>
        public bool Plane4Enabled
        {
            set
            {
                if (clipParameter.EnableCrossPlane.W != value)
                {
                    clipParameter.EnableCrossPlane.W = value;
                    InvalidateRenderer();
                }
            }
            get { return clipParameter.EnableCrossPlane.W; }
        }

        /// <summary>
        /// Defines the plane 1(Normal + d)
        /// </summary>
        public Vector4 Plane1Params
        {
            set
            {
                if(clipParameter.CrossPlaneParams.Row1 != value)
                {
                    clipParameter.CrossPlaneParams.Row1 = value;
                    InvalidateRenderer();
                }
            }
            get
            {
                return clipParameter.CrossPlaneParams.Row1;
            }
        }

        /// <summary>
        /// Defines the plane 2(Normal + d)
        /// </summary>
        public Vector4 Plane2Params
        {
            set
            {
                if (clipParameter.CrossPlaneParams.Row2 != value)
                {
                    clipParameter.CrossPlaneParams.Row2 = value;
                    InvalidateRenderer();
                }
            }
            get
            {
                return clipParameter.CrossPlaneParams.Row2;
            }
        }

        /// <summary>
        /// Defines the plane 3(Normal + d)
        /// </summary>
        public Vector4 Plane3Params
        {
            set
            {
                if (clipParameter.CrossPlaneParams.Row3 != value)
                {
                    clipParameter.CrossPlaneParams.Row3 = value;
                    InvalidateRenderer();
                }
            }
            get
            {
                return clipParameter.CrossPlaneParams.Row3;
            }
        }

        /// <summary>
        /// Defines the plane 4(Normal + d)
        /// </summary>
        public Vector4 Plane4Params
        {
            set
            {
                if (clipParameter.CrossPlaneParams.Row4 != value)
                {
                    clipParameter.CrossPlaneParams.Row4 = value;
                    InvalidateRenderer();
                }
            }
            get
            {
                return clipParameter.CrossPlaneParams.Row4;
            }
        }

        #endregion

        private ClipPlaneStruct clipParameter = new ClipPlaneStruct() { EnableCrossPlane = new Bool4(false, false, false, false), CrossSectionColors = Color.DarkGray.ToVector4(), CrossPlaneParams = new Matrix() };

        private IConstantBufferProxy clipParamCB;

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
