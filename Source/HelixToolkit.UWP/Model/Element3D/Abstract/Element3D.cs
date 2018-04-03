/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.UWP.Model;
using SharpDX;
using Windows.Foundation;
using Windows.UI.Xaml;
namespace HelixToolkit.UWP
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="HelixToolkit.UWP.Model.Element3DCore" />
    public abstract class Element3D : Element3DCore
    {
        #region Dependency Properties
        /// <summary>
        /// Indicates, if this element should be rendered,
        /// default is true
        /// </summary>
        public static readonly DependencyProperty IsRenderingProperty =
            DependencyProperty.Register("IsRendering", typeof(bool), typeof(Element3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    (d as Element3D).SceneNode.Visible = (bool)e.NewValue && (d as Element3D).Visibility == Visibility.Visible;
                }));

        /// <summary>
        /// Indicates, if this element should be rendered.
        /// Use this also to make the model visible/unvisible
        /// default is true
        /// </summary>
        public bool IsRendering
        {
            get { return (bool)GetValue(IsRenderingProperty); }
            set { SetValue(IsRenderingProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public new static readonly DependencyProperty Transform3DProperty =
            DependencyProperty.Register("Transform3D", typeof(Matrix), typeof(Element3D), new PropertyMetadata(Matrix.Identity,
                (d, e) =>
                {
                    (d as Element3DCore).SceneNode.ModelMatrix = (Matrix)e.NewValue;
                }));

        /// <summary>
        /// 
        /// </summary>
        public new Matrix Transform3D
        {
            get { return (Matrix)this.GetValue(Transform3DProperty); }
            set { this.SetValue(Transform3DProperty, value); }
        }
        /// <summary>
        /// The is throwing shadow property
        /// </summary>
        public static readonly DependencyProperty IsThrowingShadowProperty =
            DependencyProperty.Register("IsThrowingShadow", typeof(bool), typeof(Element3D), new PropertyMetadata(false, (d, e) =>
            {
                if ((d as Element3DCore).SceneNode is Core.IThrowingShadow t)
                {
                    t.IsThrowingShadow = (bool)e.NewValue;
                }
            }));
        /// <summary>
        /// <see cref="Core.IThrowingShadow.IsThrowingShadow"/>
        /// </summary>
        public bool IsThrowingShadow
        {
            set
            {
                SetValue(IsThrowingShadowProperty, value);
            }
            get
            {
                return (bool)GetValue(IsThrowingShadowProperty);
            }
        }

        #endregion        
        /// <summary>
        /// Initializes a new instance of the <see cref="Element3D"/> class.
        /// </summary>
        public Element3D()
        {
            RegisterPropertyChangedCallback(VisibilityProperty, (s, e) =>
            {
                SceneNode.Visible = (Visibility)s.GetValue(e) == Visibility.Visible && IsRendering;
            });

            RegisterPropertyChangedCallback(IsHitTestVisibleProperty, (s, e) =>
            {
                SceneNode.IsHitTestVisible = (bool)s.GetValue(e);
            });
        }
        private static readonly Size oneSize = new Size(1, 1);
        protected override Size ArrangeOverride(Size finalSize)
        {
            return oneSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return oneSize;
        }
    }
}
