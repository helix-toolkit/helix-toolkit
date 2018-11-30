using HelixToolkit.Wpf.SharpDX;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Vector3 = SharpDX.Vector3;

namespace RenderTechniqueImportExport
{
    public class MainViewModel : DemoCore.BaseViewModel
    {
        private const string OpenFileFilter = "Techniques file (*.techniques;|*.techniques";
        public ICommand ExportCommand { private set; get; }
        public ICommand ImportCommand { private set; get; }
        public ICommand ExportSingleTechnique { private set; get; }

        public MeshGeometry3D MeshModel { private set; get; }
        public Material MeshMaterial { get; } = PhongMaterials.Jade;
        public LineGeometry3D LineModel { private set; get; }
        public PointGeometry3D PointModel { private set; get; }
        public ObservableCollection<string> TechniqueList { get; } = new ObservableCollection<string>();
        public string SelectedTechnique { set; get; }

        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();

            var builder = new MeshBuilder();
            builder.AddSphere(new Vector3(), 2);
            builder.AddTorus(5, 1);
            MeshModel = builder.ToMesh();

            var lineBuilder = new LineBuilder();
            lineBuilder.AddGrid(BoxFaces.All, 10, 10, 10, 10);
            LineModel = lineBuilder.ToLineGeometry3D();

            var offset = new Vector3(-4, 0, 0);
            PointModel = new PointGeometry3D() { Positions = new Vector3Collection(MeshModel.Positions.Select(x => x + offset)) };

            ExportCommand = new RelayCommand((o) => { Export(); });
            ImportCommand = new RelayCommand((o) => { Import(); });
            ExportSingleTechnique = new RelayCommand((o) => { Export(SelectedTechnique); });
        }

        private void Export(string technique="")
        {
            var path = CreateFileDialog(OpenFileFilter, technique);
            if(string.IsNullOrEmpty(path))
            {
                return;
            }
            if (string.IsNullOrEmpty(technique))
            {
                EffectsManager.ExportTechniquesAsBinary(path);
            }
            else
            {
                EffectsManager.ExportTechniqueAsBinary(technique, path);
            }
        }

        private void Import()
        {
            var path = OpenFileDialog(OpenFileFilter);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            var manager = EffectsManager;
            EffectsManager = null;
            manager.ImportTechniques(path, true);
            EffectsManager = manager;
            TechniqueList.Clear();
            foreach(var tech in EffectsManager.RenderTechniques)
            {
                TechniqueList.Add(tech);
            }
        }

        private string OpenFileDialog(string filter)
        {
            var d = new OpenFileDialog();
            d.CustomPlaces.Clear();


            d.Filter = filter;
            d.InitialDirectory = Environment.CurrentDirectory;
            if (!d.ShowDialog().Value)
            {
                return null;
            }

            return d.FileName;
        }

        private string CreateFileDialog(string filter, string fileName)
        {
            var d = new SaveFileDialog();
            d.CustomPlaces.Clear();
            

            d.Filter = filter;
            d.FileName = fileName;
            d.DefaultExt = "techniques";
            if (!d.ShowDialog().Value)
            {
                return null;
            }

            return d.FileName;
        }
    }
}
