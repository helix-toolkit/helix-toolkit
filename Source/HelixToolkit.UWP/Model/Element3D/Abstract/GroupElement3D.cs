/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.UI.Xaml.Markup;

namespace HelixToolkit.UWP
{
    using Model.Scene;

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Element3D" />
    [ContentProperty(Name = "Children")]
    public abstract class GroupElement3D : Element3D
    {
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        public ObservableCollection<Element3D> Children
        {
            get;
        } = new ObservableCollection<Element3D>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupElement3D"/> class.
        /// </summary>
        public GroupElement3D()
        {
            Children.CollectionChanged += Items_CollectionChanged;           
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (itemsContainer != null)
            {
                itemsContainer.Items.Clear();
                foreach (var item in Children)
                {
                    if (item.Parent != itemsContainer)
                    {
                        itemsContainer.Items.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Called when [create scene node].
        /// </summary>
        /// <returns></returns>
        protected override SceneNode OnCreateSceneNode()
        {
            return new GroupNode();
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                DetachChildren(e.OldItems);
            }           
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                itemsContainer?.Items.Clear();
                var node = SceneNode as GroupNode;
                node.Clear();
                AttachChildren(sender as IList);
            }
            else if (e.NewItems != null)
            {
                AttachChildren(e.NewItems);
            }
        }
        /// <summary>
        /// Attaches the children.
        /// </summary>
        /// <param name="children">The children.</param>
        protected void AttachChildren(IList children)
        {
            var node = SceneNode as GroupNode;
            foreach (Element3D c in children)
            {
                if (node.AddChildNode(c) && itemsContainer != null)
                {
                    itemsContainer.Items.Add(c);
                }               
            }
        }
        /// <summary>
        /// Detaches the children.
        /// </summary>
        /// <param name="children">The children.</param>
        protected void DetachChildren(IList children)
        {
            var node = SceneNode as GroupNode;
            foreach (Element3D c in children)
            {                
                if(node.RemoveChildNode(c) && itemsContainer != null)
                {
                    itemsContainer.Items.Remove(c);
                }
            }
        }
    }
}
