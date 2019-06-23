// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TerrainVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a terrain model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a terrain model.
    /// </summary>
    /// <remarks>
    /// The following terrrain model file formats are supported:
    /// .bt
    /// .btz (gzip compressed .bt)
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
        /// Updates the model.
        /// </summary>
        private void UpdateModel()
        {
            var r = new TerrainModel();
            r.Load(this.Source);

            // r.Texture = new SlopeDirectionTexture(0);
            r.Texture = new SlopeTexture(8);

            // r.Texture = new MapTexture(@"D:\tmp\CraterLake.png") { Left = r.Left, Right = r.Right, Top = r.Top, Bottom = r.Bottom };
            this.visualChild.Content = r.CreateModel(2);
        }

    }
}