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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="clearDepthBuffer"></param>
        void SetScreenSpacedCoordinates(IRenderContext context, bool clearDepthBuffer);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        void SetScreenSpacedCoordinates(IRenderContext context);
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

        private RasterizerState rasterState;

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

        protected override ConstantBufferDescription GetModelConstantBufferDescription()
        {
            return new ConstantBufferDescription(DefaultBufferNames.ModelCB, ModelStruct.SizeInBytes);
        }

        protected virtual bool CreateRasterState(RasterizerStateDescription description, bool force)
        {
            if (!IsAttached && !force)
            { return false; }
            RemoveAndDispose(ref rasterState);
            rasterState = Collect(new RasterizerState(Device, description));
            return true;
        }

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

        protected override void OnBindRasterState(DeviceContext context)
        {
            context.Rasterizer.State = rasterState;
        }

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

        protected void UpdateProjectionMatrix(float width, float height)
        {
            var ratio = width / height;
            if (ScreenRatio != ratio)
            {
                ScreenRatio = ratio;
                OnCreateProjectionMatrix(SizeScale);
            }
        }

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderContext context)
        {
            model.World = ModelMatrix * context.WorldMatrix;
            model.HasInstances = 0;
        }

        protected override bool CanRender(IRenderContext context)
        {
            return true;
        }

        protected override void OnRender(IRenderContext renderContext)
        {
            SetScreenSpacedCoordinates(renderContext);
        }

        protected override void PostRender(IRenderContext context)
        {
        }
        public void SetScreenSpacedCoordinates(IRenderContext context)
        {
            SetScreenSpacedCoordinates(context, true);
        }
        public virtual void SetScreenSpacedCoordinates(IRenderContext context, bool clearDepthBuffer)
        {
            DepthStencilView dsView;
            if (clearDepthBuffer)
            {
                context.DeviceContext.OutputMerger.GetRenderTargets(out dsView);
                if (dsView == null)
                {
                    return;
                }

                context.DeviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Depth, 1f, 0);
                dsView.Dispose();
            }
            Width = (float)context.ActualWidth;
            Height = (float)context.ActualHeight;
            float viewportSize = Size * SizeScale;
            var globalTrans = context.GlobalTransform;
            UpdateProjectionMatrix(Width, Height);
            globalTrans.View = CreateViewMatrix(context, out globalTrans.EyePos);
            globalTrans.Projection = projectionMatrix;
            globalTrans.ViewProjection = globalTrans.View * globalTrans.Projection;
            globalTrans.Viewport = new Vector4(viewportSize, viewportSize, 0, 0);
            globalTransformCB.UploadDataToBuffer(context.DeviceContext, ref globalTrans);
            GlobalTransform = globalTrans;
            int offX = (int)(Width / 2 * (1 + RelativeScreenLocationX) - viewportSize / 2);
            int offY = (int)(Height / 2 * (1 - RelativeScreenLocationY) - viewportSize / 2);
            context.DeviceContext.Rasterizer.SetViewport(offX, offY, viewportSize, viewportSize);
            context.DeviceContext.Rasterizer.SetScissorRectangle(offX, offY, (int)viewportSize, (int)viewportSize);
        }
    }
}
