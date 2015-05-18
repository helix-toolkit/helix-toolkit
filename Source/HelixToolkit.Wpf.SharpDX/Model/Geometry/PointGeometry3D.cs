namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class PointGeometry3D : Geometry3D
    {
        public IEnumerable<Geometry3D.Point> Points
        {
            get
            {
                for (int i = 0; i < Indices.Count; i += 2)
                {
                    yield return new Point { P0 = Positions[Indices[i]] };
                }
            }
        }
    }
}
