using HelixToolkit.SharpDX.Cameras;
using HelixToolkit.SharpDX.Core;
using SharpDX;

namespace HelixToolkit.SharpDX.Model.Scene;

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

        public bool IsPerspective
        {
            set; get;
        }

        public float ActualWidth
        {
            set; get;
        }

        public float ActualHeight
        {
            set; get;
        }

        public float DpiScale => RenderHost?.DpiScale ?? 1.0f;

        public float NearPlane
        {
            set; get;
        }

        public float FarPlane
        {
            set; get;
        }

        public IRenderHost? RenderHost
        {
            set; get;
        }

        public CameraCore? Camera => RenderHost?.RenderContext?.Camera;

        public FrustumCameraParams CameraParams
        {
            private set; get;
        }

        public void Update()
        {
            ScreenViewProjectionMatrix = ViewMatrix * ProjectionMatrix * ViewportMatrix;
            if (Camera != null)
            {
                CameraParams = Camera.CreateCameraParams(ActualWidth / ActualHeight, NearPlane, FarPlane);
            }
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
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                core.RelativeScreenLocationX = value;
            }
        }
        get
        {

            if (RenderCore is IScreenSpacedRenderParams core)
            {
                return core.RelativeScreenLocationX;
            }

            return 0;
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
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                core.RelativeScreenLocationY = value;
            }
        }
        get
        {
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                return core.RelativeScreenLocationY;
            }

            return 0;
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
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                core.SizeScale = value;
            }
        }
        get
        {
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                return core.SizeScale;
            }

            return 1.0f;
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
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                core.Mode = value;
            }
        }
        get
        {
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                return core.Mode;
            }

            return ScreenSpacedMode.RelativeScreenSpaced;
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
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                core.AbsolutePosition3D = value;
            }
        }
        get
        {
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                return core.AbsolutePosition3D;
            }

            return Vector3.Zero;
        }
    }
    /// <summary>
    /// Only being used when <see cref="Mode"/> is RelativeScreenSpaced
    /// </summary>
    public ScreenSpacedCameraType CameraType
    {
        set
        {
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                core.CameraType = value;
            }
        }
        get
        {
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                return core.CameraType;
            }

            return ScreenSpacedCameraType.Auto;
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
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                core.FarPlane = value;
            }
        }
        get
        {
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                return core.FarPlane;
            }

            return 1.0f;
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
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                core.NearPlane = value;
            }
        }
        get
        {
            if (RenderCore is IScreenSpacedRenderParams core)
            {
                return core.NearPlane;
            }

            return 0.0f;
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

    private List<HitTestResult> screenSpaceHits = new();

    private readonly ScreenSpacedContext screenSpacedContext = new();

    public ScreenSpacedNode()
    {
        AffectsGlobalVariable = true;
    }
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

    private void Core_OnCoordinateSystemChanged(object? sender, BoolArgs e)
    {
        OnCoordinateSystemChanged(e.Value);
    }

    protected virtual void OnCoordinateSystemChanged(bool e)
    {
    }

    protected override bool OnAttach(IEffectsManager effectsManager)
    {
        RenderCore.Attach(EffectTechnique);
        var screenSpaceCore = RenderCore as ScreenSpacedMeshRenderCore;
        if (screenSpaceCore is not null)
        {
            screenSpaceCore.RelativeScreenLocationX = RelativeScreenLocationX;
            screenSpaceCore.RelativeScreenLocationY = RelativeScreenLocationY;
            screenSpaceCore.SizeScale = SizeScale;
        }
        return base.OnAttach(effectsManager);
    }

    /// <summary>
    /// Called when [detach].
    /// </summary>
    protected override void OnDetach()
    {
        RenderCore.Detach();
        base.OnDetach();
    }


    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        if (context is null)
        {
            return false;
        }

        screenSpacedContext.RenderHost = context.RenderMatrices?.RenderHost;
        var newRay = new Ray();
        var hitSP = context.HitPointSP;
        var preHit = false;
        screenSpacedContext.NearPlane = NearPlane;
        screenSpacedContext.FarPlane = FarPlane;
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
        screenSpacedContext.Update();
        screenSpaceHits.Clear();
        var spHitContext = new HitTestContext(screenSpacedContext, newRay, hitSP);
        if (base.OnHitTest(spHitContext, totalModelMatrix, ref screenSpaceHits))
        {
            hits ??= new List<HitTestResult>();
            hits.Clear();
            hits.AddRange(screenSpaceHits);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool CreateRelativeScreenModeRay(HitTestContext? context, out Ray newRay, out Vector2 hitSP)
    {
        if (context is null || context.RenderMatrices is null || RenderCore is not ScreenSpacedMeshRenderCore screenSpaceCore)
        {
            newRay = new Ray();
            hitSP = Vector2.Zero;
            return false;
        }

        var p = context.HitPointSP * context.RenderMatrices.DpiScale; //Vector3Helper.TransformCoordinate(context.RayWS.Position, context.RenderMatrices.ScreenViewProjectionMatrix);
        screenSpacedContext.IsPerspective = screenSpaceCore.IsPerspective;
        var viewportSize = screenSpaceCore.Size * screenSpaceCore.SizeScale * context.RenderMatrices.DpiScale;

        var offx = (float)(context.RenderMatrices.ActualWidth / 2 * (1 + screenSpaceCore.RelativeScreenLocationX) - viewportSize / 2);
        var offy = (float)(context.RenderMatrices.ActualHeight / 2 * (1 - screenSpaceCore.RelativeScreenLocationY) - viewportSize / 2);
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
        return true;
    }

    private bool CreateAbsoluteModeRay(HitTestContext? context, out Ray newRay, out Vector2 hitSP)
    {
        if (context is null ||  RenderCore is not ScreenSpacedMeshRenderCore screenSpaceCore || context.RenderMatrices is null)
        {
            newRay = new Ray();
            hitSP = Vector2.Zero;
            return false;
        }

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
            return true;
        }
        else
        {
            var p = context.HitPointSP * context.RenderMatrices.DpiScale;
            var viewportSize = screenSpaceCore.Size * screenSpaceCore.SizeScale * context.RenderMatrices.DpiScale;

            var abs = Vector3Helper.TransformCoordinate(AbsolutePosition3D, context.RenderMatrices.ScreenViewProjectionMatrix);
            var offx = (float)(abs.X - viewportSize / 2);
            var offy = (float)(abs.Y - viewportSize / 2);

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
            return true;
        }
    }
}
