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
    public interface IScreenSpacedRenderParams
    {
        float RelativeScreenLocationX { set; get; }

        float RelativeScreenLocationY { set; get; }

        float SizeScale { set; get; }

        bool IsPerspective { set; get; }
        bool IsRightHand { set; get; }

        float Width { get; }
        float Height { get; }
        float Size { get; }
        float ScreenRatio { get; }
        void SetScreenSpacedCoordinates(IRenderContext context, bool clearDepthBuffer);
        void SetScreenSpacedCoordinates(IRenderContext context);

        GlobalTransformStruct GlobalTransform { get; }
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
        public float RelativeScreenLocationX
        {
            set
            {
                if (relativeScreenLocX != value)
                {
                    relativeScreenLocX = value;
                }
            }
            get
            {
                return relativeScreenLocX;
            }
        }

        private float relativeScreenLocY = -0.8f;
        public float RelativeScreenLocationY
        {
            set
            {
                if (relativeScreenLocY != value)
                {
                    relativeScreenLocY = value;
                }
            }
            get
            {
                return relativeScreenLocY;
            }
        }

        private float sizeScale = 1;
        public float SizeScale
        {
            set
            {
                if (sizeScale == value)
                {
                    return;
                }
                sizeScale = value;
            }
            get
            {
                return sizeScale;
            }
        }

        public bool IsPerspective { set; get; } = true;
        public bool IsRightHand { set; get; } = true;

        public float Width { private set; get; }
        public float Height { private set; get; }

        public float Size { get; } = 150;

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
                rasterDescription = value;
                CreateRasterState(value, false);
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
#if !NETFX_CORE
            eye = -renderContext.Camera.LookDirection.ToVector3().Normalized() * 20;
            if (IsRightHand)
            {
                return Matrix.LookAtRH(eye, Vector3.Zero, renderContext.Camera.UpDirection.ToVector3());
            }
            else
            {
                return Matrix.LookAtLH(eye, Vector3.Zero, renderContext.Camera.UpDirection.ToVector3());
            }
#else
            eye = -renderContext.Camera.LookDirection.Normalized() * 20;
            if (IsRightHand)
            {
                return Matrix.LookAtRH(eye, Vector3.Zero, renderContext.Camera.UpDirection);
            }
            else
            {
                return Matrix.LookAtLH(eye, Vector3.Zero, renderContext.Camera.UpDirection);
            }
#endif
        }

        protected virtual void OnCreateProjectionMatrix(float scale)
        {
            if (IsPerspective)
            {
                projectionMatrix = IsRightHand ? Matrix.PerspectiveFovRH(45, 1, 0.1f, 100f) : Matrix.PerspectiveFovLH(45, 1, 0.1f, 100f);
            }
            else
            {
                projectionMatrix = IsRightHand ? Matrix.OrthoRH(20, 20, 0.1f, 100f) : Matrix.OrthoLH(20, 20, 0.1f, 100f);
            }
        }

        protected void UpdateProjectionMatrix(double width, double height)
        {
            var ratio = (float)(width / height);
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
            return false;
        }

        protected override void OnRender(IRenderContext renderContext)
        {           
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
            UpdateProjectionMatrix(context.ActualWidth, context.ActualHeight);
            globalTrans.View = CreateViewMatrix(context, out globalTrans.EyePos);
            globalTrans.Projection = projectionMatrix;
            globalTrans.ViewProjection = globalTrans.View * globalTrans.Projection;
            globalTrans.Viewport = new Vector4(viewportSize, viewportSize, 0, 0);
            globalTransformCB.UploadDataToBuffer(context.DeviceContext, ref globalTrans);
            GlobalTransform = globalTrans;

            context.DeviceContext.Rasterizer.SetViewport(
                Width / 2 * (1 + RelativeScreenLocationX) - viewportSize / 2,
                Height / 2 * (1 - RelativeScreenLocationY) - viewportSize / 2,
                viewportSize, viewportSize);
        }
    }
}
