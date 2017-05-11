// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformableItems3DControl.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Compute hit-testing for all children
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System.Collections.Generic;
    using System.Windows;

    using global::SharpDX;

    using Transform3D = System.Windows.Media.Media3D.Transform3D;

    public class TransformableItems3DControl : Items3DControl, ITransformable
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

        public System.Windows.Media.Media3D.Transform3D Transform
        {
            get { return (Transform3D)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }

        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform3D), typeof(TransformableItems3DControl),
                new AffectsRenderPropertyMetadata(Transform3D.Identity, TransformPropertyChanged));

        private static void TransformPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TransformableItems3DControl)d).OnTransformChanged(e);
        }

        protected virtual void OnTransformChanged(DependencyPropertyChangedEventArgs e)
        {
            var trafo = this.Transform.Value;
            this.modelMatrix = trafo.ToMatrix();
        }


        /// <summary>
        /// Compute hit-testing for all children
        /// </summary>
        protected override bool OnHitTest(IRenderMatrices context, Ray ray, ref List<HitTestResult> hits)
        {
            bool hit = false;

            foreach (var item in this.children)
            {
                var hc = item as IHitable;
                if (hc != null)
                {
                    var tc = item as ITransformable;
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
            return hit;
        }


        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public override void Render(RenderContext context)
        {
            foreach (var item in this.children)
            {
                var element = item as Element3D;
                if (element != null)
                {
                    if (!element.IsAttached)
                    {
                        element.Attach(this.renderHost);
                    }

                    var model = item as ITransformable;
                    if (model != null)
                    {
                        // push matrix                    
                        model.PushMatrix(this.modelMatrix);
                        // render model
                        element.Render(context);
                        // pop matrix                   
                        model.PopMatrix();
                    }
                    else
                    {
                        element.Render(context);
                    }
                }
            }
        }
    }
}