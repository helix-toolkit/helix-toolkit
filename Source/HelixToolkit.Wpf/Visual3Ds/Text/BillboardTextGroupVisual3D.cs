// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextGroupBillboardVisual3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// <summary>
//   A visual element that contains a text billboard.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Media.Media3D;

    /// <summary>
    /// A visual element that contains a collection of text billboards.
    /// </summary>
    public class BillboardTextGroupVisual3D : RenderingModelVisual3D
    {
        /// <summary>
        /// Identifies the <see cref="Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(BillboardTextGroupVisual3D), new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="BorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush",
            typeof(Brush),
            typeof(BillboardTextGroupVisual3D),
            new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="BorderThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BorderThicknessProperty =
            DependencyProperty.Register(
                "BorderThickness",
                typeof(Thickness),
                typeof(BillboardTextGroupVisual3D),
                new UIPropertyMetadata(new Thickness(1), VisualChanged));

        /// <summary>
        /// Identifies the <see cref="FontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontFamilyProperty = DependencyProperty.Register(
            "FontFamily",
            typeof(FontFamily),
            typeof(BillboardTextGroupVisual3D),
            new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="FontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty = DependencyProperty.Register(
            "FontSize", typeof(double), typeof(BillboardTextGroupVisual3D), new UIPropertyMetadata(0.0, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="FontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FontWeightProperty = DependencyProperty.Register(
            "FontWeight",
            typeof(FontWeight),
            typeof(BillboardTextGroupVisual3D),
            new UIPropertyMetadata(FontWeights.Normal, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Foreground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground",
            typeof(Brush),
            typeof(BillboardTextGroupVisual3D),
            new UIPropertyMetadata(Brushes.Black, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="HeightFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeightFactorProperty = DependencyProperty.Register(
            "HeightFactor", typeof(double), typeof(BillboardTextGroupVisual3D), new PropertyMetadata(1.0, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Items"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items",
            typeof(IList<BillboardTextItem>),
            typeof(BillboardTextGroupVisual3D),
            new UIPropertyMetadata(null, VisualChanged));

        /// <summary>
        /// Identifies the <see cref="Padding"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PaddingProperty = DependencyProperty.Register(
            "Padding",
            typeof(Thickness),
            typeof(BillboardTextGroupVisual3D),
            new UIPropertyMetadata(new Thickness(0), VisualChanged));

        private readonly BillboardGeometryBuilder builder;

        /// <summary>
        /// The builder.
        /// </summary>
        private readonly Dictionary<MeshGeometry3D, IList<Billboard>> meshes =
            new Dictionary<MeshGeometry3D, IList<Billboard>>();

        /// <summary>
        /// The is rendering flag.
        /// </summary>
        private bool isRendering;

        /// <summary>
        /// Initializes a new instance of the <see cref="BillboardTextGroupVisual3D" /> class.
        /// </summary>
        public BillboardTextGroupVisual3D()
        {
            this.builder = new BillboardGeometryBuilder(this);
        }

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>The background.</value>
        public Brush Background
        {
            get
            {
                return (Brush)this.GetValue(BackgroundProperty);
            }
            set
            {
                this.SetValue(BackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        /// <value>The border brush.</value>
        public Brush BorderBrush
        {
            get
            {
                return (Brush)this.GetValue(BorderBrushProperty);
            }
            set
            {
                this.SetValue(BorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the border thickness.
        /// </summary>
        /// <value>The border thickness.</value>
        public Thickness BorderThickness
        {
            get
            {
                return (Thickness)this.GetValue(BorderThicknessProperty);
            }
            set
            {
                this.SetValue(BorderThicknessProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the font family.
        /// </summary>
        /// <value>The font family.</value>
        public FontFamily FontFamily
        {
            get
            {
                return (FontFamily)this.GetValue(FontFamilyProperty);
            }

            set
            {
                this.SetValue(FontFamilyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the size of the font.
        /// </summary>
        /// <value>The size of the font.</value>
        public double FontSize
        {
            get
            {
                return (double)this.GetValue(FontSizeProperty);
            }

            set
            {
                this.SetValue(FontSizeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        /// <value>The font weight.</value>
        public FontWeight FontWeight
        {
            get
            {
                return (FontWeight)this.GetValue(FontWeightProperty);
            }

            set
            {
                this.SetValue(FontWeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the foreground brush.
        /// </summary>
        /// <value>The foreground.</value>
        public Brush Foreground
        {
            get
            {
                return (Brush)this.GetValue(ForegroundProperty);
            }

            set
            {
                this.SetValue(ForegroundProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the height factor.
        /// </summary>
        /// <value>
        /// The height factor.
        /// </value>
        public double HeightFactor
        {
            get
            {
                return (double)this.GetValue(HeightFactorProperty);
            }

            set
            {
                this.SetValue(HeightFactorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is being rendered.
        /// When the visual is removed from the visual tree, this property should be set to false.
        /// </summary>
        public bool IsRendering
        {
            get
            {
                return this.isRendering;
            }

            set
            {
                if (value != this.isRendering)
                {
                    this.isRendering = value;
                    if (this.isRendering)
                    {
                        this.SubscribeToRenderingEvent();
                    }
                    else
                    {
                        this.UnsubscribeRenderingEvent();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<BillboardTextItem> Items
        {
            get
            {
                return (IList<BillboardTextItem>)this.GetValue(ItemsProperty);
            }
            set
            {
                this.SetValue(ItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the padding.
        /// </summary>
        /// <value>The padding.</value>
        public Thickness Padding
        {
            get
            {
                return (Thickness)this.GetValue(PaddingProperty);
            }
            set
            {
                this.SetValue(PaddingProperty, value);
            }
        }

        /// <summary>
        /// Handles the CompositionTarget.Rendering event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="eventArgs">The <see cref="System.Windows.Media.RenderingEventArgs" /> instance containing the event data.</param>
        protected override void OnCompositionTargetRendering(object sender, RenderingEventArgs eventArgs)
        {
            if (this.isRendering)
            {
                if (!Visual3DHelper.IsAttachedToViewport3D(this))
                {
                    return;
                }

                if (this.UpdateTransforms())
                {
                    this.UpdateGeometry();
                }
            }
        }

        /// <summary>
        /// Called when the parent of the 3-D visual object is changed.
        /// </summary>
        /// <param name="oldParent">
        /// A value of type <see cref="T:System.Windows.DependencyObject" /> that represents the previous parent of the
        ///     <see
        ///         cref="T:System.Windows.Media.Media3D.Visual3D" />
        /// object. If the
        ///     <see
        ///         cref="T:System.Windows.Media.Media3D.Visual3D" />
        /// object did not have a previous parent, the value of the parameter is null.
        /// </param>
        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            var parent = VisualTreeHelper.GetParent(this);
            this.IsRendering = parent != null;
        }

        /// <summary>
        /// Updates the geometry.
        /// </summary>
        protected void UpdateGeometry()
        {
            foreach (var m in this.meshes)
            {
                m.Key.Positions = this.builder.GetPositions(m.Value);
            }
        }

        /// <summary>
        /// Updates the transforms.
        /// </summary>
        /// <returns>
        /// True if the transform is updated.
        /// </returns>
        protected bool UpdateTransforms()
        {
            return this.builder.UpdateTransforms();
        }

        /// <summary>
        /// The visual appearance changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void VisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BillboardTextGroupVisual3D)d).VisualChanged();
        }

        private FrameworkElement CreateElement(string text)
        {
            var textBlock = new TextBlock(new Run(text))
                                {
                                    Foreground = this.Foreground,
                                    Background = this.Background,
                                    FontWeight = this.FontWeight,
                                    Padding = this.Padding
                                };

            if (this.FontFamily != null)
            {
                textBlock.FontFamily = this.FontFamily;
            }

            if (this.FontSize > 0)
            {
                textBlock.FontSize = this.FontSize;
            }

            if (this.BorderBrush != null)
            {
                return new Border
                           {
                               BorderBrush = this.BorderBrush,
                               BorderThickness = this.BorderThickness,
                               Child = textBlock,
                               //  Margin = new Thickness(1),
                           };
            }

            //   textBlock.Margin = new Thickness(1);
            return textBlock;
        }

        /// <summary>
        /// Updates the text block when the visual appearance changed.
        /// </summary>
        private void VisualChanged()
        {
            this.meshes.Clear();

            if (this.Items == null)
            {
                this.Content = null;
                return;
            }

            var items = this.Items.Where(i => !string.IsNullOrEmpty(i.Text)).ToList();
            var group = new Model3DGroup();
            while (items.Count > 0)
            {
                Dictionary<string, FrameworkElement> elementMap;
                Dictionary<FrameworkElement, Rect> elementPositions;
                var material = TextGroupVisual3D.CreateTextMaterial(
                    items, this.CreateElement, this.Background, out elementMap, out elementPositions);
                material.Freeze();

                var billboards = new List<Billboard>();
                var addedChildren = new List<BillboardTextItem>();
                var textureCoordinates = new PointCollection();
                foreach (var item in items)
                {
                    var element = elementMap[item.Text];
                    var r = elementPositions[element];
                    if (r.Bottom > 1)
                    {
                        break;
                    }

                    billboards.Add(
                        new Billboard(
                            item.Position,
                            element.ActualWidth,
                            element.ActualHeight,
                            item.HorizontalAlignment,
                            item.VerticalAlignment,
                            item.DepthOffset));
                    textureCoordinates.Add(new Point(r.Left, r.Bottom));
                    textureCoordinates.Add(new Point(r.Right, r.Bottom));
                    textureCoordinates.Add(new Point(r.Right, r.Top));
                    textureCoordinates.Add(new Point(r.Left, r.Top));

                    addedChildren.Add(item);
                }

                var g = new MeshGeometry3D
                            {
                                TriangleIndices = BillboardGeometryBuilder.CreateIndices(billboards.Count),
                                TextureCoordinates = textureCoordinates,
                                Positions = this.builder.GetPositions(billboards)
                            };
                group.Children.Add(new GeometryModel3D(g, material));
                this.meshes.Add(g, billboards);
                foreach (var c in addedChildren)
                {
                    items.Remove(c);
                }
            }

            this.Content = group;
        }
    }
}