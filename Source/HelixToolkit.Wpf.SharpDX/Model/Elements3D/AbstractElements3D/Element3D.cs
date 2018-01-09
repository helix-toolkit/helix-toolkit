// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Element3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Base class for renderable elements.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using SharpDX;
namespace HelixToolkit.Wpf.SharpDX
{
    using System.Windows;
    using Media = System.Windows.Media;
    using Core;
    using Transform3D = System.Windows.Media.Media3D.Transform3D;
    using System.Collections.Generic;

    /// <summary>
    /// Base class for renderable elements.
    /// </summary>    
    public abstract class Element3D : Element3DCore, IVisible
    {
        #region Dependency Properties
        /// <summary>
        /// Indicates, if this element should be rendered,
        /// default is true
        /// </summary>
        public static readonly DependencyProperty IsRenderingProperty =
            DependencyProperty.Register("IsRendering", typeof(bool), typeof(Element3D), new AffectsRenderPropertyMetadata(true,
                (d, e) =>
                {
                    (d as Element3D).Visible = (bool)e.NewValue && (d as Element3D).Visibility == Visibility.Visible;
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
        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.Register("Visibility", typeof(Visibility), typeof(Element3D), new AffectsRenderPropertyMetadata(Visibility.Visible, (d, e) =>
            {
                (d as Element3D).Visible = (Visibility)e.NewValue == Visibility.Visible && (d as Element3D).IsRendering;
            }));

        /// <summary>
        /// 
        /// </summary>
        public Visibility Visibility
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
        /// 
        /// </summary>
        public static readonly DependencyProperty TransformProperty =
            DependencyProperty.Register("Transform", typeof(Transform3D), typeof(Element3D), new AffectsRenderPropertyMetadata(Transform3D.Identity, (d,e)=>
            {
                ((Element3D)d).ModelMatrix = e.NewValue != null ? ((Transform3D)e.NewValue).Value.ToMatrix() : Matrix.Identity;
            }));
        /// <summary>
        /// 
        /// </summary>
        public Transform3D Transform
        {
            get { return (Transform3D)this.GetValue(TransformProperty); }
            set { this.SetValue(TransformProperty, value); }
        }
        #endregion
        /// <summary>
        /// Looks for the first visual ancestor of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of visual ancestor.</typeparam>
        /// <param name="obj">The respective <see cref="DependencyObject"/>.</param>
        /// <returns>
        /// The first visual ancestor of type <typeparamref name="T"/> if exists, else <c>null</c>.
        /// </returns>
        public static T FindVisualAncestor<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                var parent = Media.VisualTreeHelper.GetParent(obj);
                while (parent != null)
                {
                    var typed = parent as T;
                    if (typed != null)
                    {
                        return typed;
                    }

                    parent = Media.VisualTreeHelper.GetParent(parent);
                }
            }

            return null;
        }

        ///// <summary>
        ///// Invoked whenever the effective value of any dependency property on this <see cref="Element3D"/> has been updated.
        ///// </summary>
        ///// <param name="e">The event data that describes the property that changed, as well as old and new values.</param>
        //protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        //{
        //    if (CheckAffectsRender(e))
        //    {
        //        this.InvalidateRender();
        //    }
        //    base.OnPropertyChanged(e);
        //}
        ///// <summary>
        ///// Check if dependency property changed event affects render
        ///// </summary>
        ///// <param name="e"></param>
        ///// <returns></returns>
        //protected virtual bool CheckAffectsRender(DependencyPropertyChangedEventArgs e)
        //{            
        //    // Possible improvement: Only invalidate if the property metadata has the flag "AffectsRender".
        //    // => Need to change all relevant DP's metadata to FrameworkPropertyMetadata or to a new "AffectsRenderPropertyMetadata".
        //    PropertyMetadata fmetadata = null;
        //    return ((fmetadata = e.Property.GetMetadata(this)) != null
        //        && (fmetadata is IAffectsRender
        //        || (fmetadata is FrameworkPropertyMetadata && (fmetadata as FrameworkPropertyMetadata).AffectsRender)
        //        ));
        //}
    }
}
