using HelixToolkit.Wpf.SharpDX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelixToolkit.Wpf.SharpDX.Model.Elements3D
{
    [Serializable]
    class BillboardText3D : MeshGeometry3D
    {
        public Vector2Collection Offsets { get; set; }

        
    }
}
