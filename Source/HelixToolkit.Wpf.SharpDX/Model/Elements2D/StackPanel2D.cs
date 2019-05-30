using System;
using System.Windows;
using System.Windows.Controls;
using HelixToolkit.Wpf.SharpDX.Core2D;
using SharpDX;
using System.Linq;

namespace HelixToolkit.Wpf.SharpDX.Elements2D
{
    using Model.Scene2D;
    using Extensions;
    using Orientation = System.Windows.Controls.Orientation;

    public class StackPanel2D : Panel2D
    {
        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// The orientation property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackPanel2D), new PropertyMetadata(Orientation.Horizontal, 
                (d, e) => 
                {
                    ((d as Element2DCore).SceneNode as StackPanelNode2D).Orientation = ((Orientation)e.NewValue).ToD2DOrientation();
                }));


        protected override SceneNode2D OnCreateSceneNode()
        {
            return new StackPanelNode2D();
        }
    }
}
