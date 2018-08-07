// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OrderIndependentTransparentRendering
{
    using System.Numerics;
    using HelixToolkit.Mathematics;
    using Matrix = System.Numerics.Matrix4x4;
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Media3D = System.Windows.Media.Media3D;
    using SharpDX.Direct3D11;
    using HelixToolkit.Wpf.SharpDX.Model;

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

        public OITWeightMode[] OITWeights { get; } = new OITWeightMode[] { OITWeightMode.Linear0, OITWeightMode.Linear1, OITWeightMode.Linear2, OITWeightMode.NonLinear };

        public ICommand ResetCameraCommand
        {
            set; get;
        }

        private SynchronizationContext context = SynchronizationContext.Current;

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
                    builder.AddLine(new Vector3(-i * 5, 0, j * 5), new Vector3(i * 5, 0, j * 5));
                    builder.AddLine(new Vector3(-i * 5, 0, -j * 5), new Vector3(i * 5, 0, -j * 5));
                    builder.AddLine(new Vector3(i * 5, 0, -j * 5), new Vector3(i * 5, 0, j * 5));
                    builder.AddLine(new Vector3(-i * 5, 0, -j * 5), new Vector3(-i * 5, 0, j * 5));
                    builder.AddLine(new Vector3(-i * 5, j * 5, zOff), new Vector3(i * 5, j * 5, zOff));
                    builder.AddLine(new Vector3(i * 5, 0, zOff), new Vector3(i * 5, j * 5, zOff));
                    builder.AddLine(new Vector3(-i * 5, 0, zOff), new Vector3(-i * 5, j * 5, zOff));
                }
            }
            GridModel = builder.ToLineGeometry3D();
            GridTransform = new Media3D.TranslateTransform3D(new Media3D.Vector3D(0, -10, 0));
        }

        private void BuildPlanes()
        {
            PlaneGeometry = new ObservableElement3DCollection();
            var builder = new MeshBuilder(true);
            builder.AddBox(new Vector3(0, 0, 0), 15, 15, 0.5);
            var mesh = builder.ToMesh();

            var material = new PhongMaterial();
            material.DiffuseColor = new Color4(1, 0, 0, 0.5f);

            var model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                Transform = new Media3D.TranslateTransform3D(-15, 0, 0),
                IsTransparent = true,
                CullMode = CullMode.Back
            };
            PlaneGeometry.Add(model);

            material = new PhongMaterial();
            material.DiffuseColor = new Color4(0, 1, 0, 0.5f);

            model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                Transform = new Media3D.TranslateTransform3D(-20, 5, -10),
                IsTransparent = true,
                CullMode = CullMode.Back
            };
            PlaneGeometry.Add(model);

            material = new PhongMaterial();
            material.DiffuseColor = new Color4(0, 0, 1, 0.5f);

            model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                Transform = new Media3D.TranslateTransform3D(-25, 10, -20),
                IsTransparent = true,
                CullMode = CullMode.Back
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
            
            Random rnd = new Random();

            foreach (var ob in objs)
            {
                ob.Geometry.UpdateOctree();
                Task.Delay(50).Wait(); //Only for async loading demo
                context.Post((o) => {
                    var s = new MeshGeometryModel3D
                    {
                        Geometry = ob.Geometry,
                        DepthBias = -100,
                        SlopeScaledDepthBias = 0,
                        CullMode = CullMode.Back
                    };
                    if (ob.Material is PhongMaterialCore p)
                    {
                        var diffuse = p.DiffuseColor;
                        diffuse.Red = (float)rnd.NextDouble();
                        diffuse.Green = (float)rnd.NextDouble();
                        diffuse.Blue = (float)rnd.NextDouble();
                        diffuse.Alpha = 0.8f;//(float)(Math.Min(0.8, Math.Max(0.2, rnd.NextDouble())));
                        p.DiffuseColor = diffuse;
                        if (p.DiffuseColor.Alpha < 0.9)
                        {
                            s.IsTransparent = true;
                        }
                        s.Material = p;
                    }

                    if (ob.Transform != null && ob.Transform.Count > 0)
                    {
                        s.Instances = ob.Transform;
                    }
                    this.ModelGeometry.Add(s);
                }, null);
            }
        }
    }
}