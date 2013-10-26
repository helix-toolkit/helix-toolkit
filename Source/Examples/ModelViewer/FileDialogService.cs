namespace ModelViewer
{
    using Microsoft.Win32;

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
            {
                return null;
            }

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
            {
                return null;
            }

            return d.FileName;
        }
    }
}