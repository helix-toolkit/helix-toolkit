/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.Mathematics;
using SharpDX.Direct3D11;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Render;
    using Shaders;
    using Utilities;
    using Components;
    public class CrossSectionMeshRenderCore : MeshRenderCore, ICrossSectionRenderParams
    {
        private ClipPlaneStruct clipParameter = new ClipPlaneStruct() { EnableCrossPlane = new Bool4(false, false, false, false), CrossSectionColors = Color.Blue.ToVector4(), CrossPlaneParams = new Matrix() };
        #region Shader Variables
        private ShaderPass drawBackfacePass;
        private ShaderPass drawScreenQuadPass;
        /// <summary>
        /// Used to draw back faced triangles onto stencil buffer
        /// </summary>
        private RasterizerStateProxy backfaceRasterState;

        private readonly ConstantBufferComponent clipParamCB;
        #endregion
        #region Properties
        private CuttingOperation cuttingOperation = CuttingOperation.Intersect;
        /// <summary>
        /// Gets or sets the cutting operation.
        /// </summary>
        /// <value>
        /// The cutting operation.
        /// </value>
        public CuttingOperation CuttingOperation
        {
            set
            {
                if(SetAffectsRender(ref cuttingOperation, value))
                {
                    clipParameter.CuttingOperation = (int)value;
                }
            }
            get { return cuttingOperation; }
        }
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
                if(clipParameter.CrossPlaneParams.Row1() != value)
                {
                    MatrixHelper.SetRow1(ref clipParameter.CrossPlaneParams, value);
                    InvalidateRenderer();
                }
            }
            get
            {
                return clipParameter.CrossPlaneParams.Row1();
            }
        }

        /// <summary>
        /// Defines the plane 2(Normal + d)
        /// </summary>
        public Vector4 Plane2Params
        {
            set
            {
                if (clipParameter.CrossPlaneParams.Row2() != value)
                {
                    MatrixHelper.SetRow2(ref clipParameter.CrossPlaneParams, value);
                    InvalidateRenderer();
                }
            }
            get
            {
                return clipParameter.CrossPlaneParams.Row2();
            }
        }

        /// <summary>
        /// Defines the plane 3(Normal + d)
        /// </summary>
        public Vector4 Plane3Params
        {
            set
            {
                if (clipParameter.CrossPlaneParams.Row3() != value)
                {
                    MatrixHelper.SetRow3(ref clipParameter.CrossPlaneParams, value);
                    InvalidateRenderer();
                }
            }
            get
            {
                return clipParameter.CrossPlaneParams.Row3();
            }
        }

        /// <summary>
        /// Defines the plane 4(Normal + d)
        /// </summary>
        public Vector4 Plane4Params
        {
            set
            {
                if (clipParameter.CrossPlaneParams.Row4() != value)
                {
                    MatrixHelper.SetRow4(ref clipParameter.CrossPlaneParams, value);
                    InvalidateRenderer();
                }
            }
            get
            {
                return clipParameter.CrossPlaneParams.Row4();
            }
        }

        #endregion

        public CrossSectionMeshRenderCore()
        {
            clipParamCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.ClipParamsCB, ClipPlaneStruct.SizeInBytes)));
        }

        protected override bool OnAttach(IRenderTechnique technique)
        {
            if (base.OnAttach(technique))
            {
                drawBackfacePass = technique[DefaultPassNames.Backface];
                drawScreenQuadPass = technique[DefaultPassNames.ScreenQuad];
                return true;
            }
            else { return false; }
        }

        protected override void OnDetach()
        {
            backfaceRasterState = null;
            base.OnDetach();
        }

        protected override bool CreateRasterState(RasterizerStateDescription description, bool force)
        {
            if(!base.CreateRasterState(description, force))
            {
                return false;
            }
            #region Create states
            RemoveAndDispose(ref backfaceRasterState);
            this.backfaceRasterState = Collect(EffectTechnique.EffectsManager.StateManager.Register(new RasterizerStateDescription()
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

        protected override void OnRender(RenderContext renderContext, DeviceContextProxy deviceContext)
        {
            clipParamCB.Upload(deviceContext, ref clipParameter);
            base.OnRender(renderContext, deviceContext);
            // Draw backface into stencil buffer
            var dsView = renderContext.RenderHost.DepthStencilBufferView;
            deviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Stencil, 0, 0);
            deviceContext.SetDepthStencilOnly(dsView);//Remove render target
            deviceContext.SetRasterState(backfaceRasterState);
            drawBackfacePass.BindShader(deviceContext);
            drawBackfacePass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            DrawIndexed(deviceContext, GeometryBuffer.IndexBuffer, InstanceBuffer);

            //Draw full screen quad to fill cross section            
            deviceContext.SetRasterState(RasterState);
            drawScreenQuadPass.BindShader(deviceContext);
            drawScreenQuadPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            renderContext.RenderHost.SetDefaultRenderTargets(false);//Rebind render target
            deviceContext.Draw(4, 0);
        }
    }
}
