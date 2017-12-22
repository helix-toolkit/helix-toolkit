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
    public class LineGeometry3D : Geometry3D
    {                       
        public IEnumerable<Line> Lines
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