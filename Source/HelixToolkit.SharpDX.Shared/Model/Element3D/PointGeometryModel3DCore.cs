/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP.Core
#else
namespace HelixToolkit.Wpf.SharpDX.Core
#endif
{
    public class PointGeometryModel3DCore : GeometryModel3DCore
    {
        public float HitTestThickness { set; get; } = 4f;

        protected override bool OnHitTest(IRenderContext context, Matrix modelMatrix, ref Ray ray, ref List<HitTestResult> hits, IRenderable originalSource)
        {
            return (Geometry as PointGeometry3D).HitTest(context, modelMatrix, ref ray, ref hits, originalSource, HitTestThickness);
        }
    }
}
