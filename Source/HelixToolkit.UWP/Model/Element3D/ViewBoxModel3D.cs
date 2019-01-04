// <copyright file="CoordinateSystemModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2017 Helix Toolkit contributors
//   Author: Lunci Hua
// </copyright>
using System;
using System.IO;
using SharpDX;
using Windows.UI.Xaml;

namespace HelixToolkit.UWP
{
    using Model;
    using Model.Scene;
    

    /// <summary>
    /// <para>Viewbox replacement for Viewport using swapchain rendering.</para>
    /// <para>To replace box texture (such as text, colors), bind to custom material with different diffuseMap. </para>
    /// <para>Create a image with 1 row and 6 evenly distributed columns. Each column occupies one box face. The face order is Front, Back, Down, Up, Left, Right</para>
    /// </summary>
    public class ViewBoxModel3D : ScreenSpacedElement3D
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty UpDirectionProperty = DependencyProperty.Register("UpDirection", typeof(Vector3), typeof(ViewBoxModel3D),
            new PropertyMetadata(new Vector3(0, 1, 0),
            (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ViewBoxNode).UpDirection = ((Vector3)e.NewValue);
            }));


        /// <summary>
        /// Gets or sets up direction.
        /// </summary>
        /// <value>
        /// Up direction.
        /// </value>
        public Vector3 UpDirection
        {
            set
            {
                SetValue(UpDirectionProperty, value);
            }
            get
            {
                return (Vector3)GetValue(UpDirectionProperty);
            }
        }


        public static readonly DependencyProperty ViewBoxTextureProperty = DependencyProperty.Register("ViewBoxTexture", typeof(Stream), typeof(ViewBoxModel3D),
            new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ViewBoxNode).ViewBoxTexture = (Stream)e.NewValue;
            }));

        public Stream ViewBoxTexture
        {
            set
            {
                SetValue(ViewBoxTextureProperty, value);
            }
            get
            {
                return (Stream)GetValue(ViewBoxTextureProperty);
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [enable edge click].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable edge click]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableEdgeClick
        {
            get { return (bool)GetValue(EnableEdgeClickProperty); }
            set { SetValue(EnableEdgeClickProperty, value); }
        }

        /// <summary>
        /// The enable edge click property
        /// </summary>
        public static readonly DependencyProperty EnableEdgeClickProperty =
            DependencyProperty.Register("EnableEdgeClick", typeof(bool), typeof(ViewBoxModel3D), new PropertyMetadata(false, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as ViewBoxNode).EnableEdgeClick = (bool)e.NewValue;
            }));

        protected override SceneNode OnCreateSceneNode()
        {
            return new ViewBoxNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            (node as ViewBoxNode).UpDirection = this.UpDirection;
            base.AssignDefaultValuesToSceneNode(node);
        }
    }
}
