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
            /// Gets or sets the absolute position. Used in <see cref="ScreenSpacedMode.AbsolutePosition3D"/>
            /// </summary>
            /// <value>
            /// The absolute position.
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

            private Vector3 absolutePosition;
            public Vector3 AbsolutePosition3D
            {
                set
                {
                    SetAffectsRender(ref absolutePosition, value);
                }
                get
                {
                    return absolutePosition;
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
                    if (Set(ref isRightHand, value))
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
            protected virtual void OnCreateProjectionMatrix()
            {
                projectionMatrix = CreateProjectionMatrix(IsPerspective, IsRightHand, Fov, 0.001f, 100f, CameraDistance, CameraDistance);
            }

            private static Matrix CreateProjectionMatrix(bool isPerspective, bool isRightHand, float fov, float near, float far, float w, float h)
            {
                if (isPerspective)
                {
                    return isRightHand ? Matrix.PerspectiveFovRH(fov, w / h, near, far) : Matrix.PerspectiveFovLH(fov, w / h, near, far);
                }
                else
                {
                    return isRightHand ? Matrix.OrthoRH(w, h, near, far) : Matrix.OrthoLH(w, h, near, far);
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
                    OnCreateProjectionMatrix();
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
                if (context.ActualWidth < Size || context.ActualHeight < Size)
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
                switch (mode)
                {
                    case ScreenSpacedMode.RelativeScreenSpaced:
                        RenderRelativeScreenSpaced(context, deviceContext);
                        break;
                    case ScreenSpacedMode.AbsolutePosition3D:
                        switch (context.IsPerspective)
                        {
                            case true:
                                RenderAbsolutePositionPerspective(context, deviceContext);
                                break;
                            case false:
                                RenderAbsolutePositionOrtho(context, deviceContext);
                                break;
                        }
                        break;
                }
            }

            private void RenderRelativeScreenSpaced(RenderContext context, DeviceContextProxy deviceContext)
            {
                IsRightHand = !context.Camera.CreateLeftHandSystem;
                float viewportSize = Size * SizeScale * context.DpiScale;
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
                offX = (int)(Width / 2 * (1 + RelativeScreenLocationX) - viewportSize / 2);
                offY = (int)(Height / 2 * (1 - RelativeScreenLocationY) - viewportSize / 2);
                offX = Math.Max(0, Math.Min(offX, (int)(Width - viewportSize)));
                offY = Math.Max(0, Math.Min(offY, (int)(Height - viewportSize)));
                deviceContext.SetViewport(offX, offY, viewportSize, viewportSize);
                deviceContext.SetScissorRectangle(offX, offY, (int)viewportSize + offX, (int)viewportSize + offY);
            }

            private void RenderAbsolutePositionPerspective(RenderContext context, DeviceContextProxy deviceContext)
            {
                float distance = Size / 2 / SizeScale;
                var globalTrans = context.GlobalTransform;
                var viewInv = globalTrans.View.PsudoInvert();

                Vector3 newPos;
                //Determine new camera position. So the size of the object keeps the same. Decouple it from global zooming
                var pos = Vector3.Normalize(absolutePosition - globalTrans.EyePos);
                newPos = absolutePosition - pos * distance;
                newPos -= absolutePosition; // Need to do additional translation, since translation is not in model matrix.
                viewInv.M41 = newPos.X;
                viewInv.M42 = newPos.Y;
                viewInv.M43 = newPos.Z;
                globalTrans.View = viewInv.PsudoInvert();
                // Create new projection matrix with proper near/far field.
                float w = globalTrans.Viewport.X;
                float h = globalTrans.Viewport.Y;
                globalTrans.Projection = CreateProjectionMatrix(context.IsPerspective, IsRightHand, 
                    globalTrans.Frustum.X, 0.01f, 500 / SizeScale, w, h);
                globalTrans.ViewProjection = globalTrans.View * globalTrans.Projection;
                globalTrans.EyePos = newPos;
                globalTransformCB.Upload(deviceContext, ref globalTrans);
                GlobalTransform = globalTrans;
                deviceContext.SetViewport(0, 0, context.ActualWidth, context.ActualHeight);
                deviceContext.SetScissorRectangle(0, 0, (int)context.ActualWidth, (int)context.ActualHeight);
            }

            private void RenderAbsolutePositionOrtho(RenderContext context, DeviceContextProxy deviceContext)
            {
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
                float offX = 0;
                float offY = 0;
                var svp = context.ScreenViewProjectionMatrix;
                var pos = absolutePosition;
                Vector3.TransformCoordinate(ref pos, ref svp, out Vector3 screenPoint);
                offX = (screenPoint.X - viewportSize / 2);
                offY = (screenPoint.Y - viewportSize / 2);
                deviceContext.SetViewport(offX, offY, viewportSize, viewportSize);
                deviceContext.SetScissorRectangle((int)Math.Round(offX), (int)Math.Round(offY), (int)(viewportSize + offX), (int)(viewportSize + offY));
            }
        }
    }

}
