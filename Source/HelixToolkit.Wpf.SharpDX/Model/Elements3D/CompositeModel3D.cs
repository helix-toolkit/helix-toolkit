// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeModel3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows.Markup;

    using global::SharpDX;

    /// <summary>
    ///     Represents a composite Model3D.
    /// </summary>
    [ContentProperty("Children")]
    public class CompositeModel3D : GeometryModel3D
    {
        private readonly ObservableElement3DCollection children;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompositeModel3D" /> class.
        /// </summary>
        public CompositeModel3D()
        {
            this.children = new ObservableElement3DCollection();
            this.children.CollectionChanged += this.ChildrenChanged;
        }

        /// <summary>
        ///     Gets the children.
        /// </summary>
        /// <value>
        ///     The children.
        /// </value>
        public ObservableElement3DCollection Children { get { return this.children; } }

        /// <summary>
        /// Attaches the specified host.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        public override void Attach(IRenderHost host)
        {
            base.Attach(host);
            foreach (var model in this.Children)
            {
                if (model.Parent == null)
                {
                    this.AddLogicalChild(model);                    
                }

                model.Attach(host);
            }
        }

        /// <summary>
        ///     Detaches this instance.
        /// </summary>
        public override void Detach()
        {
            foreach (var model in this.Children)
            {
                model.Detach();
                if (model.Parent == this)
                {
                    this.RemoveLogicalChild(model);                    
                }
            }
            base.Detach();
        }

        /// <summary>
        /// Compute hit-testing for all children
        /// </summary>
        public override bool HitTest(Ray ray, ref List<HitTestResult> hits)
        {
            bool hit = base.HitTest(ray, ref hits);
            
            foreach (var c in this.Children)
            {
                var hc = c as IHitable;
                if (hc != null)
                {
                    var tc = c as ITransformable;
                    if (tc != null)
                    {
                        tc.PushMatrix(this.modelMatrix);
                        if (hc.HitTest(ray, ref hits))
                        {
                            hit = true;
                        }
                        tc.PopMatrix();
                    }
                    else
                    {
                        if (hc.HitTest(ray, ref hits))
                        {
                            hit = true;
                        }
                    }
                }
            }
            return hit;
        }        

        /// <summary>
        /// 
        /// </summary>
        public override void Dispose()
        {
            this.Detach();
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void Render(RenderContext context)
        {
            base.Render(context);

            // you mean like this?
            foreach (var c in this.Children)
            {
                var model = c as ITransformable;
                if (model != null)
                {
                    // push matrix                    
                    model.PushMatrix(this.modelMatrix);
                    // render model
                    c.Render(context);
                    // pop matrix                   
                    model.PopMatrix();
                }
                else
                {
                    c.Render(context);
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
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                    foreach (Model3D item in e.OldItems)
                    {
                        // todo: detach?
                        // yes, always
                        item.Detach();
                        if (item.Parent == this)
                        {
                            this.RemoveLogicalChild(item);                            
                        }
                    }

                    break;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    foreach (Model3D item in e.NewItems)
                    {
                        if (this.IsAttached)
                        {
                            // todo: attach?
                            // yes, always  
                            // where to get a refrence to renderHost?
                            // store it as private memeber of the class?
                            if (item.Parent == null)
                            {
                                this.AddLogicalChild(item);
                            }

                            item.Attach(this.renderHost);
                        }
                    }

                    break;
            }

            UpdateBounds();
        }
        
        /// <summary>
        /// a Model3D does not have bounds, 
        /// if you want to have a model with bounds, use GeometryModel3D instead:
        /// but this prevents the CompositeModel3D containg lights, etc. (Lights3D are Models3D, which do not have bounds)
        /// </summary>
        private void UpdateBounds()
        {
            var bb = this.Bounds;
            foreach (var item in this.Children)
            {
                var model = item as IBoundable;
                if (model != null)
                {
                    bb = BoundingBox.Merge(bb, model.Bounds);
                }
            }
            this.Bounds = bb;            
        }
    }
}
