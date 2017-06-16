// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupElement3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Markup;
    using System.Linq;

    [ContentProperty("Children")]
    public abstract class GroupElement3D : Element3D //, IElement3DCollection
    {
        private ObservableElement3DCollection itemsSourceInternal;

        public ObservableElement3DCollection ItemsSource
        {
            get { return (ObservableElement3DCollection)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(ObservableElement3DCollection), typeof(GroupElement3D),
                new AffectsRenderPropertyMetadata(null, 
                    (d, e) => {
                        (d as GroupElement3D).OnItemsSourceChanged(e.NewValue as ObservableElement3DCollection);
                    }));

        public IEnumerable<Element3D> Items
        {
            get
            {
                return itemsSourceInternal == null ? Children : Children.Concat(itemsSourceInternal);
            }
        }

        public ObservableElement3DCollection Children
        {
            get;
        } = new ObservableElement3DCollection();

        public GroupElement3D()
        {
            Children.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach(Element3D c in e.OldItems)
                {
                    c.Detach();
                    if (c.Parent == this)
                    {
                        this.RemoveLogicalChild(c);
                    }
                }
            }
            if (IsAttached)
            {               
                if(e.Action== NotifyCollectionChangedAction.Reset)
                {
                    foreach (Element3D c in (sender as IEnumerable<Element3D>))
                    {
                        if (c.Parent == null)
                        {
                            this.AddLogicalChild(c);
                        }

                        c.Attach(renderHost);
                    }
                }
                else if(e.NewItems != null)
                {
                    foreach (Element3D c in e.NewItems)
                    {
                        if (c.Parent == null)
                        {
                            this.AddLogicalChild(c);
                        }

                        c.Attach(renderHost);
                    }
                }
            }
        }

        private void OnItemsSourceChanged(ObservableElement3DCollection itemsSource)
        {
            if (itemsSourceInternal != null)
            {
                itemsSourceInternal.CollectionChanged -= Items_CollectionChanged;
                foreach (var c in this.itemsSourceInternal)
                {
                    c.Detach();
                    if (c.Parent == this)
                    {
                        this.RemoveLogicalChild(c);
                    }
                }
            }
            itemsSourceInternal = itemsSource;
            if (itemsSourceInternal != null)
            {
                itemsSourceInternal.CollectionChanged += Items_CollectionChanged;
                if (IsAttached)
                {
                    foreach (var c in this.itemsSourceInternal)
                    {
                        if (c.Parent == null)
                        {
                            this.AddLogicalChild(c);
                        }

                        c.Attach(renderHost);                   
                    }  
                }            
            }
        }

        protected override bool OnAttach(IRenderHost host)
        {
            foreach (var c in this.Items)
            {
                if (c.Parent == null)
                {
                    this.AddLogicalChild(c);
                }

                c.Attach(host);
            }
            return true;
        }

        protected override void OnDetach()
        {
            base.OnDetach();
            foreach (var c in this.Items)
            {
                c.Detach();
                if (c.Parent == this)
                {
                    this.RemoveLogicalChild(c);
                }
            }
        }

        protected override bool CanRender(RenderContext context)
        {
            return true;
        }

        protected override void OnRender(RenderContext context)
        {
            foreach (var c in this.Items)
            {
                c.Render(context);
            }
        }
    }
}