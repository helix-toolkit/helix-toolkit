﻿using HelixToolkit.SharpDX.Core2D;
using SharpDX.Direct2D1;

namespace HelixToolkit.SharpDX.Model.Scene2D;

public abstract class ShapeNode2D : SceneNode2D
{
    public Brush? Fill
    {
        set
        {
            if (RenderCore is ShapeRenderCore2DBase core)
            {
                core.FillBrush = value;
            }
        }
        get
        {
            if (RenderCore is ShapeRenderCore2DBase core)
            {
                return core.FillBrush;
            }

            return null;
        }
    }

    public Brush? Stroke
    {
        set
        {
            if (RenderCore is ShapeRenderCore2DBase core)
            {
                core.StrokeBrush = value;
            }
        }
        get
        {
            if (RenderCore is ShapeRenderCore2DBase core)
            {
                return core.StrokeBrush;
            }

            return null;
        }
    }

    private CapStyle strokeDashCap = CapStyle.Flat;

    public CapStyle StrokeDashCap
    {
        set
        {
            if (SetAffectsRender(ref strokeDashCap, value))
            {
                strokeStyleChanged = true;
            }
        }
        get
        {
            return strokeDashCap;
        }
    }

    private CapStyle strokeStartLineCap = CapStyle.Flat;

    public CapStyle StrokeStartLineCap
    {
        set
        {
            if (SetAffectsRender(ref strokeStartLineCap, value))
            {
                strokeStyleChanged = true;
            }
        }
        get
        {
            return strokeStartLineCap;
        }
    }

    private CapStyle strokeEndLineCap = CapStyle.Flat;

    public CapStyle StrokeEndLineCap
    {
        set
        {
            if (SetAffectsRender(ref strokeEndLineCap, value))
            {
                strokeStyleChanged = true;
            }
        }
        get
        {
            return strokeEndLineCap;
        }
    }

    private DashStyle strokeDashStyle = DashStyle.Solid;

    public DashStyle StrokeDashStyle
    {
        set
        {
            if (SetAffectsRender(ref strokeDashStyle, value))
            {
                strokeStyleChanged = true;
            }
        }
        get
        {
            return strokeDashStyle;
        }
    }

    private float strokeDashOffset = 0;

    public float StrokeDashOffset
    {
        set
        {
            if (SetAffectsRender(ref strokeDashOffset, value))
            {
                strokeStyleChanged = true;
            }
        }
        get
        {
            return strokeDashOffset;
        }
    }

    private LineJoin strokeLineJoin = LineJoin.Miter;

    public LineJoin StrokeLineJoin
    {
        set
        {
            if (SetAffectsRender(ref strokeLineJoin, value))
            {
                strokeStyleChanged = true;
            }
        }
        get
        {
            return strokeLineJoin;
        }
    }

    private float strokeMiterLimit = 1;

    public float StrokeMiterLimit
    {
        set
        {
            if (SetAffectsRender(ref strokeMiterLimit, value))
            {
                strokeStyleChanged = true;
            }
        }
        get
        {
            return strokeMiterLimit;
        }
    }

    public float StrokeThickness
    {
        set
        {
            if (RenderCore is ShapeRenderCore2DBase core)
            {
                core.StrokeWidth = value * DpiScale;
            }
        }
        get
        {
            if (RenderCore is ShapeRenderCore2DBase core)
            {
                return core.StrokeWidth / DpiScale;
            }

            return DpiScale;
        }
    }

    private float[]? strokeDashArray;

    public float[]? StrokeDashArray
    {
        set
        {
            if (SetAffectsRender(ref strokeDashArray, value))
            {
                strokeStyleChanged = true;
            }
        }
        get
        {
            return strokeDashArray;
        }
    }

    private bool strokeStyleChanged = true;

    protected ShapeRenderCore2DBase? shapeRenderable;

    protected override RenderCore2D CreateRenderCore()
    {
        shapeRenderable = CreateShapeRenderCore();
        return shapeRenderable;
    }

    protected abstract ShapeRenderCore2DBase CreateShapeRenderCore();

    protected override bool OnAttach(IRenderHost host)
    {
        if (base.OnAttach(host))
        {
            strokeStyleChanged = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    public override void Update(RenderContext2D context)
    {
        base.Update(context);
        if (strokeStyleChanged && shapeRenderable is not null)
        {
            shapeRenderable.StrokeStyle = new StrokeStyle(context.DeviceContext.Factory,
                new StrokeStyleProperties()
                {
                    DashCap = this.StrokeDashCap,
                    StartCap = StrokeStartLineCap,
                    EndCap = StrokeEndLineCap,
                    DashOffset = StrokeDashOffset,
                    LineJoin = StrokeLineJoin,
                    MiterLimit = Math.Max(1, (float)StrokeMiterLimit),
                    DashStyle = StrokeDashStyle
                },
                StrokeDashArray == null ? Array.Empty<float>() : StrokeDashArray);
            strokeStyleChanged = false;
        }
    }
}
