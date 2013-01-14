// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileModelVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;

    /// <summary>
    /// A visual element that shows a model loaded from a file.
    /// </summary>
    /// <remarks>
    /// Supported file formats are: .3ds .obj .lwo .stl .off
    /// </remarks>
    public class FileModelVisual3D : UIElement3D
    {
        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            "Source", typeof(string), typeof(FileModelVisual3D), new UIPropertyMetadata(null, SourceChanged));

        /// <summary>
        /// The model loaded event.
        /// </summary>
        private static readonly RoutedEvent ModelLoadedEvent = EventManager.RegisterRoutedEvent(
            "ModelLoaded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FileModelVisual3D));

        /// <summary>
        /// Occurs when the model has been loaded.
        /// </summary>
        public event RoutedEventHandler ModelLoaded
        {
            add
            {
                this.AddHandler(ModelLoadedEvent, value);
            }

            remove
            {
                this.RemoveHandler(ModelLoadedEvent, value);
            }
        }

        /// <summary>
        /// Gets or sets the source.
        /// </summary>
        /// <value> The source. </value>
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
            ((FileModelVisual3D)obj).OnSourceChanged();
        }

        /// <summary>
        /// Called when the model has been loaded.
        /// </summary>
        protected virtual void OnModelLoaded()
        {
            var args = new RoutedEventArgs { RoutedEvent = ModelLoadedEvent };
            this.RaiseEvent(args);
        }

        /// <summary>
        /// Called when the source changed.
        /// </summary>
        protected virtual void OnSourceChanged()
        {
            this.Visual3DModel = this.Source != null ? ModelImporter.Load(this.Source) : null;
            this.OnModelLoaded();
        }

    }
}