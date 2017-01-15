using DemoCore;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using Media3D = System.Windows.Media.Media3D;

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
        private static readonly Random rnd = new Random();
        public SphereModel(Vector3 center, int radius, bool enableTransform = true)
        {
            Center = center;
            Radius = radius;
            CreateModel();
            isConstructed = true;
            if (enableTransform)
            {
                DynamicTransform = CreateAnimatedTransform1
                  (new Media3D.Vector3D(rnd.Next(-2, 2), rnd.Next(-2, 2), rnd.Next(-2, 2)),
                  new Media3D.Vector3D(rnd.Next(-1,1), rnd.Next(-1, 1), rnd.Next(-1, 1)), rnd.Next(10, 100));
            }
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

        public Media3D.Transform3D DynamicTransform { get; private set; }

        private void CreateModel()
        {
            var builder = new MeshBuilder(true, false, false);
            builder.AddSphere(Center, Radius, 12, 12);
            this.Model = builder.ToMeshGeometry3D();
            //this.Model.UpdateOctree();
        }

        private static Media3D.Transform3D CreateAnimatedTransform1(Media3D.Vector3D translate, Media3D.Vector3D axis, double speed = 4)
        {
            var lightTrafo = new Media3D.Transform3DGroup();
            lightTrafo.Children.Add(new Media3D.TranslateTransform3D(translate));

            var rotateAnimation = new Rotation3DAnimation
            {
                RepeatBehavior = RepeatBehavior.Forever,
                By = new Media3D.AxisAngleRotation3D(axis, 90),
                Duration = TimeSpan.FromSeconds(speed / 4),
                IsCumulative = true,
            };

            var rotateTransform = new Media3D.RotateTransform3D();
            rotateTransform.BeginAnimation(Media3D.RotateTransform3D.RotationProperty, rotateAnimation);
            lightTrafo.Children.Add(rotateTransform);

            return lightTrafo;
        }
    }
}
