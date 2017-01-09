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
        private Material orgMaterial;
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
                    orgMaterial = material;
                    Material = PhongMaterials.Yellow;
                }
                else
                {
                    Material = orgMaterial;
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
