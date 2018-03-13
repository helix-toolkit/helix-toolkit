/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Utilities;
    using Render;
    /// <summary>
    /// 
    /// </summary>
    public interface IScreenSpacedRenderParams
    {
        /// <summary>
        /// Relative position X of the center of viewport
        /// </summary>
        float RelativeScreenLocationX { set; get; }
        /// <summary>
        /// Relative position Y of the center of viewport
        /// </summary>
        float RelativeScreenLocationY { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float SizeScale { set; get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsPerspective { set; get; }
        /// <summary>
        /// 
        /// </summary>
        bool IsRightHand { set; get; }
        /// <summary>
        /// 
        /// </summary>
        float Width { get; }
        /// <summary>
        /// 
        /// </summary>
        float Height { get; }
        /// <summary>
        /// 
        /// </summary>
        float Size { get; }
        /// <summary>
        /// 
        /// </summary>
        float ScreenRatio { get; }
        /// <summary>
        /// 
        /// </summary>
        float Fov { get; }
        /// <summary>
        /// 
        /// </summary>
        float CameraDistance { get; }
        /// <summary>
        /// 
        /// </summary>
        GlobalTransformStruct GlobalTransform { get; }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="context"></param>
        ///// <param name="clearDepthBuffer"></param>
        //void SetScreenSpacedCoordinates(IRenderContext context, DeviceContextProxy deviceContext, bool clearDepthBuffer);
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="context"></param>
        //void SetScreenSpacedCoordinates(IRenderContext context, DeviceContextProxy deviceContext);
    }
    /// <summary>
    /// Used to change view matrix and projection matrix to screen spaced coordinate system.
    /// <para>Usage: Call SetScreenSpacedCoordinates(RenderHost) to move coordinate system. Call other render functions for sub models. Finally call RestoreCoordinates(RenderHost) to restore original coordinate system.</para>
    /// </summary>
    public class ScreenSpacedMeshRenderCore : RenderCoreBase<ModelStruct>, IScreenSpacedRenderParams
    {
        private IConstantBufferProxy globalTransformCB;
        private Matrix projectionMatrix;
        public GlobalTransformStruct GlobalTransform { private set; get; }
        public float ScreenRatio { private set; get; } = 1f;
        private float relativeScreenLocX = -0.8f;
        /// <summary>
        /// Relative position X of the center of viewport
        /// </summary>
        public float RelativeScreenLocationX
        {
            set
            {
                SetAffectsRender(ref relativeScreenLocX, value);
            }
            get
            {
                return relativeScreenLocX;
            }
        }

        private float relativeScreenLocY = -0.8f;
        /// <summary>
        ///  Relative position Y of the center of viewport
        /// </summary>
        public float RelativeScreenLocationY
        {
            set
            {
                SetAffectsRender(ref relativeScreenLocY, value);
            }
            get
            {
                return relativeScreenLocY;
            }
        }
        
        private float sizeScale = 1;
        /// <summary>
        /// Size scaling
        /// </summary>
        public float SizeScale
        {
            set
            {
                SetAffectsRender(ref sizeScale, value);
            }
            get
            {
                return sizeScale;
            }
        }

        private bool isPerspective = true;
        /// <summary>
        /// 
        /// </summary>
        public bool IsPerspective
        {
            set { SetAffectsRender(ref isPerspective, value); }
            get { return isPerspective; }
        }

        private bool isRightHand = true;
        /// <summary>
        /// 
        /// </summary>
        public bool IsRightHand
        {
            set { SetAffectsRender(ref isRightHand, value); }
            get { return isRightHand; }
        }
        /// <summary>
        /// Viewport Width
        /// </summary>
        public float Width { private set; get; }
        /// <summary>
        /// Viewport Height
        /// </summary>
        public float Height { private set; get; }

        /// <summary>
        /// Default size. To scale, use <see cref="SizeScale"/>
        /// </summary>
        public float Size { get; } = 100;
        /// <summary>
        /// 
        /// </summary>
        public float CameraDistance { get; } = 20;
        /// <summary>
        /// Fov in radian
        /// </summary>
        public float Fov { get; } = (float)(45 * Math.PI / 180);

        private RasterizerStateProxy rasterState;

        private RasterizerStateDescription rasterDescription = new RasterizerStateDescription()
        {
            FillMode = FillMode.Solid,
            CullMode = CullMode.Back,
        };
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
                return RasterDescription;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenSpacedMeshRenderCore"/> class.
        /// </summary>
        public ScreenSpacedMeshRenderCore() : base(RenderType.ScreenSpaced) { }

        /// <summary>
        /// Gets the model constant buffer description.
        /// </summary>
        /// <returns></returns>
        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ModelCB, ModelStruct.SizeInBytes);
        }

        /// <summary>
        /// Creates the state of the raster.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="force">if set to <c>true</c> [force].</param>
        /// <returns></returns>
        protected virtual bool CreateRasterState(RasterizerStateDescription description, bool force)
        {
            if (!IsAttached && !force)
            { return false; }
            RemoveAndDispose(ref rasterState);
            rasterState = Collect(EffectTechnique.EffectsManager.StateManager.Register(description));
            return true;
        }

        /// <summary>
        /// Called when [attach].
        /// </summary>
        /// <param name="technique">The technique.</param>
        /// <returns></returns>
        protected override bool OnAttach(IRenderTechnique technique)
        {
            if(base.OnAttach(technique))
            {
                globalTransformCB = technique.EffectsManager.ConstantBufferPool.Register(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes);
                CreateRasterState(rasterDescription, true);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Called when [bind raster state].
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void OnBindRasterState(DeviceContextProxy context)
        {
            context.SetRasterState(rasterState);
        }
        /// <summary>
        /// Creates the view matrix.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="eye">The eye.</param>
        /// <returns></returns>
        protected Matrix CreateViewMatrix(IRenderContext renderContext, out Vector3 eye)
        {
            eye = -renderContext.Camera.LookDirection.Normalized() * CameraDistance;
            if (IsRightHand)
            {
                return Matrix.LookAtRH(eye, Vector3.Zero, renderContext.Camera.UpDirection);
            }
            else
            {
                return Matrix.LookAtLH(eye, Vector3.Zero, renderContext.Camera.UpDirection);
            }
        }
        /// <summary>
        /// Called when [create projection matrix].
        /// </summary>
        /// <param name="scale">The scale.</param>
        protected virtual void OnCreateProjectionMatrix(float scale)
        {
            if (IsPerspective)
            {
                projectionMatrix = IsRightHand ? Matrix.PerspectiveFovRH(Fov, 1, 0.001f, 100f) : Matrix.PerspectiveFovLH(Fov, 1, 0.1f, 100f);
            }
            else
            {
                projectionMatrix = IsRightHand ? Matrix.OrthoRH(CameraDistance, CameraDistance, 0.1f, 100f) : Matrix.OrthoLH(CameraDistance, CameraDistance, 0.1f, 100f);
            }
        }
        /// <summary>
        /// Updates the projection matrix.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        protected void UpdateProjectionMatrix(float width, float height)
        {
            var ratio = width / height;
            if (ScreenRatio != ratio || Width != width || Height != height)
            {
                ScreenRatio = ratio;
                Width = width;
                Height = height;
                OnCreateProjectionMatrix(SizeScale);
            }
        }
        /// <summary>
        /// Called when [update per model structure].
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="context">The context.</param>
        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
            model.World = Matrix.Identity;
            model.HasInstances = 0;
        }
        /// <summary>
        /// Determines whether this instance can render the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if this instance can render the specified context; otherwise, <c>false</c>.
        /// </returns>
        protected override bool CanRender(IRenderContext context)
        {
            return context.ActualWidth > Size && context.ActualHeight > Size;
        }
        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected override void OnRender(IRenderContext renderContext, DeviceContextProxy deviceContext)
        {
            SetScreenSpacedCoordinates(renderContext, deviceContext);
        }
        /// <summary>
        /// Posts the render.
        /// </summary>
        /// <param name="context">The context.</param>
        protected override void PostRender(IRenderContext context)
        {
        }

        /// <summary>
        /// Sets the screen spaced coordinates.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        public void SetScreenSpacedCoordinates(IRenderContext context, DeviceContextProxy deviceContext)
        {
            SetScreenSpacedCoordinates(context, deviceContext, true);
        }

        /// <summary>
        /// Sets the screen spaced coordinates.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="clearDepthBuffer">if set to <c>true</c> [clear depth buffer].</param>
        protected virtual void SetScreenSpacedCoordinates(IRenderContext context, DeviceContextProxy deviceContext, bool clearDepthBuffer)
        {
            context.WorldMatrix = Matrix.Identity;
            DepthStencilView dsView;
            if (clearDepthBuffer)
            {
                deviceContext.DeviceContext.OutputMerger.GetRenderTargets(out dsView);
                if (dsView == null)
                {
                    return;
                }

                deviceContext.DeviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Depth, 1f, 0);
                dsView.Dispose();
            }

            float viewportSize = Size * SizeScale;
            var globalTrans = context.GlobalTransform;
            UpdateProjectionMatrix((float)context.ActualWidth, (float)context.ActualHeight);
            globalTrans.View = CreateViewMatrix(context, out globalTrans.EyePos);
            globalTrans.Projection = projectionMatrix;
            globalTrans.ViewProjection = globalTrans.View * globalTrans.Projection;
            globalTrans.Viewport = new Vector4(viewportSize, viewportSize, 0, 0);
            globalTransformCB.UploadDataToBuffer(deviceContext, ref globalTrans);
            GlobalTransform = globalTrans;
            int offX = (int)(Width / 2 * (1 + RelativeScreenLocationX) - viewportSize / 2);
            int offY = (int)(Height / 2 * (1 - RelativeScreenLocationY) - viewportSize / 2);
            deviceContext.DeviceContext.Rasterizer.SetViewport(offX, offY, viewportSize, viewportSize);
            deviceContext.DeviceContext.Rasterizer.SetScissorRectangle(offX, offY, (int)viewportSize + offX, (int)viewportSize + offY);
        }
    }
}
