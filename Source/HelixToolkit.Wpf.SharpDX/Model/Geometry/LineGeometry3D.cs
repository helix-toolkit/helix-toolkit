namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections.Generic;

    using Point3D = global::SharpDX.Vector3;

    [Serializable]
    public class LineGeometry3D : Geometry3D
    {                       
        public IEnumerable<Geometry3D.Line> Lines
        {
            get
            {
                for (int i = 0; i < Indices.Count; i += 2)
                {
                    yield return new Line { P0 = Positions[Indices[i]], P1 = Positions[Indices[i + 1]], };
                }
            }
        }
    }
}