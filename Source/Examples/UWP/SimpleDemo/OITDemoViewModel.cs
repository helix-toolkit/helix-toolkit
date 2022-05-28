using GalaSoft.MvvmLight;
using HelixToolkit.UWP;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleDemoW10
{
    public class OITModel : ObservableObject
    {
        public Geometry3D Model { private set; get; }

        public Material Material { private set; get; }

        public bool IsTransparent { private set; get; }

        private bool showWireframe = false;

        public bool ShowWireframe
        {
            set
            {
                Set(ref showWireframe, value);
            }
            get
            {
                return showWireframe;
            }
        }

        public OITModel(Geometry3D model, Material material, bool isTransparent)
        {
            Model = model;
            Material = material;
            IsTransparent = isTransparent;
        }
    }

    public class OITDemoViewModel : ObservableObject
    {
        public ObservableCollection<OITModel> ModelGeometry { get; private set; } = new ObservableCollection<OITModel>();

        public Matrix Transform { private set; get; } = Matrix.Translation(60, -10, 0);
        public LineGeometry3D GridModel { private set; get; }
        public Matrix GridTransform { private set; get; } = Matrix.Translation(60, -10, 0);
        public OITWeightMode[] OITWeights { get; } = new OITWeightMode[] { OITWeightMode.Linear0, OITWeightMode.Linear1, OITWeightMode.Linear2, OITWeightMode.NonLinear };
        public OITRenderType[] OITRenderTypes { get; } = new OITRenderType[] { OITRenderType.None, OITRenderType.DepthPeeling, OITRenderType.SinglePassWeighted };
        private bool showWireframe = false;

        public bool ShowWireframe
        {
            set
            {
                if(Set(ref showWireframe, value))
                {
                    foreach(var item in ModelGeometry)
                    {
                        item.ShowWireframe = value;
                    }
                }
            }
            get
            {
                return showWireframe;
            }
        }
        private bool oitWeightModeEnabled = false;
        public bool OITWeightedModeEnabled
        {
            set
            {
                Set(ref oitWeightModeEnabled, value);
            }
            get { return oitWeightModeEnabled; }
        }

        private bool oitDepthPeelModeEnabled = true;
        public bool OITDepthPeelModeEnabled
        {
            set
            {
                Set(ref oitDepthPeelModeEnabled, value);
            }
            get { return oitDepthPeelModeEnabled; }
        }

        private OITRenderType oitRenderType = OITRenderType.DepthPeeling;
        public OITRenderType OITRenderType
        {
            set
            {
                if (Set(ref oitRenderType, value))
                {
                    switch (value)
                    {
                        case OITRenderType.None:
                            OITDepthPeelModeEnabled = OITWeightedModeEnabled = false;
                            break;
                        case OITRenderType.DepthPeeling:
                            OITDepthPeelModeEnabled = true;
                            OITWeightedModeEnabled = false;
                            break;
                        case OITRenderType.SinglePassWeighted:
                            oitDepthPeelModeEnabled = false;
                            OITWeightedModeEnabled = true;
                            break;
                    }
                }
            }
            get => oitRenderType;
        }
        private SynchronizationContext syncContext = SynchronizationContext.Current;

        public OITDemoViewModel()
        {
            var packageFolder = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, "NITRO_ENGINE.3ds");
            BuildGrid();
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
            
            Random rnd = new Random(DateTime.Now.Millisecond);

            foreach (var ob in objs)
            {
                ob.Geometry.UpdateOctree();
                Task.Delay(100).Wait();
                syncContext.Post(
                    (o) => {
                        if (ob.Material is HelixToolkit.UWP.Model.PhongMaterialCore p)
                        {
                            var diffuse = p.DiffuseColor;
                            diffuse.Red = (float)rnd.NextDouble();
                            diffuse.Green = (float)rnd.NextDouble();
                            diffuse.Blue = (float)rnd.NextDouble();
                            diffuse.Alpha = 0.6f;//(float)(Math.Min(0.8, Math.Max(0.2, rnd.NextDouble())));
                            p.DiffuseColor = diffuse;
                            this.ModelGeometry.Add(new OITModel(ob.Geometry, p, p.DiffuseColor.Alpha < 0.9));
                        }               
                    }, null);             
            }
        }
    }
}
