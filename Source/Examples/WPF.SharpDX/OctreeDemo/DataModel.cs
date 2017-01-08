using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctreeDemo
{
    public class DataModel : ObservableObject
    {
        private MeshGeometry3D model = null;
        public MeshGeometry3D Model
        {
            set
            {
                SetValue<MeshGeometry3D>(ref model, value, nameof(Model));             
            }
            get { return model; }
        }

        private Material material;
        public Material Material
        {
            set
            {
                SetValue<Material>(ref material, value, nameof(Material));
            }
            get
            {
                return material;
            }
        }

        private bool highlight = false;
        public bool Highlight
        {
            set
            {
                if (highlight == value) { return; }
                highlight = value;
                if (highlight)
                {
                    Material = PhongMaterials.Yellow;
                }
                else
                {
                    Material = PhongMaterials.Red;
                }
            }
            get
            {
                return highlight;
            }
        }

        public DataModel()
        {
            Material = PhongMaterials.Red;
        }
    }
}
