using System.Windows;
using System.Windows.Media.Media3D;

namespace HelixToolkit.Wpf;

/// <summary>
/// A visual element that shows a terrain model.
/// </summary>
/// <remarks>
/// The following terrain model file formats are supported:
/// .bt
/// .btz (gzip compressed .bt)
/// .hgt (SRTM1, SRTM3)
/// .hgt.zip (SRTM1, SRTM3) compressed
///  <para>
/// The origin of model will be at the midpoint of the terrain.
/// A compression method to convert from ".bt" to ".btz" can be found in the GZipHelper.
/// Note that no LOD algorithm is implemented - this is for small terrains only...
///  </para>
/// </remarks>
public class TerrainVisual3D : ModelVisual3D
{
    /// <summary>
    /// Identifies the <see cref="Source"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
        "Source", typeof(string), typeof(TerrainVisual3D), new UIPropertyMetadata(null, SourceChanged));

    /// <summary>
    /// Identifies the <see cref="Model"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
        "Model", typeof(ITerrainModel), typeof(TerrainVisual3D), new UIPropertyMetadata(null, ModelChanged));

    /// <summary>
    /// The visual child.
    /// </summary>
    private readonly ModelVisual3D visualChild;

    /// <summary>
    /// Initializes a new instance of the <see cref = "TerrainVisual3D" /> class.
    /// </summary>
    public TerrainVisual3D()
    {
        this.visualChild = new ModelVisual3D();
        this.Children.Add(this.visualChild);
    }

    /// <summary>
    /// Gets or sets the source terrain file.
    /// </summary>
    /// <value>The source.</value>
    public string Source
    {
        get
        {
            return (string)this.GetValue(SourceProperty);
        }

        set
        {
            this.SetValue(SourceProperty, value);
        }
    }

    /// <summary>
    /// Gets or sets the terrain model.
    /// </summary>
    /// <value>The terrain model.</value>
    public ITerrainModel Model
    {
        get
        {
            return (ITerrainModel)this.GetValue(ModelProperty);
        }

        set
        {
            this.SetValue(ModelProperty, value);
        }
    }

    /// <summary>
    /// The source changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    protected static void SourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        ((TerrainVisual3D)obj).UpdateModel();
    }

    /// <summary>
    /// The source object changed.
    /// </summary>
    /// <param name="obj">
    /// The obj.
    /// </param>
    /// <param name="args">
    /// The args.
    /// </param>
    protected static void ModelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        ((TerrainVisual3D)obj).UpdateModel();
    }

    /// <summary>
    /// Updates the model.
    /// </summary>
    private void UpdateModel()
    {
        var r = GetTerrainModel();
        if (r == null) return;

        if (!string.IsNullOrWhiteSpace(this.Source))
            r.Load(this.Source);

        //r.Texture = new SlopeDirectionTexture(0);
        r.Texture ??= new SlopeTexture(8);

        // r.Texture = new MapTexture(@"D:\tmp\CraterLake.png") { Left = r.Left, Right = r.Right, Top = r.Top, Bottom = r.Bottom };
        this.visualChild.Content = r.CreateModel(r.Lod);
    }

    private ITerrainModel? GetTerrainModel()
    {
        if (this.Model != null)
            return this.Model;

        else if (this.Source.EndsWith(".bt", StringComparison.InvariantCultureIgnoreCase) ||
            this.Source.EndsWith(".btz", StringComparison.InvariantCultureIgnoreCase))
            return new TerrainModel();
                
        else if (this.Source.EndsWith(".hgt", StringComparison.InvariantCultureIgnoreCase) ||
                 this.Source.EndsWith(".hgt.zip", StringComparison.InvariantCultureIgnoreCase))
            return new HgtTerrainModel();

        return null;
    }
}
