// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace FileLoadDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Media.Media3D;

    using HelixToolkit.Wpf.SharpDX;
    using Microsoft.Win32;
    using System.Windows.Input;
    using System.IO;
    using System.ComponentModel;

    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string info)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        protected bool SetValue<T>(ref T backingField, T value, string propertyName)
        {
            if (object.Equals(backingField, value))
            {
                return false;
            }

            backingField = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class MainViewModel : ObservableObject
    {
        private const string OpenFileFilter = "3D model files (*.obj;*.3ds)|*.obj;*.3ds";

        public Element3DCollection ModelGeometry { get; private set; }
        public Transform3D ModelTransform { get; private set; }
        public Viewport3DX modelView
        {
            get;
            set;
        }


        public ICommand OpenFileCommand
        {
            get; set;
        }
        public DefaultEffectsManager EffectsManager { get; private set; }

        public DefaultRenderTechniquesManager RenderTechniquesManager { get; private set; }
        public MainViewModel()
        {
            this.OpenFileCommand = new DelegateCommand(this.OpenFile);
            this.ModelTransform = new TranslateTransform3D(0, 0, 0);

            this.ModelGeometry = new Element3DCollection();
            RenderTechniquesManager = new DefaultRenderTechniquesManager();
            EffectsManager = new DefaultEffectsManager(RenderTechniquesManager);
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
            else
            {
                LoadObj(path);
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
        public void AttachModelList(List<Object3D> objs)
        {
            this.ModelTransform = new TranslateTransform3D(0, 0, 0);
            this.ModelGeometry = new Element3DCollection();
            foreach (var ob in objs)
            {
                var s = new MeshGeometryModel3D
                {
                    Geometry = ob.Geometry,
                    Material = ob.Material,
                };
                this.ModelGeometry.Add(s);
                s.Attach(modelView.RenderHost);

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