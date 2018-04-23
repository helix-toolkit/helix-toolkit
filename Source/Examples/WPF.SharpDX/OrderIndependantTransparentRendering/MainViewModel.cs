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

        public ICommand ResetCameraCommand
        {
            set; get;
        }

        public MainViewModel()
        {
            this.ModelGeometry = new ObservableElement3DCollection();
            EffectsManager = new DefaultEffectsManager();
            Camera = new PerspectiveCamera()
            {
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -50, -50),
                Position = new System.Windows.Media.Media3D.Point3D(0, 50, 50),
                FarPlaneDistance = 1000,
                NearPlaneDistance = 0.1,
            };
            ResetCameraCommand = new RelayCommand((o) =>
            {
                // Camera.Reset();
                GroupElement3D pFirst = PlaneGeometry[0] as GroupElement3D;
                if (pFirst != null)
                {
                    PlaneGeometry.Remove(pFirst);
                    PlaneGeometry.Add(pFirst);
                }
                pFirst = PlaneGeometry[0] as GroupElement3D;
                if (pFirst != null)
                {
                    PlaneGeometry.Remove(pFirst);
                    PlaneGeometry.Add(pFirst);
                }
                pFirst = PlaneGeometry[0] as GroupElement3D;
                if (pFirst != null)
                {
                    PlaneGeometry.Remove(pFirst);
                    PlaneGeometry.Add(pFirst);
                }
                pFirst = PlaneGeometry[0] as GroupElement3D;
                if (pFirst != null)
                {
                    PlaneGeometry.Remove(pFirst);
                    PlaneGeometry.Add(pFirst);
                }
            });
            Load3ds("NITRO_ENGINE.3ds");
            PlaneGeometry = new ObservableElement3DCollection();
            BuildGrid();
            PlaneGeometry.Add(AddPlaneGroup(0));
            PlaneGeometry.Add(AddPlaneGroup(10));
            PlaneGeometry.Add(AddPlaneGroup(20));
            PlaneGeometry.Add(AddPlaneGroup(30));
            PlaneGeometry.Add(AddPlaneGroup(40));

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

        private GroupElement3D AddPlaneGroup(double zOffset)
        {
            GroupElement3D group = new GroupModel3D();
            var builder = new MeshBuilder(true);
            builder.AddBox(new SharpDX.Vector3(0, 0, 0), 5, 5, 0.5);
            var mesh = builder.ToMesh();

            var material = new PhongMaterial();
            material.DiffuseColor = new SharpDX.Color4(1, 0, 0, 0.5f);

            var model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                IsTransparent = true,
                Transform = new Media3D.TranslateTransform3D(0, 0, 0 - zOffset),
                CullMode = SharpDX.Direct3D11.CullMode.Back
            };
            group.Children.Add(model);

            material = new PhongMaterial();
            material.DiffuseColor = new SharpDX.Color4(0, 1, 0, 0.5f);

            model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                IsTransparent = true,
                Transform = new Media3D.TranslateTransform3D(-5, -0, -zOffset),
                CullMode = SharpDX.Direct3D11.CullMode.Back
            };
            group.Children.Add(model);

            material = new PhongMaterial();
            material.DiffuseColor = new SharpDX.Color4(0, 0, 1, 0.5f);

            model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                Transform = new Media3D.TranslateTransform3D(-0, -5, -zOffset),
                IsTransparent = true,
                CullMode = SharpDX.Direct3D11.CullMode.Back
            };
            group.Children.Add(model);

            group.Transform = new Media3D.ScaleTransform3D(2, 2, 2);

            return group;
        }

        private void BuildPlanes2()
        {

            var builder = new MeshBuilder(true);
            builder.AddBox(new SharpDX.Vector3(0, 0, 0), 100, 100, 0.5);
            var mesh = builder.ToMesh();

            var material = new PhongMaterial();
            material.DiffuseColor = new SharpDX.Color4(1, 0, 0, 0.5f);

            var model = new MeshGeometryModel3D()
            {
                Geometry = mesh,
                Material = material,
                Transform = new Media3D.TranslateTransform3D(-15, 0, 0),
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
            this.ModelGeometry = new ObservableElement3DCollection();
            Random rnd = new Random();

            foreach (var ob in objs)
            {
                var s = new MeshGeometryModel3D
                {
                    Geometry = ob.Geometry,
                    Material = ob.Material,
                    DepthBias = -100,
                    SlopeScaledDepthBias = 0,
                    CullMode = SharpDX.Direct3D11.CullMode.Back
                };
                if (ob.Material is PhongMaterial p)
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