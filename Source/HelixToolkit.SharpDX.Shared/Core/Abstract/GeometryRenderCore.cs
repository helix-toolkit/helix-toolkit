/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using SharpDX.Direct3D11;
using System.Runtime.CompilerServices;
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
    public abstract class GeometryRenderCore : RenderCore, IGeometryRenderCore
    {
        private RasterizerStateProxy rasterState = null;
        /// <summary>
        /// 
        /// </summary>
        public RasterizerStateProxy RasterState { get { return rasterState; } }

        private RasterizerStateProxy invertCullModeState = null;
        public RasterizerStateProxy InvertCullModeState { get { return invertCullModeState; } }

        private IElementsBufferModel instanceBuffer = MatrixInstanceBufferModel.Empty;
        /// <summary>
        /// 
        /// </summary>
        public IElementsBufferModel InstanceBuffer
        {
            set
            {
                var old = instanceBuffer;
                if(SetAffectsCanRenderFlag(ref instanceBuffer, value))
                {
                    if (old != null)
                    {
                        old.ElementChanged -= OnElementChanged;
                    }
                    if (instanceBuffer != null)
                    {
                        instanceBuffer.ElementChanged += OnElementChanged;
                    }
                    else
                    {
                        instanceBuffer = MatrixInstanceBufferModel.Empty;
                    }
                }
            }
            get
            {
                return instanceBuffer;   
            }
        }

        private IAttachableBufferModel geometryBuffer;
        /// <summary>
        /// 
        /// </summary>
        public IAttachableBufferModel GeometryBuffer
        {
            set
            {
                if(SetAffectsCanRenderFlag(ref geometryBuffer, value))
                {
                    OnGeometryBufferChanged(value);
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
                if(SetAffectsRender(ref rasterDescription, value) && IsAttached)
                {
                    CreateRasterState(value, false);
                }
            }
            get
            {
                return rasterDescription;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryRenderCore"/> class.
        /// </summary>
        public GeometryRenderCore() : base(RenderType.Opaque) { }
        /// <summary>
        /// Initializes a new instance of the <see cref="GeometryRenderCore"/> class.
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
            RemoveAndDispose(ref invertCullModeState);
            rasterState = Collect(EffectTechnique.EffectsManager.StateManager.Register(description));
            var invCull = description;
            if(description.CullMode != CullMode.None)
            {
                invCull.CullMode = description.CullMode == CullMode.Back ? CullMode.Front : CullMode.Back;
            }
            invertCullModeState = Collect(EffectTechnique.EffectsManager.StateManager.Register(invCull));
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="technique"></param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderTechnique technique)
        {
            CreateRasterState(rasterDescription, true);       
            return true;
        }

        protected override void OnDetach()
        {
            rasterState = null;
            invertCullModeState = null;
            base.OnDetach();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pass"></param>
        protected virtual void OnDefaultPassChanged(ShaderPass pass) { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pass"></param>
        protected virtual void OnShadowPassChanged(ShaderPass pass) { }
        /// <summary>
        /// Called when [geometry buffer changed].
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        protected virtual void OnGeometryBufferChanged(IAttachableBufferModel buffer) { }

        /// <summary>
        /// Set all necessary states and buffers
        /// </summary>
        /// <param name="context"></param>
        /// <param name="isInvertCullMode"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void OnBindRasterState(DeviceContextProxy context, bool isInvertCullMode)
        {
            context.SetRasterState(!isInvertCullMode ? rasterState : invertCullModeState);                
        }

        /// <summary>
        /// Attach vertex buffer routine
        /// </summary>
        /// <param name="context"></param>
        /// <param name="vertStartSlot"></param>
        protected virtual bool OnAttachBuffers(DeviceContextProxy context, ref int vertStartSlot)
        {
            if(GeometryBuffer.AttachBuffers(context, ref vertStartSlot, EffectTechnique.EffectsManager))
            {
                InstanceBuffer?.AttachBuffer(context, ref vertStartSlot);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Called when [update can render flag].
        /// </summary>
        /// <returns></returns>
        protected override bool OnUpdateCanRenderFlag()
        {
            return base.OnUpdateCanRenderFlag() && GeometryBuffer != null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawIndexed(DeviceContextProxy context, IElementsBufferProxy indexBuffer, IElementsBufferModel instanceModel)
        {
            if (!instanceModel.HasElements)
            {
                context.DrawIndexed(indexBuffer.ElementCount, 0, 0);
            }
            else
            {
                context.DrawIndexedInstanced(indexBuffer.ElementCount, instanceModel.Buffer.ElementCount, 0, 0, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawPoints(DeviceContextProxy context, IElementsBufferProxy vertexBuffer, IElementsBufferModel instanceModel)
        {
            if (!instanceModel.HasElements)
            {
                context.Draw(vertexBuffer.ElementCount, 0);
            }
            else
            {
                context.DrawInstanced(vertexBuffer.ElementCount, instanceModel.Buffer.ElementCount, 0, 0);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool PreRender(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (CanRenderFlag)
            {
                int vertStartSlot = 0;
                if (!OnAttachBuffers(deviceContext, ref vertStartSlot))
                {
                    return false;
                }
                OnBindRasterState(deviceContext, context.IsInvertCullMode);
            }
            return CanRenderFlag;
        }

        /// <summary>
        /// Trigger OnRender function delegate if CanRender()==true
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceContext"></param>
        public sealed override void Render(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (PreRender(context, deviceContext))
            {
                OnRender(context, deviceContext);
            }
        }


        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (PreRender(context, deviceContext))
            {               
                OnRenderShadow(context, deviceContext);
            }
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
            if (PreRender(context, deviceContext))
            {
                OnRenderCustom(context, deviceContext);
            }
        }

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected abstract void OnRender(RenderContext context, DeviceContextProxy deviceContext);

        /// <summary>
        /// Render function for custom shader pass. Used to do special effects
        /// </summary>
        protected abstract void OnRenderCustom(RenderContext context, DeviceContextProxy deviceContext);

        /// <summary>
        /// Called when [render shadow].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext"></param>
        protected abstract void OnRenderShadow(RenderContext context, DeviceContextProxy deviceContext);

        protected void OnElementChanged(object sender, EventArgs e)
        {
            UpdateCanRenderFlag();
            RaiseInvalidateRender();
        }

        protected void OnInvalidateRendererEvent(object sender, EventArgs e)
        {
            RaiseInvalidateRender();
        }
    }
}
