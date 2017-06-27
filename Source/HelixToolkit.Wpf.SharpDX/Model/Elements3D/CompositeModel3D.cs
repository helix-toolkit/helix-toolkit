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
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows.Markup;
    using System.Linq;
    using global::SharpDX;
    using System;

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
        protected override bool OnAttach(IRenderHost host)
        {
            foreach (var model in this.Children)
            {
                if (model.Parent == null)
                {
                    this.AddLogicalChild(model);
                }

                model.Attach(host);
            }
            return true;
        }

        /// <summary>
        ///     Detaches this instance.
        /// </summary>
        protected override void OnDetach()
        {
            foreach (var model in this.Children)
            {
                model.Detach();
                if (model.Parent == this)
                {
                    this.RemoveLogicalChild(model);
                }
            }
            base.OnDetach();
        }

        protected override bool CanRender(RenderContext context)
        {
            return IsAttached && isRenderingInternal && visibleInternal;
        }
        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected override void OnRender(RenderContext context)
        {
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
            if (e.OldItems != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
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
            }

            if (e.NewItems != null)
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Reset:
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
            }
            UpdateBounds();
        }

        /// <summary>
        /// a Model3D does not have bounds, 
        /// if you want to have a model with bounds, use GeometryModel3D instead:
        /// but this prevents the CompositeModel3D containg lights, etc. (Lights3D are Models3D, which do not have bounds)
        /// </summary>
        protected void UpdateBounds()
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

        protected override bool CanHitTest(IRenderMatrices context)
        {
            return IsAttached && visibleInternal && isRenderingInternal && isHitTestVisibleInternal;
        }

        protected override bool CheckGeometry()
        {
            return true;
        }
        /// <summary>
        /// Compute hit-testing for all children
        /// </summary>
        protected override bool OnHitTest(IRenderMatrices context, Ray ray, ref List<HitTestResult> hits)
        {
            bool hit = false;
            foreach (var c in this.Children)
            {
                var hc = c as IHitable;
                if (hc != null)
                {
                    var tc = c as ITransformable;
                    if (tc != null)
                    {
                        tc.PushMatrix(this.modelMatrix);
                        if (hc.HitTest(context, ray, ref hits))
                        {
                            hit = true;
                        }
                        tc.PopMatrix();
                    }
                    else
                    {
                        if (hc.HitTest(context, ray, ref hits))
                        {
                            hit = true;
                        }
                    }
                }
            }
            if (hit)
            {
                hits = hits.OrderBy(x => Vector3.DistanceSquared(ray.Position, x.PointHit.ToVector3())).ToList();
            }
            return hit;
        }

        protected override void OnCreateGeometryBuffers()
        {
            
        }
    }
}