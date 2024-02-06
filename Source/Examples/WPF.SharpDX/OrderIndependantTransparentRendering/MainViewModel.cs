// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OrderIndependentTransparentRendering
{
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using HelixToolkit.Wpf.SharpDX.Model;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Media3D = System.Windows.Media.Media3D;
    public enum MaterialType
    {
        BlinnPhong, PBR, Diffuse
    };
    public class MainViewModel : BaseViewModel
    {
        private const string OpenFileFilter = "3D model files (*.obj;*.3ds;*.stl|*.obj;*.3ds;*.stl;";

        public ObservableElement3DCollection ModelGeometry { get; private set; }

        public ObservableElement3DCollection PlaneGeometry { private set; get; }

        public LineGeometry3D GridModel { private set; get; }

        public Media3D.Transform3D GridTransform { private set; get; }

        private bool showWireframe = false;

        public bool ShowWireframe
        {
            set
            {
                if (SetValue(ref showWireframe, value))
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

        public OutlineMode DrawMode
        {
            set; get;
        } = OutlineMode.Merged;

        private bool highlightSeparated = false;
        public bool HighlightSeparated
        {
            set
            {
                if (SetValue(ref highlightSeparated, value))
                {
                    DrawMode = value ? OutlineMode.Separated : OutlineMode.Merged;
                    OnPropertyChanged(nameof(DrawMode));
                }
            }
            get { return highlightSeparated; }
        }

        private bool oitWeightModeEnabled = false;
        public bool OITWeightedModeEnabled
        {
            set
            {
                SetValue(ref oitWeightModeEnabled, value);
            }
            get { return oitWeightModeEnabled; }
        }

        private bool oitDepthPeelModeEnabled = true;
        public bool OITDepthPeelModeEnabled
        {
            set
            {
                SetValue(ref oitDepthPeelModeEnabled, value);
            }
            get { return oitDepthPeelModeEnabled; }
        }

        private OITRenderType oitRenderType = OITRenderType.DepthPeeling;
        public OITRenderType OITRenderType
        {
            set
            {
                if (SetValue(ref oitRenderType, value))
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

        private MaterialType materialType = MaterialType.BlinnPhong;
        public MaterialType MaterialType
        {
            set
            {
                if (SetValue(ref materialType, value))
                {
                    UpdateMaterials();
                }
            }
            get => materialType;
        }

        public OITWeightMode[] OITWeights { get; } = new OITWeightMode[] { OITWeightMode.Linear0, OITWeightMode.Linear1, OITWeightMode.Linear2, OITWeightMode.NonLinear };

        public OITRenderType[] OITRenderTypes { get; } = new OITRenderType[] { OITRenderType.None, OITRenderType.DepthPeeling, OITRenderType.SinglePassWeighted };

        public MaterialType[] MaterialTypes { get; } = new MaterialType[] { MaterialType.BlinnPhong, MaterialType.PBR, MaterialType.Diffuse };
        private int redPlaneOpacity = 60;
        public int RedPlaneOpacity
        {
            set
            {
                if (SetValue(ref redPlaneOpacity, value))
                {
                    var m = (PlaneGeometry[0] as MeshGeometryModel3D).Material as PhongMaterial;
                    m.DiffuseColor = new Color4(1, 0, 0, value / 100f);
                }
            }
            get => redPlaneOpacity;
        }
        private int greenPlaneOpacity = 60;
        public int GreenPlaneOpacity
        {
            set
            {
                if (SetValue(ref greenPlaneOpacity, value))
                {
                    var m = (PlaneGeometry[1] as MeshGeometryModel3D).Material as PhongMaterial;
                    m.DiffuseColor = new Color4(0, 1, 0, value / 100f);
                }
            }
            get => greenPlaneOpacity;
        }
        private int bluePlaneOpacity = 60;
        public int BluePlaneOpacity
        {
            set
            {
                if (SetValue(ref bluePlaneOpacity, value))
                {
                    var m = (PlaneGeometry[2] as MeshGeometryModel3D).Material as PhongMaterial;
                    m.DiffuseColor = new Color4(0, 0, 1, value / 100f);
                }
            }
            get => bluePlaneOpacity;
        }
        public ICommand ResetCameraCommand
        {
            set; get;
        }

        private SynchronizationContext context = SynchronizationContext.Current;


        private readonly Random rnd = new Random();

        public MainViewModel()
        {
            this.ModelGeometry = new ObservableElement3DCollection();
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera()
            {
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -50, -50),
                Position = new System.Windows.Media.Media3D.Point3D(0, 50, 50),
                FarPlaneDistance = 500,
                NearPlaneDistance = 0.1,
                Width = 100
            };
            ResetCameraCommand = new RelayCommand((o) => { Camera.Reset(); });
            Task.Run(() => { Load3ds("NITRO_ENGINE.3ds"); });

            BuildGrid();
            BuildPlanes();
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
            GridTransform = new Media3D.TranslateTransform3D(new Media3D.Vector3D(0, -10, 0));
        }

        private void BuildPlanes()
        {
            PlaneGeometry = new ObservableElement3DCollection();
            var builder = new MeshBuilder(true);
            builder.AddBox(new SharpDX.Vector3(0, 0, 0), 15, 15, 0.5);
            var mesh = builder.ToMesh();

            var material = new PhongMaterial();
            material.DiffuseColor = new SharpDX.Color4(1, 0, 0, RedPlaneOpacity / 100f);

            var model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                Transform = new Media3D.TranslateTransform3D(-15, 0, 0),
                IsTransparent = true,
                CullMode = SharpDX.Direct3D11.CullMode.Back
            };
            PlaneGeometry.Add(model);

            material = new PhongMaterial();
            material.DiffuseColor = new SharpDX.Color4(0, 1, 0, GreenPlaneOpacity / 100f);

            model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                Transform = new Media3D.TranslateTransform3D(-20, 5, -10),
                IsTransparent = true,
                CullMode = SharpDX.Direct3D11.CullMode.Back
            };
            PlaneGeometry.Add(model);

            material = new PhongMaterial();
            material.DiffuseColor = new SharpDX.Color4(0, 0, 1, BluePlaneOpacity / 100f);

            model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                Transform = new Media3D.TranslateTransform3D(-25, 10, -20),
                IsTransparent = true,
                CullMode = SharpDX.Direct3D11.CullMode.Back
            };
            PlaneGeometry.Add(model);
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

            var rnd = new Random();

            foreach (var ob in objs)
            {
                ob.Geometry.UpdateOctree();
                Task.Delay(50).Wait(); //Only for async loading demo
                context.Post((o) =>
                {
                    var s = new MeshGeometryModel3D
                    {
                        Geometry = ob.Geometry,
                        IsTransparent = true,
                        DepthBias = -100
                    };
                    UpdateMaterial(s);
                    this.ModelGeometry.Add(s);
                }, null);
            }
        }

        private void UpdateMaterials()
        {
            foreach (var geo in ModelGeometry)
            {
                if (geo is MeshGeometryModel3D mesh)
                {
                    UpdateMaterial(mesh);
                }
            }
        }

        private void UpdateMaterial(MeshGeometryModel3D mesh)
        {
            var diffuse = new Color4();
            diffuse.Red = (float)rnd.NextDouble();
            diffuse.Green = (float)rnd.NextDouble();
            diffuse.Blue = (float)rnd.NextDouble();
            diffuse.Alpha = 1;
            diffuse.Alpha = 0.6f;
            Material material = null;
            switch (materialType)
            {
                case MaterialType.BlinnPhong:
                    material = new PhongMaterial()
                    {
                        DiffuseColor = diffuse
                    };
                    break;
                case MaterialType.PBR:
                    material = new PBRMaterial()
                    {
                        AlbedoColor = diffuse,
                        MetallicFactor = 0.7f,
                        RoughnessFactor = 0.6f,
                        ReflectanceFactor = 0.2,
                    };
                    break;
                case MaterialType.Diffuse:
                    material = new DiffuseMaterial()
                    {
                        DiffuseColor = diffuse
                    };
                    break;
            }
            mesh.Material = material;
        }
    }
}