using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
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

    public class SphereModel : DataModel
    {
        public SphereModel(Vector3 center, int radius)
        {
            Center = center;
            Radius = radius;
            CreateModel();
            isConstructed = true;
        }

        private bool isConstructed = false;

        private Vector3 center;
        public Vector3 Center
        {
            set
            {
                if (SetValue<Vector3>(ref center, value, nameof(Center)))
                {
                    if (!isConstructed)
                    {
                        return;
                    }
                    CreateModel();
                }
            }
            get
            {
                return center;
            }
        }

        private int radius;
        public int Radius
        {
            set
            {
                if (SetValue<int>(ref radius, value, nameof(Radius)))
                {
                    if (!isConstructed)
                    {
                        return;
                    }
                    CreateModel();
                }
            }
            get
            {
                return radius;
            }
        }

        private void CreateModel()
        {
            var builder = new MeshBuilder(true, false, false);
            builder.AddSphere(Center, Radius, 12, 12);
            this.Model = builder.ToMeshGeometry3D();
            this.Model.UpdateOctree();
        }
    }
}
