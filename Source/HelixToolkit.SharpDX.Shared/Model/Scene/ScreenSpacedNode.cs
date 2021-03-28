/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System;
using System.Collections.Generic;

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
    namespace Model.Scene
    {
        using Core;
        using Cameras;

        /// <summary>
        /// Screen Spaced node uses a fixed camera to render model (Mainly used for view box and coordinate system rendering) onto screen which is separated from viewport camera.
        /// <para>
        /// Default fix camera is perspective camera with FOV 45 degree and camera distance = 20. Look direction is always looking at (0,0,0).
        /// </para>
        /// <para>
        /// User must properly scale the model to fit into the camera frustum. The usual maximum size is from (5,5,5) to (-5,-5,-5) bounding box.
        /// </para>
        /// <para>
        /// User can use <see cref="ScreenSpacedNode.SizeScale"/> to scale the size of the rendering.
        /// </para>
        /// </summary>
        public class ScreenSpacedNode : GroupNode
        {
            private sealed class ScreenSpacedContext : IRenderMatrices
            {
                public Matrix ViewMatrix
                {
                    set; get;
                }

                public Matrix ViewMatrixInv
                {
                    set; get;
                }

                public Matrix ProjectionMatrix
                {
                    set; get;
                }

                public Matrix ViewportMatrix
                {
                    get
                    {
                        return new Matrix((float)(ActualWidth / 2), 0, 0, 0,
                            0, -(float)(ActualHeight / 2), 0, 0,
                            0, 0, 1, 0,
                            (float)((ActualWidth - 1) / 2), (float)((ActualHeight - 1) / 2), 0, 1);
                    }
                }

                public Matrix ScreenViewProjectionMatrix
                {
                    get; private set;
                }

                public bool IsPerspective { set; get; }

                public float ActualWidth
                {
                    set; get;
                }

                public float ActualHeight
                {
                    set; get;
                }

                public float DpiScale => RenderHost.DpiScale;

                public IRenderHost RenderHost
                {
                    get;
                }

                public CameraCore Camera => RenderHost.RenderContext.Camera;

                public ScreenSpacedContext(IRenderHost host)
                {
                    RenderHost = host;
                }

                public void UpdateScreenViewProjectionMatrix()
                {
                    ScreenViewProjectionMatrix = ViewMatrix * ProjectionMatrix * ViewportMatrix;
                }
            }
            #region Properties
            /// <summary>
            /// Gets or sets the relative screen location x.
            /// </summary>
            /// <value>
            /// The relative screen location x.
            /// </value>
            public float RelativeScreenLocationX
            {
                set
                {
                    (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationX = value;
                }
                get
                {
                    return (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationX;
                }
            }
            /// <summary>
            /// Gets or sets the relative screen location y.
            /// </summary>
            /// <value>
            /// The relative screen location y.
            /// </value>
            public float RelativeScreenLocationY
            {
                set
                {
                    (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationY = value;
                }
                get
                {
                    return (RenderCore as IScreenSpacedRenderParams).RelativeScreenLocationY;
                }
            }
            /// <summary>
            /// Gets or sets the size scale.
            /// </summary>
            /// <value>
            /// The size scale.
            /// </value>
            public float SizeScale
            {
                set
                {
                    (RenderCore as IScreenSpacedRenderParams).SizeScale = value;
                }
                get
                {
                    return (RenderCore as IScreenSpacedRenderParams).SizeScale;
                }
            }
            /// <summary>
            /// Gets or sets the mode. Includes <see cref="ScreenSpacedMode.RelativeScreenSpaced"/> and <see cref="ScreenSpacedMode.AbsolutePosition3D"/>
            /// </summary>
            /// <value>
            /// The mode.
            /// </value>
            public ScreenSpacedMode Mode
            {
                set
                {
                    (RenderCore as IScreenSpacedRenderParams).Mode = value;
                }
                get
                {
                    return (RenderCore as IScreenSpacedRenderParams).Mode;
                }
            }
            /// <summary>
            /// Gets or sets the absolute position. <see cref="ScreenSpacedMode.AbsolutePosition3D"/>
            /// </summary>
            /// <value>
            /// The absolute position.
            /// </value>
            public Vector3 AbsolutePosition3D
            {
                set
                {
                    (RenderCore as IScreenSpacedRenderParams).AbsolutePosition3D = value;
                }
                get
                {
                    return (RenderCore as IScreenSpacedRenderParams).AbsolutePosition3D;
                }
            }
            /// <summary>
            /// Only being used when <see cref="Mode"/> is RelativeScreenSpaced
            /// </summary>
            public ScreenSpacedCameraType CameraType
            {
                set
                {
                    (RenderCore as IScreenSpacedRenderParams).CameraType = value;
                }
                get
                {
                    return (RenderCore as IScreenSpacedRenderParams).CameraType;
                }
            }
            /// <summary>
            /// Gets or sets the far plane for screen spaced camera
            /// </summary>
            /// <value>
            /// The far plane.
            /// </value>
            public float FarPlane
            {
                set
                {
                    (RenderCore as IScreenSpacedRenderParams).FarPlane = (float)value;
                }
                get
                {
                    return (RenderCore as IScreenSpacedRenderParams).FarPlane;
                }
            }
            /// <summary>
            /// Gets or sets the near plane for screen spaced camera
            /// </summary>
            /// <value>
            /// The near plane.
            /// </value>
            public float NearPlane
            {
                set
                {
                    (RenderCore as IScreenSpacedRenderParams).NearPlane = (float)value;
                }
                get
                {
                    return (RenderCore as IScreenSpacedRenderParams).NearPlane;
                }
            }
            #endregion
            /// <summary>
            /// Gets or sets a value indicating whether [need clear depth buffer].
            /// </summary>
            /// <value>
            ///   <c>true</c> if [need clear depth buffer]; otherwise, <c>false</c>.
            /// </value>
            protected bool NeedClearDepthBuffer { set; get; } = true;

            private List<HitTestResult> screenSpaceHits = new List<HitTestResult>();

            private ScreenSpacedContext screenSpacedContext;
            /// <summary>
            /// Called when [create render core].
            /// </summary>
            /// <returns></returns>
            protected override RenderCore OnCreateRenderCore()
            {
                var core = new ScreenSpacedMeshRenderCore();
                core.OnCoordinateSystemChanged += Core_OnCoordinateSystemChanged;
                return core;
            }

            private void Core_OnCoordinateSystemChanged(object sender, BoolArgs e)
            {
                OnCoordinateSystemChanged(e.Value);
            }

            protected virtual void OnCoordinateSystemChanged(bool e)
            {
            }
            /// <summary>
            /// Called when [attach].
            /// </summary>
            /// <param name="host">The host.</param>
            /// <returns></returns>
            protected override bool OnAttach(IRenderHost host)
            {
                RenderCore.Attach(EffectTechnique);
                var screenSpaceCore = RenderCore as ScreenSpacedMeshRenderCore;
                screenSpaceCore.RelativeScreenLocationX = RelativeScreenLocationX;
                screenSpaceCore.RelativeScreenLocationY = RelativeScreenLocationY;
                screenSpaceCore.SizeScale = SizeScale;
                screenSpacedContext = new ScreenSpacedContext(host);
                //for (int i = 0; i < ItemsInternal.Count; ++i)
                //{
                //    ItemsInternal[i].RenderType = RenderType.ScreenSpaced;
                //}
                return base.OnAttach(host);
            }

            /// <summary>
            /// Called when [detach].
            /// </summary>
            protected override void OnDetach()
            {
                RenderCore.Detach();
                base.OnDetach();
            }

            protected override bool OnHitTest(HitTestContext context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
            {
                var newRay = new Ray();
                var hitSP = context.HitPointSP;
                bool preHit = false;
                switch (Mode)
                {
                    case ScreenSpacedMode.RelativeScreenSpaced:
                        preHit = CreateRelativeScreenModeRay(context, out newRay, out hitSP);
                        break;
                    case ScreenSpacedMode.AbsolutePosition3D:
                        preHit = CreateAbsoluteModeRay(context, out newRay, out hitSP);                       
                        break;
                }
                if (!preHit)
                {
                    return false;
                }
                screenSpaceHits.Clear();
                var spHitContext = new HitTestContext(screenSpacedContext, newRay, hitSP);
                if (base.OnHitTest(spHitContext, totalModelMatrix, ref screenSpaceHits))
                {
                    if (hits == null)
                    {
                        hits = new List<HitTestResult>();
                    }
                    hits.Clear();
                    hits.AddRange(screenSpaceHits);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool CreateRelativeScreenModeRay(HitTestContext context, out Ray newRay, out Vector2 hitSP)
            {
                var p = context.HitPointSP * context.RenderMatrices.DpiScale; //Vector3.TransformCoordinate(context.RayWS.Position, context.RenderMatrices.ScreenViewProjectionMatrix);
                var screenSpaceCore = RenderCore as ScreenSpacedMeshRenderCore;
                screenSpacedContext.IsPerspective = screenSpaceCore.IsPerspective;
                float viewportSize = screenSpaceCore.Size * screenSpaceCore.SizeScale * context.RenderMatrices.DpiScale;

                float offx = (float)(context.RenderMatrices.ActualWidth / 2 * (1 + screenSpaceCore.RelativeScreenLocationX) - viewportSize / 2);
                float offy = (float)(context.RenderMatrices.ActualHeight / 2 * (1 - screenSpaceCore.RelativeScreenLocationY) - viewportSize / 2);
                offx = Math.Max(0, Math.Min(offx, (int)(context.RenderMatrices.ActualWidth - viewportSize)));
                offy = Math.Max(0, Math.Min(offy, (int)(context.RenderMatrices.ActualHeight - viewportSize)));

                var px = p.X - offx;
                var py = p.Y - offy;

                if (px < 0 || py < 0 || px > viewportSize || py > viewportSize)
                {
                    newRay = new Ray();
                    hitSP = Vector2.Zero;
                    return false;
                }
                hitSP = new Vector2(px, py) / context.RenderMatrices.DpiScale;
                var viewMatrix = screenSpaceCore.GlobalTransform.View;
                var projMatrix = screenSpaceCore.GlobalTransform.Projection;
                newRay = RayExtensions.UnProject(new Vector2(px, py), ref viewMatrix, ref projMatrix,
                    screenSpaceCore.NearPlane, viewportSize, viewportSize, screenSpaceCore.IsPerspective);
                screenSpacedContext.ViewMatrix = viewMatrix;
                screenSpacedContext.ViewMatrixInv = viewMatrix.PsudoInvert();
                screenSpacedContext.ProjectionMatrix = projMatrix;
                screenSpacedContext.ActualWidth = screenSpacedContext.ActualHeight = viewportSize;
                screenSpacedContext.UpdateScreenViewProjectionMatrix();
                return true;
            }

            private bool CreateAbsoluteModeRay(HitTestContext context, out Ray newRay, out Vector2 hitSP)
            {
                var screenSpaceCore = RenderCore as ScreenSpacedMeshRenderCore;
                screenSpacedContext.IsPerspective = screenSpaceCore.IsPerspective;
                var viewMatrix = screenSpaceCore.GlobalTransform.View;
                var projMatrix = screenSpaceCore.GlobalTransform.Projection;
                screenSpacedContext.ViewMatrix = viewMatrix;
                screenSpacedContext.ViewMatrixInv = viewMatrix.PsudoInvert();
                screenSpacedContext.ProjectionMatrix = projMatrix;
                if (context.RenderMatrices.IsPerspective)
                {
                    hitSP = context.HitPointSP;
                    newRay = RayExtensions.UnProject(hitSP * context.RenderMatrices.DpiScale, ref viewMatrix, ref projMatrix,
                        screenSpaceCore.NearPlane,
                        context.RenderMatrices.ActualWidth, context.RenderMatrices.ActualHeight, screenSpaceCore.IsPerspective);
                    screenSpacedContext.ActualWidth = context.RenderMatrices.ActualWidth;
                    screenSpacedContext.ActualHeight = context.RenderMatrices.ActualHeight;
                    screenSpacedContext.UpdateScreenViewProjectionMatrix();
                    return true;                
                }
                else
                {
                    var p = context.HitPointSP * context.RenderMatrices.DpiScale;
                    float viewportSize = screenSpaceCore.Size * screenSpaceCore.SizeScale * context.RenderMatrices.DpiScale;

                    var abs = Vector3.TransformCoordinate(AbsolutePosition3D, context.RenderMatrices.ScreenViewProjectionMatrix);
                    float offx = (float)(abs.X - viewportSize / 2);
                    float offy = (float)(abs.Y - viewportSize / 2);

                    var px = p.X - offx;
                    var py = p.Y - offy;

                    if (px < 0 || py < 0 || px > viewportSize || py > viewportSize)
                    {
                        newRay = new Ray();
                        hitSP = Vector2.Zero;
                        return false;
                    }
                    hitSP = new Vector2(px, py) / context.RenderMatrices.DpiScale;
                    newRay = RayExtensions.UnProject(new Vector2(px, py), ref viewMatrix, ref projMatrix,
                        screenSpaceCore.NearPlane, viewportSize, viewportSize, screenSpaceCore.IsPerspective);
                    screenSpacedContext.ActualWidth = viewportSize;
                    screenSpacedContext.ActualHeight = viewportSize;
                    screenSpacedContext.ViewMatrix = viewMatrix;
                    screenSpacedContext.ViewMatrixInv = viewMatrix.PsudoInvert();
                    screenSpacedContext.ProjectionMatrix = projMatrix;
                    screenSpacedContext.UpdateScreenViewProjectionMatrix();
                    return true;
                }
            }
        }
    }

}