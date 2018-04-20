// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OrderIndependentTransparentRendering
{
    using DemoCore;
    using HelixToolkit.Wpf.SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using Media3D = System.Windows.Media.Media3D;

    public class MainViewModel : BaseViewModel
    {
        private const string OpenFileFilter = "3D model files (*.obj;*.3ds;*.stl|*.obj;*.3ds;*.stl;";

        public ObservableElement3DCollection ModelGeometry { get; private set; }

        public LineGeometry3D GridModel { private set; get; }

        public Media3D.Transform3D GridTransform { private set; get; }

        private bool showWireframe = false;
        public bool ShowWireframe
        {
            set
            {
                if(SetValue(ref showWireframe, value))
                {
                    foreach(var model in ModelGeometry)
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

        public ICommand ResetCameraCommand
        {
            set;get;
        }


        public MainViewModel()
        {
            this.ModelGeometry = new ObservableElement3DCollection();
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera() {
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -50, -50),
                Position = new System.Windows.Media.Media3D.Point3D(0, 50, 50),
                FarPlaneDistance = 1000, NearPlaneDistance = 0.1, Width = 100 };
            ResetCameraCommand = new RelayCommand((o) => { Camera.Reset(); });
            Load3ds("NITRO_ENGINE.3ds");
            BuildGrid();
        }

        private void BuildGrid()
        {
            var builder = new LineBuilder();
            int zOff = -45;
            for(int i =0; i< 10; ++i)
            {
                for(int j =0; j < 10; ++j)
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
            this.ModelGeometry = new ObservableElement3DCollection();
            Random rnd = new Random();
            
            foreach (var ob in objs)
            {
                var s = new MeshGeometryModel3D
                {
                    Geometry = ob.Geometry,
                    Material = ob.Material,
                    DepthBias = -100, SlopeScaledDepthBias = 0,
                    CullMode = SharpDX.Direct3D11.CullMode.Back
                };
                if(ob.Material is PhongMaterial p)
                {
                    var diffuse = p.DiffuseColor;
                    diffuse.Red = (float)rnd.NextDouble();
                    diffuse.Green = (float)rnd.NextDouble();
                    diffuse.Blue = (float)rnd.NextDouble();
                    diffuse.Alpha = (float)(Math.Max(0.5, rnd.NextDouble()));
                    p.DiffuseColor = diffuse;
                    if (p.DiffuseColor.Alpha < 0.99)
                    {
                        s.IsTransparent = true;
                    }
                }
                if (ob.Transform != null && ob.Transform.Count > 0)
                {
                    s.Instances = ob.Transform;
                }
                this.ModelGeometry.Add(s);

            }
            this.OnPropertyChanged("ModelGeometry");
        }
    }

}