using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctreeDemo
{
    public class DataModel
    {
        public MeshGeometry3D Model { set; get; }
        public Material Material { set; get; }

        public DataModel()
        {
            Material = PhongMaterials.Red;
        }
    }
}
