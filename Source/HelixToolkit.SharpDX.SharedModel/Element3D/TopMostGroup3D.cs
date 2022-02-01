/*
The MIT License (MIT)
Copyright (c) 2021 Helix Toolkit contributors
*/
#if NETFX_CORE
using  Windows.UI.Xaml;

namespace HelixToolkit.UWP
#elif WINUI 
using Microsoft.UI.Xaml;
using HelixToolkit.SharpDX.Core.Model.Scene;
namespace HelixToolkit.WinUI
#else
#if COREWPF
using HelixToolkit.SharpDX.Core.Model.Scene;
#endif
using System.Windows;
namespace HelixToolkit.Wpf.SharpDX
#endif
{
#if !COREWPF && !WINUI
    using Model.Scene;
#endif
    using Model;
    /// <summary>
    /// Provides a way to render child elements always on top of other elements
    /// This is rendered at the same level of screen spaced group items.
    /// Child items do not support post effects.
    /// </summary>
    public class TopMostGroup3D : GroupModel3D
    {
        /// <summary>
        /// Gets or sets a value indicating whether [enable top most mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable top most mode]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTopMost
        {
            get
            {
                return (bool)GetValue(EnableTopMostProperty);
            }
            set
            {
                SetValue(EnableTopMostProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for EnableTopMost.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableTopMostProperty =
            DependencyProperty.Register("EnableTopMost", typeof(bool), typeof(TopMostGroup3D), new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as TopMostGroupNode).EnableTopMost = (bool)e.NewValue;
            }));


        protected override SceneNode OnCreateSceneNode()
        {
            return new TopMostGroupNode();
        }

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            (node as TopMostGroupNode).EnableTopMost = EnableTopMost;
            base.AssignDefaultValuesToSceneNode(node);
        }
    }
}
