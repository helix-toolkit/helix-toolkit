namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media.Media3D;

    using global::SharpDX;

    public class GroupModel3D : GroupElement3D, ITransformable, IHitable, IVisible
    {
        private Stack<Matrix> matrixStack = new Stack<Matrix>();

        protected Matrix modelMatrix = Matrix.Identity;
       
        public Matrix ModelMatrix
        {
            get { return this.modelMatrix; }
        }

        public void PushMatrix(Matrix matrix)
        {
            matrixStack.Push(this.modelMatrix);
            this.modelMatrix = this.modelMatrix * matrix;
        }

        public void PopMatrix()
        {
            this.modelMatrix = matrixStack.Pop();
        }     

        public Transform3D Transform
        {
            get { return (Transform3D)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }

        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform3D), typeof(GroupModel3D), new UIPropertyMetadata(Transform3D.Identity, TransformPropertyChanged));

        private static void TransformPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((GroupModel3D)d).OnTransformChanged(e);
        }


        protected virtual void OnTransformChanged(DependencyPropertyChangedEventArgs e)
        {
            var trafo = this.Transform.Value;            
            this.modelMatrix = trafo.ToMatrix();
        }

        public override void Attach(IRenderHost host)
        {
            this.renderTechnique = host.RenderTechnique;
            base.Attach(host);
        }

        public override void Render(RenderContext renderContext)
        {
            /// --- check to render the model
            {
                if (!this.IsRendering)
                    return;

                if (this.Visibility != System.Windows.Visibility.Visible)
                    return;

                //if (renderContext.IsShadowPass)
                //    if (!this.IsThrowingShadow)
                //        return;
            }

            foreach (var c in this.Children)
            {
                var model = c as ITransformable;
                if (model != null)
                {
                    // push matrix                    
                    model.PushMatrix(this.modelMatrix);
                    // render model
                    c.Render(renderContext);
                    // pop matrix                   
                    model.PopMatrix();
                }
                else
                {
                    c.Render(renderContext);
                }
            }
        }

        public virtual bool HitTest(Ray ray, ref List<HitTestResult> hits)
        {
            if (this.Visibility == Visibility.Collapsed)
            {
                return false;
            }
            if (this.IsHitTestVisible == false)
            {
                return false;
            }

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
            if (hit)
            {
                hits = hits.OrderBy(x => Vector3.DistanceSquared(ray.Position, x.PointHit.ToVector3())).ToList();
            }            
            return hit;
        }        
    }
}