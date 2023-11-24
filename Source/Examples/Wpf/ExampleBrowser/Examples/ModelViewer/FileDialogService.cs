using Microsoft.Win32;

namespace ModelViewer;

public sealed class FileDialogService : IFileDialogService
{
    public string? OpenFileDialog(string? initialDirectory, string? defaultPath, string filter, string defaultExtension)
    {
        var d = new OpenFileDialog
        {
            InitialDirectory = initialDirectory,
            FileName = defaultPath,
            Filter = filter,
            DefaultExt = defaultExtension
        };

        if (d.ShowDialog() != true)
        {
            return null;
        }

        return d.FileName;
    }

    public string? SaveFileDialog(string? initialDirectory, string? defaultPath, string filter, string defaultExtension)
    {
        var d = new SaveFileDialog
        {
            InitialDirectory = initialDirectory,
            FileName = defaultPath,
            Filter = filter,
            DefaultExt = defaultExtension
        };

        if (d.ShowDialog() != true)
        {
            return null;
        }

        return d.FileName;
    }
}
