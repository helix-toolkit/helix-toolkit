// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Microsoft.Win32;

namespace StudioDemo
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Media;

    public class VisualElement
    {
        public IEnumerable<VisualElement> Children
        {
            get
            {
                var mv = element as ModelVisual3D;
                if (mv != null)
                {
                    if (mv.Content!=null)
                        yield return new VisualElement(mv.Content);
                    foreach (var mc in mv.Children)
                        yield return new VisualElement(mc);
                }

                var mg = element as Model3DGroup;
                if (mg != null)
                    foreach (var mc in mg.Children) yield return new VisualElement(mc);

                var gm = element as GeometryModel3D;
                if (gm != null)
                {
                    yield return new VisualElement(gm.Geometry);
                }

                //int n = VisualTreeHelper.GetChildrenCount(element);
                //for (int i = 0; i < n; i++)
                //    yield return new VisualElement(VisualTreeHelper.GetChild(element, i));
                foreach (DependencyObject c in LogicalTreeHelper.GetChildren(element))
                    yield return new VisualElement(c);
            }
        }

        public string TypeName
        {
            get
            {
                return element.GetType().Name;
            }
        }
        public Brush Brush
        {
            get
            {
                if (element.GetType() == typeof(ModelVisual3D))
                    return Brushes.Orange;
                if (element.GetType() == typeof(GeometryModel3D))
                    return Brushes.Green;
                if (element.GetType() == typeof(Model3DGroup))
                    return Brushes.Blue;
                if (element.GetType() == typeof(Visual3D))
                    return Brushes.Gray;
                if (element.GetType() == typeof(Model3D))
                    return Brushes.Black;
                return null;
            }
        }
        public override string ToString()
        {
            return element.GetType().ToString();
        }

        private DependencyObject element;

        public VisualElement(DependencyObject e)
        {
            element = e;
        }

    }

    public class MainViewModel : Observable
    {
        public ICommand FileOpenCommand { get; set; }
        public ICommand FileExportCommand { get; set; }
        public ICommand FileExitCommand { get; set; }
        public ICommand HelpAboutCommand { get; set; }
        public ICommand ViewZoomExtentsCommand { get; set; }
        public ICommand EditCopyXamlCommand { get; set; }

        private const string OpenFileFilter = "3D model files (*.3ds;*.obj;*.lwo;*.stl)|*.3ds;*.obj;*.objz;*.lwo;*.stl";
        private const string TitleFormatString = "3D model viewer - {0}";

        private string _currentModelPath;
        public string CurrentModelPath
        {
            get { return _currentModelPath; }
            set { _currentModelPath = value; RaisePropertyChanged("CurrentModelPath"); }
        }

        private string _applicationTitle;
        public string ApplicationTitle
        {
            get { return _applicationTitle; }
            set { _applicationTitle = value; RaisePropertyChanged("ApplicationTitle"); }
        }

        private double expansion;

        public double Expansion
        {
            get { return expansion; }
            set
            {
                if (expansion != value)
                {
                    expansion = value;
                    RaisePropertyChanged("Expansion");
                }
            }
        }

        private IFileDialogService FileDialogService;
        public IHelixViewport3D HelixView { get; set; }

        private Model3D _currentModel;

        public Model3D CurrentModel
        {
            get { return _currentModel; }
            set { _currentModel = value; RaisePropertyChanged("CurrentModel"); }
        }

        public List<VisualElement> Elements { get; set; }

        public MainViewModel(IFileDialogService fds, HelixViewport3D hv)
        {
            Expansion = 1;
            FileDialogService = fds;
            HelixView = hv;
            FileOpenCommand = new DelegateCommand(FileOpen);
            FileExportCommand = new DelegateCommand(FileExport);
            FileExitCommand = new DelegateCommand(FileExit);
            ViewZoomExtentsCommand = new DelegateCommand(ViewZoomExtents);
            EditCopyXamlCommand = new DelegateCommand(CopyXaml);
            ApplicationTitle = "3D Model viewer";
            Elements = new List<VisualElement>();
            foreach (var c in hv.Children) Elements.Add(new VisualElement(c));

        }

        private void FileExit()
        {
            App.Current.Shutdown();
        }

        private void FileExport()
        {
            var path = FileDialogService.SaveFileDialog(null, null, Exporters.Filter, ".png");
            if (path == null)
                return;
            HelixView.Export(path);
            /*
                        var ext = Path.GetExtension(path).ToLowerInvariant();
                        switch (ext)
                        {
                            case ".png":
                            case ".jpg":
                                HelixView.Export(path);
                                break;
                            case ".xaml":
                                {
                                    var e = new XamlExporter(path);
                                    e.Export(CurrentModel);
                                    e.Close();
                                    break;
                                }

                            case ".xml":
                                {
                                    var e = new KerkytheaExporter(path);
                                    e.Export(HelixView.Viewport);
                                    e.Close();
                                    break;
                                }
                            case ".obj":
                                {
                                    var e = new ObjExporter(path);
                                    e.Export(CurrentModel);
                                    e.Close();
                                    break;
                                }
                            case ".objz":
                                {
                                    var tmpPath = Path.ChangeExtension(path, ".obj");
                                     var e = new ObjExporter(tmpPath);
                                     e.Export(CurrentModel);
                                     e.Close();
                                    GZipHelper.Compress(tmpPath);
                                    break;
                                }
                            case ".x3d":
                                {
                                    var e = new X3DExporter(path);
                                    e.Export(CurrentModel);
                                    e.Close();
                                    break;
                                }
                        }*/
        }

        private void CopyXaml()
        {
            var rd = XamlExporter.WrapInResourceDictionary(CurrentModel);
            Clipboard.SetText(XamlHelper.GetXaml(rd));
        }

        private void ViewZoomExtents()
        {
            HelixView.ZoomExtents(500);
        }

        private void FileOpen()
        {
            CurrentModelPath = FileDialogService.OpenFileDialog("models", null, OpenFileFilter, ".3ds");
#if !DEBUG
            try
            {
#endif
            CurrentModel = ModelImporter.Load(CurrentModelPath);
            ApplicationTitle = String.Format(TitleFormatString, CurrentModelPath);
            HelixView.ZoomExtents(0);
#if !DEBUG
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
#endif
        }
    }

    public interface IFileDialogService
    {
        string OpenFileDialog(string initialDirectory, string defaultPath, string filter, string defaultExtension);
        string SaveFileDialog(string initialDirectory, string defaultPath, string filter, string defaultExtension);
    }

    public class FileDialogService : IFileDialogService
    {
        public string OpenFileDialog(string initialDirectory, string defaultPath, string filter, string defaultExtension)
        {
            var d = new OpenFileDialog();
            d.InitialDirectory = initialDirectory;
            d.FileName = defaultPath;
            d.Filter = filter;
            d.DefaultExt = defaultExtension;
            if (!d.ShowDialog().Value)
                return null;
            return d.FileName;
        }

        public string SaveFileDialog(string initialDirectory, string defaultPath, string filter, string defaultExtension)
        {
            var d = new SaveFileDialog();
            d.InitialDirectory = initialDirectory;
            d.FileName = defaultPath;
            d.Filter = filter;
            d.DefaultExt = defaultExtension;
            if (!d.ShowDialog().Value)
                return null;
            return d.FileName;
        }
    }
}