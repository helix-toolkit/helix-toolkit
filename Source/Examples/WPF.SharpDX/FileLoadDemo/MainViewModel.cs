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
    using HelixToolkit.Wpf.SharpDX.Model;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    public class MainViewModel : BaseViewModel
    {
        private const string OpenFileFilter = "3D model files (*.obj;*.3ds;*.stl|*.obj;*.3ds;*.stl;*.ply;";

        public ObservableElement3DCollection ModelGeometry { get; private set; }

        public Viewport3DX modelView
        {
            get;
            set;
        }

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

        public ICommand OpenFileCommand
        {
            get; set;
        }

        public ICommand ResetCameraCommand
        {
            set;get;
        }

        public ICommand ExportCommand { private set; get; }

        private SynchronizationContext context = SynchronizationContext.Current;
        public MainViewModel()
        {
            this.OpenFileCommand = new DelegateCommand(this.OpenFile);
            this.ModelGeometry = new ObservableElement3DCollection();
            EffectsManager = new DefaultEffectsManager();
            Camera = new OrthographicCamera() {
                LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -10, -10),
                Position = new System.Windows.Media.Media3D.Point3D(0, 10, 10),
                FarPlaneDistance = 10000, NearPlaneDistance = 0.1 };
            ResetCameraCommand = new DelegateCommand(() =>
            {
                Camera.LookDirection = new System.Windows.Media.Media3D.Vector3D(0, -10, -10);
                Camera.Position = new System.Windows.Media.Media3D.Point3D(0, 10, 10);
            });
            ExportCommand = new DelegateCommand(() => { ExportFile(); });
        }

        private void OpenFile()
        {
            string path = OpenFileDialog(OpenFileFilter);
            if (path == null)
            {
                return;
            }
            ModelGeometry.Clear();
            Task.Run(() => {
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
                else if(Path.GetExtension(path).ToLower() == ".ply")
                {
                    LoadPly(path);
                }
            });

        }

        private void ExportFile()
        {
            string path = SaveFileDialog("3D model files (*.obj;|*.obj;");
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            var exporter = new ObjExporter(path);
            exporter.Export(ModelGeometry[0]);
            exporter.Close();
            MessageBox.Show($"Export Finished. {path}");
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

        public void LoadPly(string path)
        {
            var reader = new PlyReader();
            var objCol = reader.Read(path);
            AttachModelList(objCol);
        }
        public void AttachModelList(List<Object3D> objs)
        {            
            foreach (var ob in objs)
            {
                ob.Geometry.UpdateOctree();
                context.Post((o) => {
                    var s = new MeshGeometryModel3D
                    {
                        Geometry = ob.Geometry,
                    };
                    if(ob.Material is PhongMaterialCore p)
                    {
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

        private string SaveFileDialog(string filter)
        {
            var d = new SaveFileDialog();
            d.Filter = filter;
            if (d.ShowDialog() == true)
            {
                return d.FileName;
            }
            else { return ""; }
        }
    }

}