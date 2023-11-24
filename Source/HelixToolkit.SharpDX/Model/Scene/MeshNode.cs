using HelixToolkit.SharpDX.Core;
using SharpDX;
using SharpDX.Direct3D11;

namespace HelixToolkit.SharpDX.Model.Scene;

/// <summary>
///
/// </summary>
public class MeshNode : MaterialGeometryNode, IDynamicReflectable
{
    #region Properties
    private bool frontCCW = true;
    /// <summary>
    /// Gets or sets a value indicating whether [front CCW].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [front CCW]; otherwise, <c>false</c>.
    /// </value>
    public bool FrontCCW
    {
        get
        {
            return frontCCW;
        }
        set
        {
            if (Set(ref frontCCW, value))
            {
                OnRasterStateChanged();
            }
        }
    }

    private CullMode cullMode = CullMode.None;
    /// <summary>
    /// Gets or sets the cull mode.
    /// </summary>
    /// <value>
    /// The cull mode.
    /// </value>
    public CullMode CullMode
    {
        get
        {
            return cullMode;
        }
        set
        {
            if (Set(ref cullMode, value))
            {
                OnRasterStateChanged();
            }
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [invert normal].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [invert normal]; otherwise, <c>false</c>.
    /// </value>
    public bool InvertNormal
    {
        get
        {
            if (RenderCore is IInvertNormal core)
            {
                return core.InvertNormal;
            }

            return false;
        }
        set
        {
            if (RenderCore is IInvertNormal core)
            {
                core.InvertNormal = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the color of the wireframe.
    /// </summary>
    /// <value>
    /// The color of the wireframe.
    /// </value>
    public Color4 WireframeColor
    {
        get
        {
            if (RenderCore is IMeshRenderParams core)
            {
                return core.WireframeColor;
            }

            return Color4.White;
        }
        set
        {
            if (RenderCore is IMeshRenderParams core)
            {
                core.WireframeColor = value;
            }
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether [render wireframe].
    /// </summary>
    /// <value>
    ///   <c>true</c> if [render wireframe]; otherwise, <c>false</c>.
    /// </value>
    public bool RenderWireframe
    {
        get
        {
            if (RenderCore is IMeshRenderParams core)
            {
                return core.RenderWireframe;
            }

            return false;
        }
        set
        {
            if (RenderCore is IMeshRenderParams core)
            {
                core.RenderWireframe = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the dynamic reflector.
    /// </summary>
    /// <value>
    /// The dynamic reflector.
    /// </value>
    public IDynamicReflector? DynamicReflector
    {
        set
        {
            if (RenderCore is IDynamicReflectable core)
            {
                core.DynamicReflector = value;
            }
        }
        get
        {
            if (RenderCore is IDynamicReflectable core)
            {
                return core.DynamicReflector;
            }

            return null;
        }
    }
    #endregion

    /// <summary>
    /// Called when [create render core].
    /// </summary>
    /// <returns></returns>
    protected override RenderCore OnCreateRenderCore()
    {
        return new MeshRenderCore();
    }

    /// <summary>
    /// Called when [create buffer model].
    /// </summary>
    /// <param name="modelGuid"></param>
    /// <param name="geometry"></param>
    /// <returns></returns>
    protected override IAttachableBufferModel OnCreateBufferModel(Guid modelGuid, Geometry3D? geometry)
    {
        var buffer = geometry != null && geometry.IsDynamic ? EffectsManager?.GeometryBufferManager?.Register<DynamicMeshGeometryBufferModel>(modelGuid, geometry)
            : EffectsManager?.GeometryBufferManager?.Register<DefaultMeshGeometryBufferModel>(modelGuid, geometry);
        return buffer ?? EmptyGeometryBufferModel.Empty;
    }

    /// <summary>
    /// Create raster state description.
    /// </summary>
    /// <returns></returns>
    protected override RasterizerStateDescription CreateRasterState()
    {
        return new RasterizerStateDescription()
        {
            FillMode = FillMode,
            CullMode = CullMode,
            DepthBias = DepthBias,
            DepthBiasClamp = -1000,
            SlopeScaledDepthBias = SlopeScaledDepthBias,
            IsDepthClipEnabled = IsDepthClipEnabled,
            IsFrontCounterClockwise = FrontCCW,
            IsMultisampleEnabled = IsMSAAEnabled,
            IsScissorEnabled = !IsThrowingShadow && IsScissorEnabled,
        };
    }

    protected override bool OnCheckGeometry(Geometry3D? geometry)
    {
        return base.OnCheckGeometry(geometry) && geometry is MeshGeometry3D;
    }

    protected override bool OnHitTest(HitTestContext? context, Matrix totalModelMatrix, ref List<HitTestResult> hits)
    {
        if (Geometry is MeshGeometry3D mesh)
        {
            return mesh.HitTest(context, totalModelMatrix, ref hits, this.WrapperSource);
        }

        return false;
    }
}
