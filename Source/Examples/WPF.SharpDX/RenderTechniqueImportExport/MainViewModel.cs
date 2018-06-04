using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using HelixToolkit.Wpf.SharpDX;
using Microsoft.Win32;

namespace RenderTechniqueImportExport
{
    public class MainViewModel : DemoCore.BaseViewModel
    {
        private const string OpenFileFilter = "Techniques file (*.techniques;|*.techniques";
        public ICommand ExportCommand { private set; get; }
        public ICommand ImportCommand { private set; get; }
        public MainViewModel()
        {
            EffectsManager = new DefaultEffectsManager();

            ExportCommand = new RelayCommand((o) => { Export(); });
            ImportCommand = new RelayCommand((o) => { Import(); });
        }

        private void Export()
        {
            var path = CreateFileDialog(OpenFileFilter);
            if(string.IsNullOrEmpty(path))
            {
                return;
            }
            EffectsManager.ExportTechniquesAsBinary(path);
        }

        private void Import()
        {
            var path = OpenFileDialog(OpenFileFilter);
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            EffectsManager.ImportTechniques(path);
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

        private string CreateFileDialog(string filter)
        {
            var d = new SaveFileDialog();
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
