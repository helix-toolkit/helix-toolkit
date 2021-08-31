/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using System.Collections.Generic;
using System;
#if !NETFX_CORE && !WINUI
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#elif WINUI
namespace HelixToolkit.WinUI
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
            /// Only being used when <see cref="Mode"/> is RelativeScreenSpaced
            /// </summary>
            ScreenSpacedCameraType CameraType
            {
                set; get;
            }

            bool IsPerspective
            {
                get;
            }
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
            /// <summary>
            /// Gets or sets the far plane for screen spaced camera rendering.
            /// </summary>
            /// <value>
            /// The far plane.
            /// </value>
            float FarPlane
            {
                set; get;
            }
            /// <summary>
            /// Gets or sets the near plane for screen spaced camera rendering.
            /// </summary>
            /// <value>
            /// The near plane.
            /// </value>
            float NearPlane
            {
                set; get;
            }
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

            private ScreenSpacedCameraType cameraType = ScreenSpacedCameraType.Auto;
            /// <summary>
            /// Only being used when <see cref="Mode"/> is RelativeScreenSpaced
            /// </summary>
            public ScreenSpacedCameraType CameraType
            {
                set { SetAffectsRender(ref cameraType, value); }
                get { return cameraType; }
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
            /// Gets the near plane.
            /// </summary>
            /// <value>
            /// The near plane.
            /// </value>
            public float NearPlane
            {
                set; get;
            } = 1e-2f;
            /// <summary>
            /// Gets the far plane.
            /// </summary>
            /// <value>
            /// The far plane.
            /// </value>
            public float FarPlane
            {
                set; get;
            } = 1e3f;

            public bool IsPerspective
            {
                private set; get;
            }

            private bool isMainCameraPerspective = false;

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
            protected virtual void OnCreateProjectionMatrix(RenderContext context)
            {
                bool isPerspective = false;
                switch (CameraType)
                {
                    case ScreenSpacedCameraType.Auto:
                        isPerspective = context.IsPerspective;
                        break;
                    case ScreenSpacedCameraType.Perspective:
                        isPerspective = true;
                        break;
                    case ScreenSpacedCameraType.Orthographic:
                        break;
                }
                IsPerspective = isPerspective;
                switch (mode)
                {
                    case ScreenSpacedMode.AbsolutePosition3D:
                        if (IsPerspective)
                        {
                            projectionMatrix = context.Camera.CreateProjectionMatrix(context.ActualWidth / context.ActualHeight, NearPlane, FarPlane);
                        }
                        else
                        {
                            //projectionMatrix = context.ProjectionMatrix;
                            projectionMatrix = CreateProjectionMatrix(context.IsPerspective, IsRightHand,
                                Fov, NearPlane, FarPlane, CameraDistance, CameraDistance);
                        }
                        break;
                    case ScreenSpacedMode.RelativeScreenSpaced:
                        projectionMatrix = CreateProjectionMatrix(isPerspective, IsRightHand, Fov, NearPlane, FarPlane, CameraDistance, CameraDistance);
                        break;
                }
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

            protected void UpdateParameters(RenderContext context, float width, float height)
            {
                var ratio = width / height;
                if (ScreenRatio != ratio || Width != width || Height != height || isMainCameraPerspective != context.IsPerspective)
                {
                    ScreenRatio = ratio;
                    Width = width;
                    Height = height;
                    isMainCameraPerspective = context.IsPerspective;
                    OnCreateProjectionMatrix(context);
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
                        if (context.IsPerspective)
                        {
                            RenderAbsolutePositionPerspective(context, deviceContext);
                        }
                        else
                        {
                            RenderAbsolutePositionOrtho(context, deviceContext);
                        }
                        break;
                }
            }

            private void RenderRelativeScreenSpaced(RenderContext context, DeviceContextProxy deviceContext)
            {
                IsRightHand = !context.Camera.CreateLeftHandSystem;
                float viewportSize = Size * SizeScale * context.DpiScale;
                var globalTrans = context.GlobalTransform;
                UpdateParameters(context, (float)context.ActualWidth, (float)context.ActualHeight);
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
                var globalTrans = context.GlobalTransform;            
                UpdateParameters(context, (float)context.ActualWidth, (float)context.ActualHeight);
                float distance = Size / 2 / SizeScale;
                var viewInv = globalTrans.View.PsudoInvert();
                //Determine new camera position. So the size of the object keeps the same. Decouple it from global zooming
                var pos = Vector3.Normalize(absolutePosition - globalTrans.EyePos);
                Vector3 newPos = absolutePosition - pos * distance;
                newPos -= absolutePosition; // Need to do additional translation, since translation is not in model matrix.
                viewInv.M41 = newPos.X;
                viewInv.M42 = newPos.Y;
                viewInv.M43 = newPos.Z;
                globalTrans.View = viewInv.PsudoInvert();
                globalTrans.EyePos = newPos;                
                float w = context.ActualWidth;
                float h = context.ActualHeight;
                globalTrans.Viewport = new Vector4(w, h, 1f / w, 1f / h);
                // Create new projection matrix with proper near/far field.
                globalTrans.Projection = projectionMatrix;
                globalTrans.ViewProjection = globalTrans.View * globalTrans.Projection;               
                globalTransformCB.Upload(deviceContext, ref globalTrans);
                GlobalTransform = globalTrans;
                deviceContext.SetViewport(0, 0, w, h);
                deviceContext.SetScissorRectangle(0, 0, (int)w, (int)h);
            }

            private void RenderAbsolutePositionOrtho(RenderContext context, DeviceContextProxy deviceContext)
            {
                IsRightHand = !context.Camera.CreateLeftHandSystem;
                float viewportSize = Size * SizeScale * context.DpiScale;
                var globalTrans = context.GlobalTransform;
                UpdateParameters(context, (float)context.ActualWidth, (float)context.ActualHeight);
                globalTrans.View = CreateViewMatrix(context, out globalTrans.EyePos);
                globalTrans.Projection = projectionMatrix;
                globalTrans.ViewProjection = globalTrans.View * globalTrans.Projection;
                globalTrans.Viewport = new Vector4(viewportSize, viewportSize, 1f / viewportSize, 1f / viewportSize);
                globalTransformCB.Upload(deviceContext, ref globalTrans);
                GlobalTransform = globalTrans;
                var svp = context.ScreenViewProjectionMatrix;
                var pos = absolutePosition;
                Vector3.TransformCoordinate(ref pos, ref svp, out Vector3 screenPoint);
                float offX = (screenPoint.X - viewportSize / 2);
                float offY = (screenPoint.Y - viewportSize / 2);
                deviceContext.SetViewport(offX, offY, viewportSize, viewportSize);
                deviceContext.SetScissorRectangle((int)Math.Round(offX), (int)Math.Round(offY), (int)(viewportSize + offX), (int)(viewportSize + offY));
            }
        }
    }

}
