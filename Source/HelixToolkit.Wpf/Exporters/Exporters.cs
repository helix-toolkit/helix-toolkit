using CommunityToolkit.Diagnostics;
using System.IO;

namespace HelixToolkit.Wpf;

/// <summary>
/// Contains a list of all supported exporters.
/// </summary>
public static class Exporters
{
    /// <summary>
    /// Default file export extension.
    /// </summary>
    public static readonly string DefaultExtension = ".png";

    /// <summary>
    /// File filter for all the supported exporters.
    /// </summary>
    public static readonly string Filter =
        "Bitmap Files (*.png;*.jpg)|*.png;*.jpg|XAML Files (*.xaml)|*.xaml|Kerkythea Files (*.xml)|*.xml|Wavefront Files (*.obj)|*.obj|Wavefront Files zipped (*.objz)|*.objz|Extensible 3D Graphics Files (*.x3d)|*.x3d|Collada Files (*.dae)|*.dae|STereoLithography (*.stl)|*.stl";

    /// <summary>
    /// Creates an exporter based on the extension of the specified path.
    /// </summary>
    /// <param name="path">
    /// The output path.
    /// </param>
    /// <returns>
    /// An exporter.
    /// </returns>
    public static IExporter? Create(string path)
    {
        if (path == null)
        {
            return null;
        }

        string ext = Path.GetExtension(path);
        return ext.ToLower() switch
        {
            ".png" or ".jpg" => new BitmapExporter(),
            ".obj" or ".objz" => new ObjExporter(),
            ".xaml" => new XamlExporter(),
            ".xml" => new KerkytheaExporter(),
            ".x3d" => new X3DExporter(),
            ".dae" => new ColladaExporter(),
            ".stl" => new StlExporter(),
            _ => ThrowHelper.ThrowInvalidOperationException<IExporter>("File format not supported."),
        };
    }
}
