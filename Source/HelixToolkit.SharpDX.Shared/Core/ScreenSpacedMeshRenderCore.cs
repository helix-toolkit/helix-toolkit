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
    /// Used to change view matrix and projection matrix to screen spaced coordinate system.
    /// <para>Usage: Call SetScreenSpacedCoordinates(RenderHost) to move coordinate system. Call other render functions for sub models. Finally call RestoreCoordinates(RenderHost) to restore original coordinate system.</para>
    /// </summary>
    public class ScreenSpacedMeshRenderCore : RenderCoreBase<ModelStruct>
    {
        private IBufferProxy globalTransformCB;
        private Matrix projectionMatrix;
        public GlobalTransformStruct GlobalTransform { private set; get; }
        public float ScreenRatio = 1f;
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
                OnCreateProjectionMatrix(value);
            }
            get
            {
                return sizeScale;
            }
        }

        public bool IsPerspective { set; get; } = true;
        public bool IsRightHand { set; get; } = true;

        private const int DefaultHeight = 4;
        public float Width { get { return DefaultHeight * ScreenRatio; } }
        public float Height { get { return DefaultHeight; } }

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

        protected Matrix CreateViewMatrix(IRenderMatrices renderContext, out Vector3 eye)
        {
            eye = -renderContext.Camera.LookDirection.ToVector3().Normalized() * 20 / SizeScale;
#if !NETFX_CORE
            if (IsRightHand)
            {
                return Matrix.LookAtRH(
                    eye,
                    Vector3.Zero,
                    renderContext.Camera.UpDirection.ToVector3());
            }
            else
            {
                return Matrix.LookAtLH(eye, Vector3.Zero, renderContext.Camera.UpDirection.ToVector3());
            }
#else
            return Matrix.LookAtRH(-renderContext.Camera.LookDirection.Normalized() * 20, Vector3.Zero, renderContext.Camera.UpDirection);
#endif
        }

        protected virtual void OnCreateProjectionMatrix(float scale)
        {
            if (IsPerspective)
            {
                projectionMatrix = IsRightHand ? Matrix.PerspectiveRH(Width, Height, 1f, 200f) : Matrix.PerspectiveLH(Width, Height, 1f, 200f);
            }
            else
            {
                projectionMatrix = IsRightHand ? Matrix.OrthoRH(Width, Height, 1f, 200f) : Matrix.OrthoLH(Width, Height, 1f, 200f);
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

        protected override void OnUpdatePerModelStruct(ref ModelStruct model, IRenderMatrices context)
        {
            model.World = ModelMatrix * context.WorldMatrix;
            model.HasInstances = 0;
        }

        protected override bool CanRender()
        {
            return false;
        }

        protected override void OnRender(IRenderMatrices renderContext)
        {           
        }

        protected override void PostRender(IRenderMatrices context)
        {
        }

        public virtual void SetScreenSpacedCoordinates(IRenderMatrices context, bool clearDepthBuffer = true)
        {
            DepthStencilView dsView;
            context.DeviceContext.OutputMerger.GetRenderTargets(out dsView);
            if (dsView == null)
            {
                return;
            }

            context.DeviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Depth, 1f, 0);
            dsView.Dispose();
            var globalTrans = context.GlobalTransform;
            UpdateProjectionMatrix(context.ActualWidth, context.ActualHeight);
            globalTrans.View = CreateViewMatrix(context, out globalTrans.EyePos);
            globalTrans.Projection = projectionMatrix;
            globalTrans.ViewProjection = globalTrans.View * globalTrans.Projection;
            globalTransformCB.UploadDataToBuffer(context.DeviceContext, ref globalTrans);
            GlobalTransform = globalTrans;
            context.DeviceContext.Rasterizer.SetViewport(
                (float)context.ActualWidth / 2 * RelativeScreenLocationX, 
                -(float)context.ActualHeight / 2 * RelativeScreenLocationY, 
                (float)context.ActualWidth, (float)context.ActualHeight);
        }
    }
}
