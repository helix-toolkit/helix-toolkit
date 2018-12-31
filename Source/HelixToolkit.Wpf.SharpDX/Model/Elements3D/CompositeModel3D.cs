// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Represents a composite Model3D.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using Model.Scene;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Markup;

    /// <summary>
    ///     Represents a composite Model3D.
    /// </summary>
    [ContentProperty("Children")]
    public class CompositeModel3D : Element3D, IHitable, ISelectable, IMouse3D
    {
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(CompositeModel3D), new PropertyMetadata(false));

        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        ///     Gets the children.
        /// </summary>
        /// <value>
        ///     The children.
        /// </value>
        public ObservableElement3DCollection Children { get; } = new ObservableElement3DCollection();

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompositeModel3D" /> class.
        /// </summary>
        public CompositeModel3D()
        {
            Children.CollectionChanged += this.ChildrenChanged;
            Loaded += GroupElement3D_Loaded;
        }

        private void GroupElement3D_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Element3D c in Children)
            {
                if (c.Parent == this)
                {
                    this.RemoveLogicalChild(c);
                }
            }
            foreach (Element3D c in Children)
            {
                if (c.Parent == null)
                {
                    this.AddLogicalChild(c);
                }
            }
        }

        /// <summary>
        /// Handles changes in the Children collection.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var node = SceneNode as GroupNode;
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldItems != null)
                    {
                        foreach (Element3D item in e.OldItems)
                        {
                            if (item.Parent == this)
                            {
                                this.RemoveLogicalChild(item);
                            }
                            node.RemoveChildNode(item.SceneNode);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems != null)
                    {
                        foreach (Element3D item in e.OldItems)
                        {
                            if (item.Parent == this)
                            {
                                this.RemoveLogicalChild(item);
                            }
                        }
                    }
                    node.Clear();
                    break;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    foreach (Element3D item in Children)
                    {
                        if (item.Parent == null)
                        {
                            this.AddLogicalChild(item);
                        }
                        node.AddChildNode(item.SceneNode);
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    foreach (Element3D item in e.NewItems)
                    {
                        if (item.Parent == null)
                        {
                            this.AddLogicalChild(item);
                        }
                        node.AddChildNode(item.SceneNode);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    node.MoveChildNode(e.OldStartingIndex, e.NewStartingIndex);
                    break;
            }
        }

        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode();
        }
    }
}