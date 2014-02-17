namespace DataTemplateDemo
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Media.Media3D;

    /// <summary>
    ///     Represents a model that can be used to present a collection of items.supports generating child items by a
    ///     <see cref="DataTemplate3D" />.
    /// </summary>
    /// <remarks>
    ///     Use the ItemsSource property to specify the collection to use to generate the content of your ItemsVisual3D. You can set the ItemsSource
    ///     property to any type that implements IEnumerable. ItemsSource is typically used to display a data collection or to bind an
    ///     ItemsControl to a collection object.
    /// </remarks>
    public class ItemsVisual3D : ModelVisual3D
    {
        /// <summary>
        ///     The item template property
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", typeof(DataTemplate3D), typeof(ItemsVisual3D), new PropertyMetadata(null));

        /// <summary>
        ///     The items source property
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(IEnumerable),
            typeof(ItemsVisual3D),
            new PropertyMetadata(null, (s, e) => ((ItemsVisual3D)s).ItemsSourceChanged(e)));

        /// <summary>
        ///     Gets or sets the <see cref="DataTemplate3D" /> used to display each item.
        /// </summary>
        /// <value>
        ///     The item template.
        /// </value>
        public DataTemplate3D ItemTemplate
        {
            get
            {
                return (DataTemplate3D)this.GetValue(ItemTemplateProperty);
            }

            set
            {
                this.SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        ///     Gets or sets a collection used to generate the content of the <see cref="ItemsVisual3D" />.
        /// </summary>
        /// <value>
        ///     The items source.
        /// </value>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(ItemsSourceProperty);
            }

            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Handles changes in the ItemsSource property.
        /// </summary>
        /// <param name="e">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        /// <exception cref="System.InvalidOperationException">
        /// Cannot create a Model3D from ItemTemplate.
        /// </exception>
        private void ItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (var item in this.ItemsSource)
            {
                var model = this.ItemTemplate.CreateItem(item);
                if (model != null)
                {
                    // todo: set up bindings?
                    // Cannot set DataContext, set bindings manually
                    // http://stackoverflow.com/questions/7725313/how-can-i-use-databinding-for-3d-elements-like-visual3d-or-uielement3d
                    this.Children.Add(model);
                }
                else
                {
                    throw new InvalidOperationException("Cannot create a Model3D from ItemTemplate.");
                }
            }
        }
    }
}