
#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class PointGeometry3D : Geometry3D
    {
        public IEnumerable<Point> Points
        {
            get
            {
                for (int i = 0; i < Positions.Count; ++i)
                {
                    yield return new Point { P0 = Positions[i] };
                }
            }
        }

        protected override IOctree<GeometryModel3D> CreateOctree(OctreeBuildParameter parameter)
        {
            return new PointGeometryOctree(Positions, parameter);
        }

        protected override bool CanCreateOctree()
        {
            return Positions != null && Positions.Count > 0;
        }
    }
}
