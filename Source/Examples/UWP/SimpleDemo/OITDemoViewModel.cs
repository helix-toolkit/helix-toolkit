using GalaSoft.MvvmLight;
using HelixToolkit.UWP;
using SharpDX;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleDemoW10
{
    public class OITDemoViewModel : ObservableObject
    {
        public ObservableElement3DCollection ModelGeometry { get; private set; }

        public Matrix Transform { private set; get; } = Matrix.Translation(60, -10, 0);
        public LineGeometry3D GridModel { private set; get; }
        public Matrix GridTransform { private set; get; } = Matrix.Translation(60, -10, 0);

        private bool showWireframe = false;

        public bool ShowWireframe
        {
            set
            {
                if (Set(ref showWireframe, value))
                {
                    foreach (var model in ModelGeometry)
                    {
                        (model as MeshGeometryModel3D).RenderWireframe = value;
                    }
                }
            }
            get
            {
                return showWireframe;
            }
        }

        private SynchronizationContext syncContext = SynchronizationContext.Current;

        public OITDemoViewModel()
        {
            this.ModelGeometry = new ObservableElement3DCollection();
            var packageFolder = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "NITRO_ENGINE.3ds");
            BuildGrid();
            this.ModelGeometry = new ObservableElement3DCollection();
            Task.Run(() => { Load3ds(packageFolder); });          
        }
        private void BuildGrid()
        {
            var builder = new LineBuilder();
            int zOff = -45;
            for (int i = 0; i < 10; ++i)
            {
                for (int j = 0; j < 10; ++j)
                {
                    builder.AddLine(new SharpDX.Vector3(-i * 5, 0, j * 5), new SharpDX.Vector3(i * 5, 0, j * 5));
                    builder.AddLine(new SharpDX.Vector3(-i * 5, 0, -j * 5), new SharpDX.Vector3(i * 5, 0, -j * 5));
                    builder.AddLine(new SharpDX.Vector3(i * 5, 0, -j * 5), new SharpDX.Vector3(i * 5, 0, j * 5));
                    builder.AddLine(new SharpDX.Vector3(-i * 5, 0, -j * 5), new SharpDX.Vector3(-i * 5, 0, j * 5));
                    builder.AddLine(new SharpDX.Vector3(-i * 5, j * 5, zOff), new SharpDX.Vector3(i * 5, j * 5, zOff));
                    builder.AddLine(new SharpDX.Vector3(i * 5, 0, zOff), new SharpDX.Vector3(i * 5, j * 5, zOff));
                    builder.AddLine(new SharpDX.Vector3(-i * 5, 0, zOff), new SharpDX.Vector3(-i * 5, j * 5, zOff));
                }
            }
            GridModel = builder.ToLineGeometry3D();
        }

        public void Load3ds(string path)
        {
            var reader = new StudioReader();
            var objCol = reader.Read(path);
            AttachModelList(objCol);
        }

        public void LoadObj(string path)
        {
            var reader = new ObjReader();
            var objCol = reader.Read(path);
            AttachModelList(objCol);
        }

        public void LoadStl(string path)
        {
            var reader = new StLReader();
            var objCol = reader.Read(path);
            AttachModelList(objCol);
        }

        public void AttachModelList(List<Object3D> objs)
        {
            
            Random rnd = new Random();

            foreach (var ob in objs)
            {
                ob.Geometry.UpdateOctree();
                Task.Delay(100).Wait();
                syncContext.Post(
                    (o) => {
                        var s = new MeshGeometryModel3D
                        {
                            Geometry = ob.Geometry,
                            DepthBias = -100,
                            SlopeScaledDepthBias = 0,
                            CullMode = SharpDX.Direct3D11.CullMode.Back
                        };
                        if (ob.Material is HelixToolkit.UWP.Model.PhongMaterialCore p)
                        {
                            var diffuse = p.DiffuseColor;
                            diffuse.Red = (float)rnd.NextDouble();
                            diffuse.Green = (float)rnd.NextDouble();
                            diffuse.Blue = (float)rnd.NextDouble();
                            diffuse.Alpha = 0.6f;//(float)(Math.Min(0.8, Math.Max(0.2, rnd.NextDouble())));
                            p.DiffuseColor = diffuse;
                            if (p.DiffuseColor.Alpha < 0.9)
                            {
                                s.IsTransparent = true;
                            }
                            s.Material = p;
                        }               
                        s.Transform3D = Matrix.RotationY((float)(45.0 / 180.0 * Math.PI));
                        this.ModelGeometry.Add(s);
                    }, null);             
            }
        }
    }
}
