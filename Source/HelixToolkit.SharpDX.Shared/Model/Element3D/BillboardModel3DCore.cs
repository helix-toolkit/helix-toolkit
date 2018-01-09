/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using SharpDX;
using SharpDX.Direct3D11;
using System.Collections.Generic;
#if NETFX_CORE
namespace HelixToolkit.UWP.Core
#else
namespace HelixToolkit.Wpf.SharpDX.Core
#endif
{
    public class BillboardModel3DCore : GeometryModel3DCore
    {
        private bool fixedSize = true;
        public bool FixedSize
        {
            set
            {
                if(Set(ref fixedSize, value))
                {
                    InvalidateRender();
                }
            }
            get
            {
                return fixedSize;
            }
        }

        protected override bool OnHitTest(IRenderContext context, Matrix modelMatrix, ref Ray ray, ref List<HitTestResult> hits, IRenderable originalSource)
        {
            return (Geometry as BillboardBase).HitTest(context, modelMatrix, ref ray, ref hits, originalSource, FixedSize);
        }
    }
}