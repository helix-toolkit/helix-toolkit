using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media = System.Windows.Media;
using System.Windows;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Core2D;
    public abstract class Model2D : Element2D, ITransformable2D
    {
        public static readonly DependencyProperty TransformProperty = 
            DependencyProperty.Register("Transform", typeof(Media.Transform), typeof(Model2D), new AffectsRenderPropertyMetadata(Media.Transform.Identity, (d, e) =>
            {
                (d as Model2D).transformMatrix = e.NewValue == null ? Matrix3x2.Identity : ((Media.Transform)e.NewValue).Value.ToMatrix3x2();
            }));

        /// <summary>
        /// Render transform
        /// </summary>
        public Media.Transform Transform
        {
            get
            {
                return (Media.Transform)GetValue(TransformProperty);
            }

            set
            {
                SetValue(TransformProperty, value);
            }
        }

        private Matrix3x2 transformMatrix = Matrix3x2.Identity;

        private readonly Stack<Matrix3x2> matrixStack = new Stack<Matrix3x2>();

        public Matrix3x2 TransformMatrix
        {
            get { return this.transformMatrix; }
        }

        public void PushMatrix(Matrix3x2 matrix)
        {
            matrixStack.Push(this.transformMatrix);
            this.transformMatrix = this.transformMatrix * matrix;
        }

        public void PopMatrix()
        {
            this.transformMatrix = matrixStack.Pop();
        }

        protected override void PreRender(IRenderContext2D context)
        {
            base.PreRender(context);
            if (RenderCore is ITransform2D)
            {
                ((ITransform2D)RenderCore).Transform = TransformMatrix;
            }
        }

        protected override bool OnHitTest(ref Vector2 mousePoint, out HitTest2DResult hitResult)
        {
            if (RenderCore != null && RenderCore.Bound.Contains(mousePoint))
            {
                hitResult = new HitTest2DResult(this);
                return true;
            }
            else
            {
                hitResult = null;
                return false;
            }
        }
    }
}
