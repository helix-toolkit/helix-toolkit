// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GroupElement3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Markup;

    [ContentProperty("Items")]
    public abstract class GroupElement3D : Element3D //, IElement3DCollection
    {
        private Element3DCollection childrenInternal;

        public Element3DCollection ItemsSource
        {
            get { return (Element3DCollection)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(Element3DCollection), typeof(GroupElement3D),
                new AffectsRenderPropertyMetadata(null, 
                    (d, e) => {
                        (d as GroupElement3D).OnItemsSourceChanged(e.NewValue as Element3DCollection);
                    }));

        public Element3DCollection Items
        {
            get;
        } = new Element3DCollection();

        public GroupElement3D()
        {
        }

        private void OnItemsSourceChanged(Element3DCollection itemsSource)
        {
            if (childrenInternal != null)
            {
                foreach (var c in this.childrenInternal)
                {
                    Items.Remove(c);
                    c.Detach();
                    if (c.Parent == this)
                    {
                        this.RemoveLogicalChild(c);
                    }
                }
            }
            childrenInternal = itemsSource;
            if (childrenInternal != null)
            {
                Items.AddRange(childrenInternal);
                if (IsAttached)
                {
                    foreach (var c in this.childrenInternal)
                    {
                        if (c.Parent == null)
                        {
                            this.AddLogicalChild(c);
                        }

                        c.Attach(this.RenderHost);                   
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
            return Items.Count > 0;
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