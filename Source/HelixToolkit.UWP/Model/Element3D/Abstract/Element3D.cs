/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using HelixToolkit.UWP.Model;
using SharpDX;
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
        /// 
        /// </summary>
        public new static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Element3D), new PropertyMetadata(Visibility.Visible, (d, e) =>
            {
                (d as Element3D).SceneNode.Visible = (Visibility)e.NewValue == Visibility.Visible && (d as Element3D).IsRendering;
            }));

        /// <summary>
        /// 
        /// </summary>
        public new Visibility Visibility
        {
            set
            {
                SetValue(VisibilityProperty, value);
            }
            get
            {
                return (Visibility)GetValue(VisibilityProperty);
            }
        }

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
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Matrix), typeof(Element3D), new PropertyMetadata(Matrix.Identity,
                (d, e) =>
                {
                    (d as Element3DCore).SceneNode.ModelMatrix = (Matrix)e.NewValue;
                }));
        /// <summary>
        /// 
        /// </summary>
        public Matrix Transform
        {
            get { return (Matrix)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }

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

        /// <summary>
        /// The is hit test visible property
        /// </summary>
        public new static readonly DependencyProperty IsHitTestVisibleProperty = DependencyProperty.Register("IsHitTestVisible", typeof(bool), typeof(Element3D),
            new PropertyMetadata(true, (d, e) => {
                (d as Element3D).SceneNode.IsHitTestVisible = (bool)e.NewValue;
            }));

        /// <summary>
        /// Indicates, if this element should be hit-tested.
        /// default is true
        /// </summary>
        public new bool IsHitTestVisible
        {
            set
            {
                SetValue(IsHitTestVisibleProperty, value);
            }
            get
            {
                return (bool)GetValue(IsHitTestVisibleProperty);
            }
        }
        #endregion
    }
}
