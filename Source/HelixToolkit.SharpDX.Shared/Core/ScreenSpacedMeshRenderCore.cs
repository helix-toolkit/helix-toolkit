/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX.Direct3D11;
using System;
using System.Numerics;
using Matrix = System.Numerics.Matrix4x4;
#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Core
#else
namespace HelixToolkit.UWP.Core
#endif
{
    using Shaders;
    using Utilities;
    using Render;
    using Components;
    using Mathematics;
    /// <summary>
    /// 
    /// </summary>
    public interface IScreenSpacedRenderParams
    {
        event EventHandler<BoolArgs> OnCoordinateSystemChanged;
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
        //void SetScreenSpacedCoordinates(RenderContext context, DeviceContextProxy deviceContext, bool clearDepthBuffer);
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="context"></param>
        //void SetScreenSpacedCoordinates(RenderContext context, DeviceContextProxy deviceContext);
    }
    /// <summary>
    /// Used to change view matrix and projection matrix to screen spaced coordinate system.
    /// <para>Usage: Call SetScreenSpacedCoordinates(RenderHost) to move coordinate system. Call other render functions for sub models. Finally call RestoreCoordinates(RenderHost) to restore original coordinate system.</para>
    /// </summary>
    public class ScreenSpacedMeshRenderCore : RenderCoreBase<ModelStruct>, IScreenSpacedRenderParams
    {
        public event EventHandler<BoolArgs> OnCoordinateSystemChanged;
        
        private readonly ConstantBufferComponent globalTransformCB;
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
            get { return isRightHand; }
            private set
            {
                if(Set(ref isRightHand, value))
                {
                    OnCoordinateSystemChanged?.Invoke(this, value ? BoolArgs.TrueArgs : BoolArgs.FalseArgs);
                }
            }
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
            CullMode = CullMode.None,
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
        public ScreenSpacedMeshRenderCore() : base(RenderType.ScreenSpaced)
        {
            globalTransformCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes)));
        }

        ///// <summary>
        ///// Gets the model constant buffer description.
        ///// </summary>
        ///// <returns></returns>
        //protected override ConstantBufferDescription GetModelConstantBufferDescription()
        //{
        //    return new ConstantBufferDescription(DefaultBufferNames.ModelCB, ModelStruct.SizeInBytes);
        //}

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
               // globalTransformCB = technique.EffectsManager.ConstantBufferPool.Register(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes);
            CreateRasterState(rasterDescription, true);
            return true;
        }

        protected override void OnDetach()
        {
            rasterState = null;
            base.OnDetach();
        }
        /// <summary>
        /// Called when [bind raster state].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="isInvertCullMode"></param>
        protected override void OnBindRasterState(DeviceContextProxy context, bool isInvertCullMode)
        {
            context.SetRasterState(rasterState);
        }
        /// <summary>
        /// Creates the view matrix.
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="eye">The eye.</param>
        /// <returns></returns>
        protected Matrix CreateViewMatrix(RenderContext renderContext, out Vector3 eye)
        {
            eye = -Vector3.Normalize(renderContext.Camera.LookDirection) * CameraDistance;
            if (IsRightHand)
            {
                return Matrix.CreateLookAt(eye, Vector3.Zero, renderContext.Camera.UpDirection);
            }
            else
            {
                return MatrixHelper.LookAtLH(eye, Vector3.Zero, renderContext.Camera.UpDirection);
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
                projectionMatrix = IsRightHand ? Matrix.CreatePerspectiveFieldOfView(Fov, 1, 0.001f, 100f) : MatrixHelper.PerspectiveFovLH(Fov, 1, 0.1f, 100f);
            }
            else
            {
                projectionMatrix = IsRightHand ? Matrix.CreateOrthographic(CameraDistance, CameraDistance, 0.1f, 100f) : MatrixHelper.OrthoLH(CameraDistance, CameraDistance, 0.1f, 100f);
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
        protected override void OnUpdatePerModelStruct(ref ModelStruct model, RenderContext context)
        {
            model.World = Matrix.Identity;
            model.HasInstances = 0;
        }

        /// <summary>
        /// Called when [render].
        /// </summary>
        /// <param name="renderContext">The render context.</param>
        /// <param name="deviceContext">The device context.</param>
        protected override void OnRender(RenderContext renderContext, DeviceContextProxy deviceContext)
        {
            SetScreenSpacedCoordinates(renderContext, deviceContext);
        }

        /// <summary>
        /// Sets the screen spaced coordinates.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        public void SetScreenSpacedCoordinates(RenderContext context, DeviceContextProxy deviceContext)
        {
            SetScreenSpacedCoordinates(context, deviceContext, true);
        }

        /// <summary>
        /// Sets the screen spaced coordinates.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deviceContext">The device context.</param>
        /// <param name="clearDepthBuffer">if set to <c>true</c> [clear depth buffer].</param>
        protected virtual void SetScreenSpacedCoordinates(RenderContext context, DeviceContextProxy deviceContext, bool clearDepthBuffer)
        {
            if(context.ActualWidth < Size || context.ActualHeight < Size)
            {
                return;
            }
            if (clearDepthBuffer)
            {
                deviceContext.GetDepthStencilView(out DepthStencilView dsView);
                if (dsView == null)
                {
                    return;
                }

                deviceContext.ClearDepthStencilView(dsView, DepthStencilClearFlags.Depth, 1f, 0);
                dsView.Dispose();
            }
            IsRightHand = !context.Camera.CreateLeftHandSystem;
            float viewportSize = Size * SizeScale;
            var globalTrans = context.GlobalTransform;
            UpdateProjectionMatrix((float)context.ActualWidth, (float)context.ActualHeight);
            globalTrans.View = CreateViewMatrix(context, out globalTrans.EyePos);
            globalTrans.Projection = projectionMatrix;
            globalTrans.ViewProjection = globalTrans.View * globalTrans.Projection;
            globalTrans.Viewport = new Vector4(viewportSize, viewportSize, 1f / viewportSize, 1f / viewportSize);
            globalTransformCB.Upload(deviceContext, ref globalTrans);
            GlobalTransform = globalTrans;
            int offX = (int)(Width / 2 * (1 + RelativeScreenLocationX) - viewportSize / 2);
            int offY = (int)(Height / 2 * (1 - RelativeScreenLocationY) - viewportSize / 2);
            offX = Math.Max(0, Math.Min(offX, (int)(Width - viewportSize)));
            offY = Math.Max(0, Math.Min(offY, (int)(Height - viewportSize)));
            //if(offX + viewportSize > Width)
            //{
            //    offX = (int)(Width - viewportSize);
            //}
            //else if(offX < 0)
            //{
            //    offX = 0;
            //}
            //if(offY + viewportSize > Height)
            //{
            //    offY = (int)(Height - viewportSize);
            //}
            //else if(offY < 0)
            //{
            //    offY = 0;
            //}
            deviceContext.SetViewport(offX, offY, viewportSize, viewportSize);
            deviceContext.SetScissorRectangle(offX, offY, (int)viewportSize + offX, (int)viewportSize + offY);
        }

        public sealed override void RenderShadow(RenderContext context, DeviceContextProxy deviceContext)
        {
        }

        public sealed override void RenderCustom(RenderContext context, DeviceContextProxy deviceContext)
        {
        }
    }
}
