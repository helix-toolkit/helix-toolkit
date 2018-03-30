/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX;
using SharpDX.Direct3D11;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Utilities;
    using Render;
    using Shaders;
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="MODELSTRUCT"></typeparam>
    public abstract class GeometryRenderCore<MODELSTRUCT> : RenderCoreBase<MODELSTRUCT>, IGeometryRenderCore where MODELSTRUCT : struct
    {
        private RasterizerStateProxy rasterState = null;
        /// <summary>
        /// 
        /// </summary>
        public RasterizerStateProxy RasterState { get { return rasterState; } }
        /// <summary>
        /// 
        /// </summary>
        public InputLayout VertexLayout { private set; get; }
        private IElementsBufferModel instanceBuffer;
        /// <summary>
        /// 
        /// </summary>
        public IElementsBufferModel InstanceBuffer
        {
            set
            {
                if (instanceBuffer != value)
                {
                    if (instanceBuffer != null)
                    {
                        instanceBuffer.OnElementChanged -= InvalidateRenderEvent;
                    }
                    instanceBuffer = value;
                    if (instanceBuffer != null)
                    {
                        instanceBuffer.OnElementChanged += InvalidateRenderEvent;
                    }
                }
            }
            get
            {
                return instanceBuffer;   
            }
        }

        private IGeometryBufferModel geometryBuffer;
        /// <summary>
        /// 
        /// </summary>
        public IGeometryBufferModel GeometryBuffer
        {
            set
            {
                if(geometryBuffer == value)
                {
                    return;
                }
                if(geometryBuffer != null)
                {
                    geometryBuffer.OnInvalidateRender -= InvalidateRenderEvent;
                }
                geometryBuffer = value;
                if (geometryBuffer != null)
                {
                    geometryBuffer.OnInvalidateRender += InvalidateRenderEvent;
                }
            }
            get { return geometryBuffer; }
        }

        private RasterizerStateDescription rasterDescription = new RasterizerStateDescription()
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.None,
        };
        /// <summary>
        /// 
        /// </summary>
        public RasterizerStateDescription RasterDescription
        {
            set
            {
                if(SetAffectsRender(ref rasterDescription, value))
                {
                    CreateRasterState(value, false);
                }
            }
            get
            {
                return rasterDescription;
            }
        }

        private string defaultPassName = DefaultPassNames.Default;
        /// <summary>
        /// Name of the default pass inside a technique.
        /// <para>Default: <see cref="DefaultPassNames.Default"/></para>
        /// </summary>
        public string DefaultShaderPassName
        {
            set
            {
                if(Set(ref defaultPassName, value) && IsAttached)
                {
                    DefaultShaderPass = EffectTechnique[value];
                }
            }
            get
            {
                return defaultPassName;
            }
        }

        private string defaultShadowPassName = DefaultPassNames.ShadowPass;
        /// <summary>
        /// 
        /// </summary>
        public string DefaultShadowPassName
        {
            set
            {
                if (Set(ref defaultShadowPassName, value) && IsAttached)
                {
                    ShadowPass = EffectTechnique[value];
                }
            }
            get
            {
                return defaultShadowPassName;
            }
        }
        private IShaderPass defaultShaderPass = null;
        /// <summary>
        /// 
        /// </summary>
        protected IShaderPass DefaultShaderPass
        {
            private set
            {
                if(Set(ref defaultShaderPass, value))
                {
                    OnDefaultPassChanged(value);
                    InvalidateRenderer();
                }
            }
            get
            {
                return defaultShaderPass;
            }
        }

        private IShaderPass shadowPass = null;
        /// <summary>
        /// 
        /// </summary>
        protected IShaderPass ShadowPass
        {
            private set
            {
                if(Set(ref shadowPass, value))
                {
                    OnShadowPassChanged(value);
                    InvalidateRenderer();
                }
            }
            get
            {
                return shadowPass;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryRenderCore{MODELSTRUCT}"/> class.
        /// </summary>
        public GeometryRenderCore() : base(RenderType.Opaque) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryRenderCore{MODELSTRUCT}"/> class.
        /// </summary>
        /// <param name="renderType">Type of the render.</param>
        public GeometryRenderCore(RenderType renderType) : base(renderType) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="force"></param>
        /// <returns></returns>
        protected virtual bool CreateRasterState(RasterizerStateDescription description, bool force)
        {
            RemoveAndDispose(ref rasterState);
            rasterDescription = description;
            if (!IsAttached && !force)
            { return false; }
            rasterState = Collect(EffectTechnique.EffectsManager.StateManager.Register(description));
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="technique"></param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                DefaultShaderPass = technique[DefaultShaderPassName];
                ShadowPass = technique[DefaultShadowPassName];
                this.VertexLayout = technique.Layout;
                CreateRasterState(rasterDescription, true);       
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pass"></param>
        protected virtual void OnDefaultPassChanged(IShaderPass pass) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pass"></param>
        protected virtual void OnShadowPassChanged(IShaderPass pass) { }
        /// <summary>
        /// Set all necessary states and buffers
        /// </summary>
        /// <param name="context"></param>
        protected override void OnBindRasterState(DeviceContextProxy context)
        {
            context.SetRasterState(rasterState);
        }
        /// <summary>
        /// Attach vertex buffer routine
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vertStartSlot"></param>
        protected override void OnAttachBuffers(DeviceContext context, ref int vertStartSlot)
        {
            GeometryBuffer.AttachBuffers(context, this.VertexLayout, ref vertStartSlot, EffectTechnique.EffectsManager);
            InstanceBuffer?.AttachBuffer(context, ref vertStartSlot);           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override bool CanRender(IRenderContext context)
        {
            return base.CanRender(context) && GeometryBuffer != null;
        }

        /// <summary>
        /// Draw call
        /// </summary>
        /// <param name="context"></param>
        /// <param name="instanceModel"></param>
        protected virtual void OnDraw(DeviceContext context, IElementsBufferModel instanceModel)
        {
            if (GeometryBuffer.IndexBuffer != null)
            {
                if (instanceModel == null || !instanceModel.HasElements)
                {
                    context.DrawIndexed(GeometryBuffer.IndexBuffer.ElementCount, GeometryBuffer.IndexBuffer.Offset, 0);
                }
                else
                {
                    context.DrawIndexedInstanced(GeometryBuffer.IndexBuffer.ElementCount, instanceModel.Buffer.ElementCount, GeometryBuffer.IndexBuffer.Offset, 0, instanceModel.Buffer.Offset);
                }
            }
            else if (GeometryBuffer.VertexBuffer.Length > 0)
            {
                if (instanceModel == null || !instanceModel.HasElements)
                {
                    context.Draw(GeometryBuffer.VertexBuffer[0].ElementCount, 0);
                }
                else
                {
                    context.DrawInstanced(GeometryBuffer.VertexBuffer[0].ElementCount, instanceModel.Buffer.ElementCount,
                        0, instanceModel.Buffer.Offset);
                }
            }
        }

        protected override void OnRenderShadow(IRenderContext context, DeviceContextProxy deviceContext)
        {
            if (!IsThrowingShadow) { return; }
            ShadowPass.BindShader(deviceContext);
            ShadowPass.BindStates(deviceContext, StateType.BlendState | StateType.DepthStencilState);
            OnDraw(deviceContext, InstanceBuffer);
        }

        protected override void OnRenderCustom(IRenderContext context, DeviceContextProxy deviceContext, IShaderPass shaderPass)
        {
            OnDraw(deviceContext, InstanceBuffer);
        }

        protected void InvalidateRenderEvent(object sender, EventArgs e)
        {
            InvalidateRenderer();
        }
    }
}
