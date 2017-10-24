using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Media = System.Windows.Media;
using HelixToolkit.SharpDX.Core2D;
using System.Windows;
using SharpDX;

namespace HelixToolkit.Wpf.SharpDX
{
    public abstract class Model2D : Element2D, ITransformable2D
    {
        public static readonly DependencyProperty TransformProperty = 
            DependencyProperty.Register("Transform", typeof(Media.Transform), typeof(Model2D), new AffectsRenderPropertyMetadata(Media.Transform.Identity, (d, e) =>
            {
                (d as Model2D).transformMatrix = e.NewValue == null ? Matrix3x3.Identity : ((Media.Transform)e.NewValue).Value.ToMatrix3x3();
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

        protected Matrix3x3 transformMatrix { private set; get; } = Matrix3x3.Identity;
    }
}
