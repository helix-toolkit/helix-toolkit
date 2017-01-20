namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;
    using HelixToolkit.SharpDX.Shared.Utilities;

    [Serializable]
    public class PointGeometry3D : Geometry3D
    {
        public IEnumerable<Geometry3D.Point> Points
        {
            get
            {
                for (int i = 0; i < Positions.Count; ++i)
                {
                    yield return new Point { P0 = Positions[i] };
                }
            }
        }

        protected override IOctree CreateOctree(OctreeBuildParameter parameter)
        {
            return new PointGeometryOctree(Positions, parameter);
        }
    }
}
