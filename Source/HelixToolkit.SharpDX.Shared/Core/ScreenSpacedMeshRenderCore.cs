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
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Core
    {
        using Shaders;
        using Utilities;
        using Render;
        using Components;

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
            /// <summary>
            /// Gets or sets the mode.
            /// </summary>
            /// <value>
            /// The mode.
            /// </value>
            ScreenSpacedMode Mode { set; get; }
            /// <summary>
            /// Gets or sets the absolute position in 3d. Use by <see cref="Mode"/> = <see cref="ScreenSpacedMode.AbsolutePosition3D"/>
            /// </summary>
            /// <value>
            /// The absolute position3 d.
            /// </value>
            Vector3 AbsolutePosition3D { set; get; }
        }
        /// <summary>
        /// Used to change view matrix and projection matrix to screen spaced coordinate system.
        /// <para>Usage: Call SetScreenSpacedCoordinates(RenderHost) to move coordinate system. Call other render functions for sub models. Finally call RestoreCoordinates(RenderHost) to restore original coordinate system.</para>
        /// </summary>
        public class ScreenSpacedMeshRenderCore : RenderCore, IScreenSpacedRenderParams
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

            private ScreenSpacedMode mode = ScreenSpacedMode.RelativeScreenSpaced;
            public ScreenSpacedMode Mode
            {
                set
                {
                    SetAffectsRender(ref mode, value);
                }
                get
                {
                    return mode;
                }
            }

            private Vector3 absolutePosition3D = Vector3.Zero;
            public Vector3 AbsolutePosition3D
            {
                set
                {
                    SetAffectsRender(ref absolutePosition3D, value);
                }
                get
                {
                    return absolutePosition3D;
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

            /// <summary>
            /// Initializes a new instance of the <see cref="ScreenSpacedMeshRenderCore"/> class.
            /// </summary>
            public ScreenSpacedMeshRenderCore() : base(RenderType.ScreenSpaced)
            {
                globalTransformCB = AddComponent(new ConstantBufferComponent(new ConstantBufferDescription(DefaultBufferNames.GlobalTransformCB, GlobalTransformStruct.SizeInBytes)));
            }

            protected override bool OnAttach(IRenderTechnique technique)
            {
                return true;
            }

            /// <summary>
            /// Creates the view matrix.
            /// </summary>
            /// <param name="renderContext">The render context.</param>
            /// <param name="eye">The eye.</param>
            /// <returns></returns>
            protected Matrix CreateViewMatrix(RenderContext renderContext, out Vector3 eye)
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
            /// Called when [render].
            /// </summary>
            /// <param name="renderContext">The render context.</param>
            /// <param name="deviceContext">The device context.</param>
            public override void Render(RenderContext renderContext, DeviceContextProxy deviceContext)
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
                int offX = 0;
                int offY = 0;
                switch(mode)
                {
                    case ScreenSpacedMode.RelativeScreenSpaced:
                        offX = (int)(Width / 2 * (1 + RelativeScreenLocationX) - viewportSize / 2);
                        offY = (int)(Height / 2 * (1 - RelativeScreenLocationY) - viewportSize / 2);
                        offX = Math.Max(0, Math.Min(offX, (int)(Width - viewportSize)));
                        offY = Math.Max(0, Math.Min(offY, (int)(Height - viewportSize)));
                        break;
                    case ScreenSpacedMode.AbsolutePosition3D:
                        var svp = context.ScreenViewProjectionMatrix;
                        Vector3.TransformCoordinate(ref absolutePosition3D, ref svp, out Vector3 screenPoint);
                        offX = (int)(screenPoint.X - viewportSize / 2);
                        offY = (int)(Height - screenPoint.Y - viewportSize / 2);
                        break;
                }

                deviceContext.SetViewport(offX, offY, viewportSize, viewportSize);
                deviceContext.SetScissorRectangle(offX, offY, (int)viewportSize + offX, (int)viewportSize + offY);
            }
        }
    }

}
