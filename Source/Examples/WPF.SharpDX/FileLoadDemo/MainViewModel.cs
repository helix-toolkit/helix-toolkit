// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FileLoadDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using HelixToolkit.Wpf.SharpDX;
    using Microsoft.Win32;
    using System.Windows.Input;
    using System.IO;
    using System.ComponentModel;
    using DemoCore;

    public class MainViewModel : BaseViewModel
    {
        private const string OpenFileFilter = "3D model files (*.obj;*.3ds;*.stl|*.obj;*.3ds;*.stl;";

        public Element3DCollection ModelGeometry { get; private set; }

        public Viewport3DX modelView
        {
            get;
            set;
        }


        public ICommand OpenFileCommand
        {
            get; set;
        }

        public ICommand ResetCameraCommand
        {
            set;get;
        }


        public MainViewModel()
        {
            this.OpenFileCommand = new DelegateCommand(this.OpenFile);
            this.ModelGeometry = new Element3DCollection();
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera() {
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -10, -10),
                Position = new System.Windows.Media.Media3D.Point3D(0, 10, 10),
                FarPlaneDistance = 10000, NearPlaneDistance = 0.1 };
            ResetCameraCommand = new DelegateCommand(() => { Camera.Reset(); });
        }

        private void OpenFile()
        {
            string path = OpenFileDialog(OpenFileFilter);
            if (path == null)
            {
                return;
            }
            if (Path.GetExtension(path).ToLower() == ".3ds")
            {
                Load3ds(path);
            }
            else if(Path.GetExtension(path).ToLower() == ".obj")
            {
                LoadObj(path);
            }
            else if(Path.GetExtension(path).ToLower() == ".stl")
            {
                LoadStl(path);
            }
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
            this.ModelGeometry = new Element3DCollection();
            foreach (var ob in objs)
            {
                var s = new MeshGeometryModel3D
                {
                    Geometry = ob.Geometry,
                    Material = ob.Material,
                };
                this.ModelGeometry.Add(s);

            }
            this.OnPropertyChanged("ModelGeometry");
        }
        private string OpenFileDialog(string filter)
        {
            var d = new OpenFileDialog();
            d.CustomPlaces.Clear();


            d.Filter = filter;

            if (!d.ShowDialog().Value)
            {
                return null;
            }

            return d.FileName;
        }
    }

}