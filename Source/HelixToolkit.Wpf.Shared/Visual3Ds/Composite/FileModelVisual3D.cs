// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileModelVisual3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   A visual element that shows a model loaded from a file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that shows a model loaded from a file.
    /// </summary>
    /// <remarks>
    /// Supported file formats: .3ds .obj .lwo .stl .off
    /// </remarks>
    public class FileModelVisual3D : UIElement3D
    {
        /// <summary>
        /// Identifies the <see cref="DefaultMaterial"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DefaultMaterialProperty =
            DependencyProperty.Register(
                "DefaultMaterial", typeof(Material), typeof(FileModelVisual3D), new PropertyMetadata(null, SourceChanged));

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
        /// Gets or sets the default material.
        /// </summary>
        /// <value>
        /// The default material.
        /// </value>
        public Material DefaultMaterial
        {
            get
            {
                return (Material)this.GetValue(DefaultMaterialProperty);
            }

            set
            {
                this.SetValue(DefaultMaterialProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the source file name.
        /// </summary>
        /// <value> The source file name. </value>
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
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        protected static void SourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((FileModelVisual3D)obj).SourceChanged();
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
        /// Called when the source or default material changed.
        /// </summary>
        protected virtual void SourceChanged()
        {
            var importer = new ModelImporter { DefaultMaterial = this.DefaultMaterial };
            this.Visual3DModel = this.Source != null ? importer.Load(this.Source) : null;
            this.OnModelLoaded();
        }
    }
}