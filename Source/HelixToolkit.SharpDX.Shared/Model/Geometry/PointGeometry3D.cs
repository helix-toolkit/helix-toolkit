/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

#if NETFX_CORE
namespace HelixToolkit.UWP
#else
namespace HelixToolkit.Wpf.SharpDX
#endif
{
    using System;
    using System.Collections.Generic;

#if !NETFX_CORE
    [Serializable]
#endif
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
#if !NETFX_CORE
            return new PointGeometryOctree(Positions, parameter);
#else
            throw new NotImplementedException();
#endif
        }

        protected override bool CanCreateOctree()
        {
            return Positions != null && Positions.Count > 0;
        }
    }
}
