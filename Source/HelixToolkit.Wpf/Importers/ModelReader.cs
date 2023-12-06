using System.IO;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace HelixToolkit.Wpf;

/// <summary>
/// Class ModelReader.
/// </summary>
public abstract class ModelReader : IModelReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModelReader"/> class.
    /// </summary>
    /// <param name="dispatcher">The dispatcher.</param>
    protected ModelReader(Dispatcher? dispatcher = null)
    {
        this.DefaultMaterial = Materials.Gold;
        this.Dispatcher = dispatcher;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the model should be frozen.
    /// </summary>
    /// <value><c>true</c> if model should be frozen; otherwise, <c>false</c>.</value>
    public bool Freeze { get; set; }

    /// <summary>
    /// Gets or sets the default material.
    /// </summary>
    /// <value>
    /// The default material.
    /// </value>
    public Material DefaultMaterial { get; set; }

    /// <summary>
    /// Gets the dispatcher.
    /// </summary>
    /// <value>The dispatcher.</value>
    public Dispatcher? Dispatcher { get; private set; }

    /// <summary>
    /// Gets or sets the directory.
    /// </summary>
    /// <value>The directory.</value>
    public string Directory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the texture path.
    /// </summary>
    /// <value>The texture path.</value>
    public string TexturePath
    {
        get
        {
            return this.Directory;
        }

        set
        {
            this.Directory = value;
        }
    }

    /// <summary>
    /// Reads the model from the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The model.</returns>
    public virtual Model3DGroup? Read(string path)
    {
        this.Directory = IsResourcePath(path) ? "" : (Path.GetDirectoryName(path) ?? "");
        using var stream = GetResourceStream(path);

        if (stream is null)
        {
            return null;
        }

        return this.Read(stream);
    }

    protected bool IsResourcePath(string path)
    {
        return path.StartsWith("pack://application:");
    }

    /// <summary>
    /// Gets the resource stream by the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The stream.</returns>
    protected Stream? GetResourceStream(string path)
    {
        if (IsResourcePath(path))
        {
            var streamInfo = Application.GetResourceStream(new Uri(path, UriKind.Absolute));
            return streamInfo?.Stream;
        }

        return new FileStream(path, FileMode.Open, FileAccess.Read);
    }

    /// <summary>
    /// Reads the model from the specified stream.
    /// </summary>
    /// <param name="s">The stream.</param>
    /// <returns>The model.</returns>
    public abstract Model3DGroup? Read(Stream s);

    /// <summary>
    /// Invokes the specified action on the dispatcher.
    /// </summary>
    /// <param name="action">The action.</param>
    protected void Dispatch(Action action)
    {
        if (this.Dispatcher == null)
        {
            action();
            return;
        }

        this.Dispatcher.Invoke(action);
    }
}
