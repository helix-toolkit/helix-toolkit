using DemoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelixToolkit.Mathematics;
using HelixToolkit.Wpf.SharpDX;
using System.Numerics;
using System.Threading;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using Matrix = System.Numerics.Matrix4x4;
using HelixToolkit.Wpf.SharpDX.Model;
using Media3D = System.Windows.Media.Media3D;

namespace BatchedMeshDemo
{
    public class MainViewModel : BaseViewModel
    {
        private IList<BatchedMeshGeometryConfig> batchedMeshes;
        public IList<BatchedMeshGeometryConfig> BatchedMeshes
        {
            set
            {
                SetValue(ref batchedMeshes, value);
            }
            get
            {
                return batchedMeshes;
            }
        }

        private IList<Material> batchedMaterial;
        public IList<Material> BatchedMaterials
        {
            set
            {
                SetValue(ref batchedMaterial, value);
            }
            get
            {
                return batchedMaterial;
            }
        }

        public Media3D.Transform3D BatchedTransform
        {
            get;
        } = new Media3D.ScaleTransform3D(0.1, 0.1, 0.1);

        private Geometry3D selectedGeometry;
        public Geometry3D SelectedGeometry
        {
            set
            {
                if(SetValue(ref selectedGeometry, value))
                {
                    SelectedTransform = new Media3D.MatrixTransform3D(BatchedMeshes.Where(x => x.Geometry == value).Select(x => x.ModelTransform).First().ToMatrix3D() * BatchedTransform.Value);
                }
            }
            get { return selectedGeometry; }
        }

        private Media3D.Transform3D selectedTransform;
        public Media3D.Transform3D SelectedTransform
        {
            set
            {
                SetValue(ref selectedTransform, value);
            }
            get { return selectedTransform; }
        }

        public Material MainMaterial { get; } = PhongMaterials.White;

        public Material SelectedMaterial { get; } = new PhongMaterial() { EmissiveColor = Color.Yellow };

        public Geometry3D FloorModel { private set; get; }

        public Material FloorMaterial { private set; get; } = PhongMaterials.Pearl;

        private SynchronizationContext context = SynchronizationContext.Current;

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();
            Camera = new PerspectiveCamera() { Position = new Point3D(0, 0, 200), LookDirection = new Vector3D(0, 0, -200), UpDirection = new Vector3D(0, 1, 0), FarPlaneDistance = 1000 };
            Task.Run(() => { LoadModels(); });
            var builder = new MeshBuilder(true);
            builder.AddBox(new Vector3(0, -65, 0), 600, 1, 600);
            FloorModel = builder.ToMesh();
            (MainMaterial as PhongMaterial).NormalMap = LoadFileToMemory("TextureNoise1_dot3.jpg");
            (MainMaterial as PhongMaterial).RenderShadowMap = true;
            (FloorMaterial as PhongMaterial).RenderShadowMap = true;
        }

        private void LoadModels()
        {
            var models = Load3ds("Car.3DS");
            int count = 0;
            Dictionary<MaterialCore, int> materialDict = new Dictionary<MaterialCore, int>();
            //materialDict.Add(new PhongMaterialCore() { DiffuseColor = new Color4(1, 0, 0, 1) }, count);
            foreach (var model in models)
            {
                if (materialDict.ContainsKey(model.Material))
                {
                    continue;
                }
                materialDict.Add(model.Material, count++);
            }
            var modelList = new List<BatchedMeshGeometryConfig>(models.Count);
            foreach(var model in models)
            {
                model.Geometry.UpdateOctree();
                if(model.Transform != null)
                {
                    foreach(var transform in model.Transform)
                    {
                        modelList.Add(new BatchedMeshGeometryConfig(model.Geometry, transform, materialDict[model.Material]));
                        //modelList.Add(new BatchedMeshGeometryConfig(model.Geometry, transform, 0));
                    }
                }
                else
                {
                    modelList.Add(new BatchedMeshGeometryConfig(model.Geometry, Matrix.Identity, materialDict[model.Material]));
                    //modelList.Add(new BatchedMeshGeometryConfig(model.Geometry, Matrix.Identity, 0));
                }
            }

            Material[] materials = new Material[materialDict.Count];
            foreach(var m in materialDict.Keys)
            {
                materials[materialDict[m]] = m.ConvertToMaterial();
            }
            context.Post((o) => 
            {
                BatchedMeshes = modelList;
                BatchedMaterials = materials;
            }, null);
        }

        public List<Object3D> Load3ds(string path)
        {
            if (path.EndsWith(".obj", StringComparison.CurrentCultureIgnoreCase))
            {
                var reader = new ObjReader();
                var list = reader.Read(path);
                return list;
            }
            else if (path.EndsWith(".3ds", StringComparison.CurrentCultureIgnoreCase))
            {
                var reader = new StudioReader();
                var list = reader.Read(path);
                return list;
            }
            else
            {
                return new List<Object3D>();
            }
        }
    }
}
